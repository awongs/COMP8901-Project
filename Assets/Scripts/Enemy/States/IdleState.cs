using System;
using UnityEngine;

public class IdleState : FiniteState
{
    public IdleState(Enemy enemy) : base(enemy) { }

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
        // Switch to combat state if the player is sighted while idle.
        if (m_enemy.sightedPlayer != null)
        {
            m_enemy.CurrentState = new CombatState(m_enemy, m_enemy.sightedPlayer);
        }
    }
}
