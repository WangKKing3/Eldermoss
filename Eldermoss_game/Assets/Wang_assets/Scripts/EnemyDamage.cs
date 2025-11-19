using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EnemyDamage : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public int damage;
 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerHealth.TakeDamage(damage);
        }
    }

}
