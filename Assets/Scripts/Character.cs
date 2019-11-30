using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum Team
    {
        Player,
        Enemy
    }

    // The character's team.
    public Team team;

    // Speed of the character's movement.
    public float speed;

    // Current health of the character.
    public int health;

    // The character's gun.
    public Gun gun;

    public void TakeDamage(int damage)
    {
        health -= damage;

        // Check death condition.
        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
