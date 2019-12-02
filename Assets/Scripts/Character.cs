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

    /// <summary>
    /// Retrieves the tile that the character is currently on.
    /// </summary>
    /// <returns>The tile that the character is currently on.</returns>
    public Tile GetTile()
    {
        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(Mathf.Abs(transform.position.z));

        return Level.TileAt(x, y);
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
