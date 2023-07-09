using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Range(0f, 30f)]
    public float forwardSpeed = 10f;
    [Range(0f, 30f)]
    public float horizontalSpeed = 10f;
    [Range(0f, 30f)]
    public float fallSpeed = 10f;
    public float leftX = -2;
    public float rightX = 2;
    public float inputZoneWidth = 0.8f;
    public Animator stickman;
    public new Camera camera;
    public CubeHolder cubeHolder;
    public ParticleSystem cubeStackEffect;
    public CollectCubeText pickupTextSource;
    public float wallThickness = 1f;
    [NonSerialized]
    public new Rigidbody rigidbody;
    private float target = 0f;
    public event Action OnFatalCollision;
    private float recalcTimeoutLeft = float.PositiveInfinity;
    private float screenZ;
    private bool fatalCollisionInvoked = false;
    private bool vibrate = false;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        if (stickman == null)
        {
            stickman = transform.GetComponentInChildren<Animator>();
        }
        if (camera == null)
        {
            camera = Camera.main;
        }
        if (cubeHolder == null)
        {
            cubeHolder = transform.GetComponentInChildren<CubeHolder>();
        }
        if (cubeStackEffect == null)
        {
            transform.Find("CubeStackEffect").TryGetComponent(out cubeStackEffect);
        }
        if (pickupTextSource == null)
        {
            var found = Resources.FindObjectsOfTypeAll<CollectCubeText>();
            if (found.Length > 0)
            {
                pickupTextSource = found[0];
            }
        }
        screenZ = transform.position.z - camera.transform.position.z;
    }
    private void Update()
    {
        if (vibrate)
        {
            Handheld.Vibrate();
            if (camera.TryGetComponent(out Shaker shaker))
            {
                shaker.Shake();
            }
            vibrate = false;
        }
    }
    private void FixedUpdate()
    {
        Vector3 touchPosition = Vector3.forward;
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
        }
        else if (Input.GetMouseButton(0))
        {
            touchPosition = Input.mousePosition;
        }
        //Only if was reassigned
        if (touchPosition.z == 0)
        {
            touchPosition.z = screenZ;
            target = Mathf.Lerp(leftX, rightX, camera.ScreenToViewportPoint(touchPosition).x / inputZoneWidth);
        }
        rigidbody.velocity = new Vector3(
            Mathf.Clamp((target - rigidbody.position.x) / Time.fixedDeltaTime, -horizontalSpeed, horizontalSpeed),
            Mathf.Clamp(-transform.position.y / Time.fixedDeltaTime, -fallSpeed, fallSpeed),
            forwardSpeed
        );
        if (recalcTimeoutLeft <= 0)
        {
            RecalcPartsPostion();
            recalcTimeoutLeft = float.PositiveInfinity;
        }
        recalcTimeoutLeft -= Time.fixedDeltaTime;
    }
    public void RecalcPartsPostion()
    {
        for (int i = 0; i < cubeHolder.transform.childCount; i++)
        {
            var child = cubeHolder.transform.GetChild(i);
            child.localPosition = (cubeHolder.transform.childCount - i - 1) * Vector3.up;
        }
        cubeStackEffect.transform.localPosition = cubeHolder.transform.childCount * Vector3.up;
        Vector3 newStickmanPosition = cubeHolder.transform.childCount * Vector3.up;
        Vector3 stickmanPositionDelta = newStickmanPosition - stickman.transform.localPosition;
        if (stickmanPositionDelta.y < 0)
        {
            transform.position -= stickmanPositionDelta;
        }
        stickman.transform.localPosition = newStickmanPosition;
    }
    public void CollectPickup(Pickup pickup)
    {
        if (pickup.wasPicked)
        {
            return;
        }
        pickup.collider.isTrigger = false;
        pickup.transform.parent = cubeHolder.transform;
        RecalcPartsPostion();
        Instantiate(pickupTextSource, stickman.transform.position, pickupTextSource.transform.rotation);
        stickman.SetBool("Jump", true);
        cubeStackEffect.Play();
        pickup.wasPicked = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.TryGetComponent(out Wall wall))
        {
            return;
        }
        foreach (var contact in collision.contacts)
        {
            if (contact.thisCollider.TryGetComponent(out Pickup cube))
            {
                if (contact.normal.z >= 0)
                {
                    continue;
                }
                if (!fatalCollisionInvoked && cube.transform.parent.GetChild(0).gameObject == cube.gameObject)
                {
                    OnFatalCollision?.Invoke();
                    fatalCollisionInvoked = true;
                }
                cube.transform.parent = wall.transform.parent;
                cube.collider.isTrigger = false;
                recalcTimeoutLeft = wallThickness * 2 / forwardSpeed;
                vibrate = true;
                break;
            }
            else if (contact.thisCollider.TryGetComponent(out Ragdoll _))
            {
                if (!fatalCollisionInvoked)
                {
                    OnFatalCollision?.Invoke();
                    fatalCollisionInvoked = true;
                }
                break;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Pickup pickup))
        {
            CollectPickup(pickup);
        }
    }
}
