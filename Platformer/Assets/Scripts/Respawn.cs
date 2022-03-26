using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HERE");
        if (other.gameObject.tag == "Player")
            other.gameObject.GetComponent<Player>().SetRespawnPoint(this.gameObject.transform);
    }
}
