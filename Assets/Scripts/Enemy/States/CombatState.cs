using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatState : FiniteState
{
    // The targeted character.
    private Character m_target;

    // Reference to the enemy agent's goal for killing the player.
    private Goal m_attackPlayerGoal;

    // Reference to the enemy agent's goal for staying alive.
    private Goal m_stayAliveGoal;

    public CombatState(Enemy enemy, Character target) : base(enemy)
    {
        m_target = target;
        m_attackPlayerGoal = m_enemy.Goals.Find(item => item.name == Enemy.ATTACK_PLAYER);
        m_stayAliveGoal = m_enemy.Goals.Find(item => item.name == Enemy.STAY_ALIVE);
    }

    public override void OnStateEnter()
    {
        // Do nothing.
    }

    public override void OnStateExit()
    {
        // Reset goal values when exiting.
        m_attackPlayerGoal.value = 0f;
        //m_stayAliveGoal.value = 0f;
    }

    public override void Run()
    {
        Vector3 direction = m_target.transform.position - m_enemy.transform.position;

        // Flatten and normalize the direction.
        direction.y = 0f;
        direction = Vector3.Normalize(direction);

        m_enemy.transform.forward = direction;

        // Increase goal values while in this state, depending on aggressiveness levels.
        switch (m_enemy.aggressiveness)
        {
            case Enemy.Aggressiveness.Low:
                m_attackPlayerGoal.value += 3 * Time.deltaTime;
                m_stayAliveGoal.value += 5 * Time.deltaTime;
                break;
            case Enemy.Aggressiveness.Normal:
                m_attackPlayerGoal.value += 10 * Time.deltaTime;
                m_stayAliveGoal.value += 3 * Time.deltaTime;
                break;
            case Enemy.Aggressiveness.High:
                m_attackPlayerGoal.value += 10 * Time.deltaTime;
                break;
        }

        // Check if we can still see the target.
        Ray ray = new Ray(m_enemy.transform.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform != m_target.transform)
            {
                // More aggressive enemies will chase the player.
                if (m_enemy.aggressiveness == Enemy.Aggressiveness.High || m_enemy.aggressiveness == Enemy.Aggressiveness.Normal)
                {
                    // Run towards where the target was.
                    int x = Mathf.RoundToInt(m_target.transform.position.x);
                    int y = Mathf.RoundToInt(Mathf.Abs(m_target.transform.position.z));
                    m_enemy.CurrentState = new MoveState(m_enemy, m_enemy.pathfinder.CalculatePath(Level.RandomTileNear(Level.TileAt(x, y))));
                }
                else
                {
                    m_enemy.CurrentState = new IdleState(m_enemy);
                }
            }
        }
    }
}
