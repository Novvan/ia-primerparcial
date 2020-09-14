using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public Transform spawnPoint;
    private void Start()
    {
        transform.position = spawnPoint.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            //var respawn = new Vector3(spawnPoint.position.x,spawnPoint.position.y,spawnPoint.position.z);
            gameObject.transform.position = new Vector3(0,100,0 );
        }
    }
}