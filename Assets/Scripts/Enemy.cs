using UnityEngine;

public class Enemy : Character
{
    private const float PI_DEG = 180.0f;

    enum State
    {
        Idle,
        Moving,
        Alert,
        Combat
    }

    // Reference to the pathfinder component.
    public Pathfinder pathfinder;

    // Reference to th rigid body component.
    public Rigidbody rigidBody;

    // Current state of the enemy agent.
    private State m_currentState;

    // Which way should the enemy agent be looking towards?
    private Vector3 m_targetDirection;

    // Array of visible colliders for this enemy agent.
    private Collider[] m_visibleColliders;

    private void Start()
    {
        m_currentState = State.Idle;
        m_visibleColliders = new Collider[10];
    }

    private void Update()
    {
        HandleStates();

        // Get all nearby colliders, ignoring level layer.
        Physics.OverlapSphereNonAlloc(transform.position, 5.0f, m_visibleColliders, ~LayerMask.GetMask("Level"));
        foreach (Collider c in m_visibleColliders)
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

    private void HandleStates()
    {
        switch (m_currentState)
        {
            case State.Idle:
                pathfinder.currentPath = pathfinder.CalculatePath(Level.TileAt(1, 5));
                m_currentState = State.Moving;
                break;
            case State.Moving:

                // Skip lag frames.
                if (Time.deltaTime > 0.1f)
                {
                    return;
                }

                // Is this character done following the path?
                if (pathfinder.currentPathIndex != pathfinder.currentPath.Count)
                {
                    // Calculate direction of movement.
                    Vector3 destination = pathfinder.currentPath[pathfinder.currentPathIndex].transform.position;
                    destination.y = transform.position.y;
                    Vector3 direction = destination - transform.position;
                    //direction.y = 0f;
                    direction = Vector3.Normalize(direction);

                    // Calculate new position and direction.
                    Vector3 newPosition = transform.position + speed * direction * Time.deltaTime;
                    Vector3 newDirection = destination - newPosition;
                    rigidBody.MovePosition(newPosition);

                    // We have passed the destination, set next index.
                    if (Vector3.Angle(direction, newDirection) == PI_DEG)
                    {
                        pathfinder.currentPathIndex++;
                    }
                }
                break;
        }
    }
}
