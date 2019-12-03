using UnityEngine;

public class Bullet : MonoBehaviour
{
    /// <summary>
    /// Actives the hit scanner for the bullet.
    /// </summary>
    /// <param name="firingCharacter">The character who fired the gun.</param>
    /// <param name="damage">The amount of damage that the gun deals.</param>
    /// <returns>True if the bullet dealt damage to a character, otherwise false.</returns>
    public bool Activate(Character firingCharacter, int damage)
    {
        gameObject.SetActive(true);

        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3[] positions = { transform.position, hit.point };
            gameObject.GetComponent<LineRenderer>().SetPositions(positions);

            Character character = hit.transform.GetComponent<Character>();
            if (character != null && character.team != firingCharacter.team)
            {
                character.TakeDamage(damage);
                return true;
            }
        }

        return false;
    }

    private void Start()
    {
        // Limit the bullet's lifetime.
        Invoke("Die", 0.5f);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
