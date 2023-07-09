using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour
{
    public float magnitude = 0.3f;
    private float shakeTimeout = 0;
    private Vector3 lastDelta = Vector3.zero;
    private void FixedUpdate()
    {
        if (shakeTimeout > 0)
        {
            float currentMagnitude = Random.Range(0, magnitude);
            Vector3 delta = Random.rotation * Vector3.forward * currentMagnitude;
            transform.position += delta - lastDelta;
            lastDelta = delta;
            shakeTimeout -= Time.fixedDeltaTime;
        }
    }
    public void Shake(float duration = 0.1f) {
        shakeTimeout = duration;
    }
}
