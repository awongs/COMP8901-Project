using UnityEngine;

public class Enemy : Character
{
    private const float PI_DEG = 180.0f;

    enum State
    {
        Idle,
        Moving
    }

    // Reference to the pathfinder component.
    public Pathfinder pathfinder;

    // Reference to th rigid body component.
    public Rigidbody rigidBody;

    // Current state of the enemy agent.
    private State m_currentState;

    private void Start()
    {
        m_currentState = State.Idle;
    }

    private void Update()
    {
        switch(m_currentState)
        {
            case State.Idle:
                //pathfinder.currentPath = pathfinder.CalculatePath(Level.TileAt(1, 5));
                //m_currentState = State.Moving;
                Collider[] colliders = Physics.OverlapSphere(transform.position, 5.0f);
                foreach (Collider c in colliders)
                {
                    Character character = c.GetComponent<Character>();
                    if (character != null && character.team != team)
                    {
                        Vector3 direction = character.transform.position - transform.position;

                        // Flatten and normalize the direction.
                        direction.y = 0f;
                        direction = Vector3.Normalize(direction);

                        gun.Fire(direction);
                    }
                }
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
