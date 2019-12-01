using System.Collections.Generic;
using UnityEngine;

public class MoveState : FiniteState
{
    private const float PI_DEG = 180.0f;

    // The current path being taken.
    public List<Tile> currentPath;

    // The current index in the list of nodes for the path.
    public int currentPathIndex;

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
        // Check if the player is within range to shoot.
        foreach (Collider c in m_enemy.nearbyColliders)
        {
            if (c == null) { continue; }

            Character character = c.GetComponent<Character>();
            if (character != null && character.team != m_enemy.team)
            {
                Vector3 direction = character.transform.position - m_enemy.transform.position;

                // Flatten and normalize the direction.
                direction.y = 0f;
                direction = Vector3.Normalize(direction);

                // Check if we can actually see the target.
                bool isTargetVisible = false;
                Ray ray = new Ray(m_enemy.transform.position, direction);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform == character.transform)
                    {
                        isTargetVisible = true;
                    }
                }

                // If the target is visible, switch to combat state.
                if (isTargetVisible)
                {
                    m_enemy.CurrentState = new CombatState(m_enemy, character);
                }
            }
        }

        // Is this enemy character done following the path?
        if (currentPathIndex != currentPath.Count)
        {
            // Calculate direction of movement.
            Vector3 destination = currentPath[currentPathIndex].transform.position;
            destination.y = m_enemy.transform.position.y;
            Vector3 direction = destination - m_enemy.transform.position;
            //direction.y = 0f;
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
            m_enemy.CurrentState = new IdleState(m_enemy);
        }
    }
}
