using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCubeText : MonoBehaviour
{
    public float destroyTimeout = 1f;
    private void Start()
    {
        Camera camera = Camera.main;
        if (camera != null)
        {
            transform.rotation = Quaternion.LookRotation(camera.transform.forward, transform.up);
        }
    }
    private void FixedUpdate()
    {
        if (destroyTimeout < 0)
        {
            Destroy(gameObject);
            return;
        }
        destroyTimeout -= Time.fixedDeltaTime;
    }
}
