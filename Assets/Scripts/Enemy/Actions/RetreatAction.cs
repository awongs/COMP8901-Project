﻿using System.Collections.Generic;
using UnityEngine;

public class RetreatAction : Action
{
    // Reference to the move state for this action.
    private MoveState m_moveState;

    // Reference to the enemy agent's goal for staying alive.
    private Goal m_stayAliveGoal;

    // The tile that the enemy agent started from.
    private Tile m_initialTile;

    // The ally that has been chosen to alert.
    private Enemy m_ally;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="enemy">Reference to the enemy.</param>
    public RetreatAction(Enemy enemy) : base(enemy)
    {
        m_stayAliveGoal = m_enemy.Goals.Find(item => item.name == Enemy.STAY_ALIVE);
        m_initialTile = enemy.GetTile();
    }

    public override float CalculateDiscontentment()
    {
        discontentment = 0f;

        foreach (Goal goal in m_enemy.Goals)
        {
            // This action reduces the discontentment value of the stay alive goal.
            if (goal.name == Enemy.STAY_ALIVE)
            {
                discontentment += Mathf.Pow(Mathf.Max(goal.value - 1f, 0), 2);
            }
            else
            {
                discontentment += Mathf.Pow(goal.value, 2);
            }
        }

        return discontentment;
    }

    public override bool CheckPrecondition()
    {
        // Action can only be done if there are allies nearby.
        // The ally must have normal or high aggressiveness, and they must be idle.
        foreach (Collider collider in m_enemy.nearbyColliders)
        {
            if (collider == null) { continue; }

            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null && enemy != m_enemy && enemy.aggressiveness != Enemy.Aggressiveness.Low && enemy.CurrentState is IdleState)
            {
                // Check if the ally is actually visible.
                Ray ray = new Ray(m_enemy.transform.position, enemy.transform.position - m_enemy.transform.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override void Perform()
    {
        if (m_moveState == null)
        {
            // Find an ally that's within range.
            foreach (Collider collider in m_enemy.nearbyColliders)
            {
                if (collider == null) { continue; }

                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && enemy != m_enemy && enemy.aggressiveness != Enemy.Aggressiveness.Low && enemy.CurrentState is IdleState)
                {
                    // Check if the ally is actually visible.
                    Ray ray = new Ray(m_enemy.transform.position, enemy.transform.position - m_enemy.transform.position);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.transform == enemy.transform)
                        {
                            // Calculate a path to the ally.
                            m_ally = enemy;
                            List<Tile> retreatPath = m_enemy.pathfinder.CalculatePath(Level.RandomTileNear(enemy.GetTile()));

                            // Move towards the ally.
                            m_moveState = new MoveState(m_enemy, retreatPath);
                            m_enemy.CurrentState = m_moveState;
                        }
                    }
                }
            }

            // Couldn't find a path, so abort.
            if (m_moveState == null)
            {
                m_stayAliveGoal.value = Mathf.Max(m_stayAliveGoal.value - 1f, 0f);
                isDone = true;
            }
        }
        else
        {
            if (m_moveState.completedPath)
            {
                // Make sure the ally hasn't died before trying to alert it.
                if (m_ally != null)
                {
                    m_ally.Alert(m_initialTile);
                    m_enemy.CurrentState = new MoveState(m_enemy, m_enemy.pathfinder.CalculatePath(m_initialTile));
                }

                // Reduce the goal value after completing this action.
                m_stayAliveGoal.value = Mathf.Max(m_stayAliveGoal.value - 1f, 0f);
                isDone = true;
            }
        }
    }
}
