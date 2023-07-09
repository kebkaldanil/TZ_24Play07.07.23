using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public bool wasPicked = false;
    [NonSerialized]
    public new Collider collider;
    private void Start()
    {
        TryGetComponent(out collider);
    }
}
