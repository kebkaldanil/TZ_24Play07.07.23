using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Track : MonoBehaviour
{
    public Player player;
    public Vector3 destroyPoint;
    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
    }
    public Track Copy(Player player, Vector3 position, Vector3 destroyPoint)
    {
        var result = Instantiate(this, position, transform.rotation);
        result.player = player;
        result.destroyPoint = destroyPoint;
        return result;
    }

    private void FixedUpdate()
    {
        if (player.transform.position.z >= destroyPoint.z)
        {
            Destroy(gameObject);
        }
    }
}
