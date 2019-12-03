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

    // Array of nearby colliders for the physics overlap sphere.
    private Collider[] m_nearbyColliders;

    // Damage value for the gun.
    public int damage;

    private void Start()
    {
        m_nearbyColliders = new Collider[10];
    }

    public void Fire(Vector3 direction)
    {
        if (IsReady)
        {
            GameObject bulletInstance = Instantiate(bulletObj);
            bulletInstance.transform.position = transform.position;
            bulletInstance.transform.forward = direction;
            bool hitSomething = bulletInstance.GetComponent<Bullet>().Activate(transform.parent.GetComponent<Character>(), damage);

            // Record dodges if applicable.
            DodgePredictor dodgePredictor = transform.parent.GetComponent<DodgePredictor>();
            if (dodgePredictor != null && !hitSomething)
            {
                dodgePredictor.RecordDodge(direction);
            }

            // Set cooldown.
            m_currentCooldown = cooldown;
            
            /*
            // Only the player's gun shot makes noise.
            if (team == Character.Team.Player)
            {
                Physics.OverlapSphereNonAlloc(transform.position, 5.0f, m_nearbyColliders, ~LayerMask.GetMask("Level"));
                foreach (Collider c in m_nearbyColliders)
                {
                    if (c == null) { continue; }

                    Enemy enemy = c.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        int x = Mathf.RoundToInt(transform.position.x);
                        int y = Mathf.RoundToInt(Mathf.Abs(transform.position.z));
                        enemy.Alert(Level.TileAt(x, y));
                    }
                }
            }
            */
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
