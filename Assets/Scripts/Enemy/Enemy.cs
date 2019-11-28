using UnityEngine;

public class Enemy : Character
{
    // Reference to the pathfinder component.
    public Pathfinder pathfinder;

    // Reference to th rigid body component.
    public Rigidbody rigidBody;

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

    // Array of visible colliders for this enemy agent.
    private Collider[] m_nearbyColliders;

    private void Start()
    {
        m_currentState = new IdleState(this);
        m_nearbyColliders = new Collider[10];
    }

    private void Update()
    {
        CurrentState.Run();

        // Get all nearby colliders, ignoring level layer.
        Physics.OverlapSphereNonAlloc(transform.position, 5.0f, m_nearbyColliders, ~LayerMask.GetMask("Level"));
        foreach (Collider c in m_nearbyColliders)
        {
            if (c == null) { continue; }

            Character character = c.GetComponent<Character>();
            if (character != null && character.team != team)
            {
                Vector3 direction = character.transform.position - transform.position;

                // Flatten and normalize the direction.
                direction.y = 0f;
                direction = Vector3.Normalize(direction);

                // Check if we can actually see the target.
                bool isTargetVisible = false;
                Ray ray = new Ray(transform.position, direction);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform == character.transform)
                    {
                        isTargetVisible = true;
                    }
                }

                if (isTargetVisible)
                {
                    gun.Fire(direction);
                }
            }
        }
    }

    public void Alert(Tile sourceTile)
    {
        if (CurrentState is IdleState)
        {
            pathfinder.currentPath = pathfinder.CalculatePath(Level.RandomTileNear(sourceTile));
            CurrentState = new MoveState(this);
        }
    }
}
