using UnityEngine;

public class Enemy : Character
{
    private const float EPSILON = 0.05f;

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
                pathfinder.currentPath = pathfinder.CalculatePath(Level.TileAt(1, 5));
                m_currentState = State.Moving;
                break;
            case State.Moving:

                // Skip 
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
                    direction.y = 0f;
                    direction = Vector3.Normalize(direction);

                    rigidBody.MovePosition(transform.position + speed * direction * Time.deltaTime);

                    // Check if we have arrived to the destination.
                    // TODO: Implementation is very scuffed
                    if (Vector3.Distance(transform.position, destination) < EPSILON)
                    {
                        pathfinder.currentPathIndex++;
                    }
                }
                break;
        }
    }
}
