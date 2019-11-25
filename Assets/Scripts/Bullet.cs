using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Speed of the bullet;
    public float speed;

    // The character who fired this bullet's team.
    private Character.Team m_team;

    // Damage of this bullet.
    private int m_damage;

    // The bullet's rigid body component.
    private Rigidbody m_rigidBody;

    public void Activate(Character.Team team, int damage)
    {
        m_team = team;
        m_damage = damage;
        gameObject.SetActive(true);
    }

    private void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();

        // Limit the bullet's lifetime.
        Invoke("Die", 1f);
    }

    private void Update()
    {
        m_rigidBody.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Die();
            return;
        }

        Character otherCharacter = other.GetComponent<Character>();
        if (otherCharacter != null && otherCharacter.team != m_team)
        {
            otherCharacter.TakeDamage(m_damage);
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
