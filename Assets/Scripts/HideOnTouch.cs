using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnTouch : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            gameObject.SetActive(false);
        }
    }
}
