using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatState : FiniteState
{
    // The targetted character.
    private Character m_target;

    public CombatState(Enemy enemy, Character target) : base(enemy)
    {
        m_target = target;
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
        Vector3 direction = m_target.transform.position - m_enemy.transform.position;

        // Flatten and normalize the direction.
        direction.y = 0f;
        direction = Vector3.Normalize(direction);

        m_enemy.gun.Fire(direction);

        // Check if we can still see the target.
        Ray ray = new Ray(m_enemy.transform.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform != m_target.transform)
            {
                // Run towards where the target was.
                int x = Mathf.RoundToInt(m_target.transform.position.x);
                int y = Mathf.RoundToInt(Mathf.Abs(m_target.transform.position.z));
                m_enemy.CurrentState = new MoveState(m_enemy, m_enemy.pathfinder.CalculatePath(Level.RandomTileNear(Level.TileAt(x, y))));
            }
        }
    }
}
