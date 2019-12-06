using System.Collections.Generic;
using UnityEngine;

public class MoveState : FiniteState
{
    private const float PI_DEG = 180.0f;

    // The current path being taken.
    public List<Tile> currentPath;

    // The current index in the list of nodes for the path.
    public int currentPathIndex;

    // Did the enemy complete the path?
    public bool completedPath;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="enemy">The enemy that owns this state.</param>
    /// <param name="path">The path to follow.</param>
    public MoveState(Enemy enemy, List<Tile> path) : base(enemy)
    {
        currentPath = path;
    }

    public override void OnStateEnter()
    {
        // Do nothing.
    }

    public override void OnStateExit()
    {
        // Do nothing.
    }

    public override void Run()
    {
        // Switch to combat state if the player is sighted while moving, and we are not fleeing.
        if (m_enemy.sightedPlayer != null && !(m_enemy.CurrentAction is FleeAction || m_enemy.CurrentAction is RetreatAction))
        {
            m_enemy.CurrentState = new CombatState(m_enemy, m_enemy.sightedPlayer);
        }

        // Is this enemy character done following the path?
        if (currentPathIndex < currentPath.Count)
        {
            // Calculate direction of movement.
            Vector3 destination = currentPath[currentPathIndex].transform.position;
            destination.y = m_enemy.transform.position.y;
            Vector3 direction = destination - m_enemy.transform.position;

            // Flatten and normalize the direction.
            direction.y = 0f;
            direction = Vector3.Normalize(direction);

            // Calculate new position and direction.
            Vector3 newPosition = m_enemy.transform.position + m_enemy.speed * direction * Time.deltaTime;
            Vector3 newDirection = destination - newPosition;
            m_enemy.rigidBody.MovePosition(newPosition);

            // We have passed the destination, set next index.
            if (Vector3.Angle(direction, newDirection) == PI_DEG)
            {
                currentPathIndex++;
            }
        }
        else
        {
            completedPath = true;
            m_enemy.CurrentState = new IdleState(m_enemy);
        }
    }
}
