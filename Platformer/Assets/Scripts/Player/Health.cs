using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{    
    private int currentHealth;
    [SerializeField]
    private int maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth <= 0)
            Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            currentHealth -= collision.gameObject.GetComponent<Bullet>().GetDamage();
            Destroy(collision.gameObject);
        }
    }
}
