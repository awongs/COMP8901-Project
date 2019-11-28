using UnityEngine;

public class MoveState : FiniteState
{
    private const float PI_DEG = 180.0f;

    public MoveState(Enemy enemy) : base(enemy)
    {
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
        Pathfinder pathfinder = m_enemy.pathfinder;

        // Is this character done following the path?
        if (pathfinder.currentPathIndex != pathfinder.currentPath.Count)
        {
            // Calculate direction of movement.
            Vector3 destination = pathfinder.currentPath[pathfinder.currentPathIndex].transform.position;
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
                pathfinder.currentPathIndex++;
            }
        }
        else
        {
            m_enemy.CurrentState = new IdleState(m_enemy);
        }
    }
}
