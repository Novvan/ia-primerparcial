using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _bulletSpeed = 1f;
    
    public void BulletShot(Vector3 dir)
    {
        transform.position = dir * Time.deltaTime * _bulletSpeed;
    }
}
