using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Player player;
    public new Camera camera;
    public RectTransform gameOverOverlay;
    public ParticleSystem warpEffect;
    public List<Track> trackVariants;
    [Range(0f, 1000f)]
    public float spawnDistance = 10f;
    public Vector3 nextTrackPositon = new(0, 0, 0);
    public Vector3 trackPositonOffset = new(0, 0, 30);
    [Range(0f, 1000f)]
    public float destroyOffset = 50f;
    private Track lastTrack;
    public bool GameStarted { get; private set; }
    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
        if (camera == null)
        {
            camera = Camera.main;
        }
        if (warpEffect == null)
        {
            GameObject.Find("WarpEffect").TryGetComponent(out warpEffect);
        }
        if (gameOverOverlay == null)
        {
            GameObject.Find("GameOverOverlay").TryGetComponent(out gameOverOverlay);
        }
        player.OnFatalCollision += GameOver;
        player.enabled = false;
        gameOverOverlay.gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        while (lastTrack == null || lastTrack.transform.position.z - player.transform.position.z <= spawnDistance)
        {
            PlaceNextTrack();
        }
        if (!GameStarted)
        {
            if (Input.touchCount > 0 || Input.GetMouseButton(0))
            {
                GameStarted = true;
                GameStart();
            }
        }
    }
    public void PlaceNextTrack()
    {
        if (trackVariants.Count == 0)
        {
            return;
        }
        var trackVariant = trackVariants[Random.Range(0, trackVariants.Count)];
        lastTrack = trackVariant.Copy(player, nextTrackPositon, nextTrackPositon + new Vector3(0, 0, destroyOffset));
        nextTrackPositon += trackPositonOffset;
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
    public void GameStart()
    {
        player.enabled = true;
        warpEffect.Play();
    }
    public void GameOver()
    {
        Ragdoll ragdoll = player.stickman.GetComponent<Ragdoll>();
        ragdoll.IsRagdoll = true;
        foreach (var rigitbody in ragdoll.GetComponentsInChildren<Rigidbody>())
        {
            rigitbody.velocity = player.forwardSpeed * Vector3.forward;
        }
        foreach (var pickup in FindObjectsOfType<Pickup>())
        {
            pickup.collider.isTrigger = false;
        }
        player.rigidbody.isKinematic = true;
        player.enabled = false;
        warpEffect.Stop();
        gameOverOverlay.gameObject.SetActive(true);
    }
}
