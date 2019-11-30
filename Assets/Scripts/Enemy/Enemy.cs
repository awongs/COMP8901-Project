using UnityEngine;

public class Enemy : Character
{
    // Reference to the pathfinder component.
    public Pathfinder pathfinder;

    // Reference to th rigid body component.
    public Rigidbody rigidBody;

    // How far this enemy can see.
    public float sightRange;

    // Current state of the enemy agent.
    private FiniteState m_currentState;
    public FiniteState CurrentState
    {
        get
        {
            return m_currentState;
        }
        set
        {
            // Exit the previous state.
            m_currentState.OnStateExit();

            // Set the new state.
            m_currentState = value;
            
            // Enter the new state.
            m_currentState.OnStateEnter();
        }
    }

    // Which way should the enemy agent be looking towards?
    private Vector3 m_targetDirection;

    private void Start()
    {
        m_currentState = new IdleState(this);
        
    }

    private void Update()
    {
        CurrentState.Run();
    }

    public void Alert(Tile sourceTile)
    {
        if (CurrentState is IdleState)
        {
            CurrentState = new MoveState(this, pathfinder.CalculatePath(Level.RandomTileNear(sourceTile)));
        }
    }
    
    public override void Die()
    {
        Destroy(gameObject);

        // Alert nearby enemies of death.
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, sightRange, ~LayerMask.GetMask("Level"));
        foreach (Collider c in nearbyColliders)
        {
            Enemy enemy = c.GetComponent<Enemy>();
            if (enemy != null)
            {
                Ray ray = new Ray(enemy.transform.position, transform.position - enemy.transform.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Make sure the other enemy is within sight.
                    if (hit.transform == transform && hit.distance <= enemy.sightRange)
                    {
                        int x = Mathf.RoundToInt(transform.position.x);
                        int y = Mathf.RoundToInt(Mathf.Abs(transform.position.z));
                        enemy.Alert(Level.TileAt(x, y));
                    }
                }
            }
        }
    }
}
