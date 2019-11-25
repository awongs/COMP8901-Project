using UnityEngine;

public class Bullet : MonoBehaviour
{

    // The character who fired this bullet's team.
    private Character.Team m_team;

    // Damage of this bullet.
    private int m_damage;

    public void Activate(Character.Team team, int damage)
    {
        m_team = team;
        m_damage = damage;
        gameObject.SetActive(true);

        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3[] positions = { transform.position, hit.point };
            gameObject.GetComponent<LineRenderer>().SetPositions(positions);

            Character character = hit.transform.GetComponent<Character>();
            if (character != null && character.team != team)
            {
                character.TakeDamage(damage);
            }
        }
    }

    private void Start()
    {
        // Limit the bullet's lifetime.
        Invoke("Die", 1f);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
