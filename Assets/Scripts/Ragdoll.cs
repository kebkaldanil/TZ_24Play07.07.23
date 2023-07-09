using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField]
    private bool isRagdoll = false;
    public bool IsRagdoll
    {
        get => isRagdoll;
        set
        {
            if (isRagdoll == value)
            {
                return;
            }
            if (value)
            {
                RagdollEnable();
            }
            else
            {
                RagdollDisable();
            }
        }
    }
    private void Start()
    {
        if (isRagdoll)
        {
            RagdollEnable();
        }
        else
        {
            RagdollDisable();
        }
    }
    public void RagdollEnable()
    {
        foreach (var collider in GetComponents<Collider>())
        {
            collider.enabled = false;
        }
        foreach (var rigitbody in transform.GetComponentsInChildren<Rigidbody>(true))
        {
            rigitbody.isKinematic = false;
            rigitbody.GetComponent<Collider>().enabled = true;
        }
        foreach (var animator in transform.GetComponentsInChildren<Animator>(true))
        {
            animator.enabled = false;
        }
        isRagdoll = true;
    }
    public void RagdollDisable()
    {
        foreach (var collider in GetComponents<Collider>())
        {
            collider.enabled = true;
        }
        foreach (var rigitbody in transform.GetComponentsInChildren<Rigidbody>(true))
        {
            rigitbody.isKinematic = true;
            rigitbody.GetComponent<Collider>().enabled = false;
        }
        foreach (var animator in transform.GetComponentsInChildren<Animator>(true))
        {
            animator.enabled = true;
        }
        isRagdoll = false;
    }
}
