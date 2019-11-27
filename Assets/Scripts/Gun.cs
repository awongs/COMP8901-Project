using UnityEngine;

public class Gun : MonoBehaviour
{
    // Bullet prefab to instantiate.
    public GameObject bulletObj;

    // Minimum interval between gunshots.
    public float cooldown;
    private float m_currentCooldown;

    // Is the gun ready to fire?
    private bool IsReady {
        get {
            return m_currentCooldown <= 0.0f;
        }
    }

    // Damage value for the gun.
    public int damage;

    public void Fire(Vector3 direction)
    {
        if (IsReady)
        {
            Character.Team team = transform.parent.GetComponent<Character>().team;

            GameObject bulletInstance = Instantiate(bulletObj);
            bulletInstance.transform.position = transform.position;
            bulletInstance.transform.forward = direction;
            bulletInstance.GetComponent<Bullet>().Activate(team, damage);

            // Set cooldown.
            m_currentCooldown = cooldown;
        }
    }

    private void Update()
    {
        if (m_currentCooldown > 0.0f)
        {
            m_currentCooldown -= Time.deltaTime;
        }
    }
}
