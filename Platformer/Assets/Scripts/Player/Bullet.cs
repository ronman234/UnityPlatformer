using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float bulletLife;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float damage;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(this.gameObject, bulletLife);
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.up * speed;
    }
}
