﻿using UnityEngine;

public class IdleAction : Action
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="enemy">Reference to the enemy.</param>
    public IdleAction(Enemy enemy) : base(enemy) { }

    public override float CalculateDiscontentment()
    {
        discontentment = 0f;

        foreach (Goal goal in m_enemy.Goals)
        {
            discontentment += Mathf.Pow(goal.value, 2);
        }

        return discontentment;
    }

    public override bool CheckPrecondition()
    {
        return true;
    }

    public override void Perform()
    {
        // Do nothing.
    }
}
