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
    }
}
