using System.Collections.Generic;
using UnityEngine;

public class FleeAction : Action
{
    // The amount of distance for fleeing.
    private const float FLEE_RADIUS = 4f;

    // The direction to flee towards.
    private Vector3 m_fleeDirection;

    // Reference to the move state for this action.
    private MoveState m_moveState;

    // Reference to the enemy agent's goal for staying alive.
    private Goal m_stayAliveGoal;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="enemy">Reference to the enemy.</param>
    /// <param name="fleeDirection">The direction to flee towards.</param>
    public FleeAction(Enemy enemy, Vector3 fleeDirection) : base(enemy)
    {
        m_fleeDirection = fleeDirection;
        m_stayAliveGoal = m_enemy.Goals.Find(item => item.name == Enemy.STAY_ALIVE);
    }

    public override bool CheckPrecondition()
    {
        // Don't run this action if the enemy character is already moving somewhere.
        return !(m_enemy.CurrentState is MoveState);
    }

    public override float CalculateDiscontentment()
    {
        discontentment = 0f;

        foreach (Goal goal in m_enemy.Goals)
        {
            // This action reduces the discontentment value of the stay alive goal.
            if (goal.name == Enemy.STAY_ALIVE)
            {
                discontentment += Mathf.Pow(Mathf.Max(goal.value - 0.1f, 0), 2);
            }
            else
            {
                discontentment += Mathf.Pow(goal.value, 2);
            }
        }

        return discontentment;
    }

    public override void Perform()
    {   
        if (m_moveState == null)
        {
            int fleeX = Mathf.RoundToInt(m_enemy.transform.position.x + m_fleeDirection.x * FLEE_RADIUS);
            int fleeY = Mathf.RoundToInt(Mathf.Abs(m_enemy.transform.position.z + m_fleeDirection.z * FLEE_RADIUS));

            // Clamp the values to ensure that they are valid tiles.
            fleeX = Mathf.Clamp(fleeX, 1, Level.MaxX - 1);
            fleeY = Mathf.Clamp(fleeY, 1, Level.MaxY - 1);

            // Calculate a path to flee with.
            List<Tile> fleePath = m_enemy.pathfinder.CalculatePath(Level.TileAt(fleeX, fleeY));

            // Path count of zero indicates that the enemy agent is already cornered.
            // Large path count indicates that the path is looping around a wall, and is unlikely to be running away from the player.
            // If the angle between the first tile in the path and the current forward is less than 90 degrees,
            // then that means the path is going towards the player.
            // Immediately end if any of these checks are true.
            if (fleePath.Count == 0 || fleePath.Count > FLEE_RADIUS * 2 || Vector3.Angle(fleePath[0].transform.position, m_enemy.transform.forward) < 90.0f)
            {
                // Reduce the goal value for this action.
                m_stayAliveGoal.value = Mathf.Max(m_stayAliveGoal.value - 5f, 0f);
                isDone = true;

                return;
            }

            // Flee towards the destination tile.
            m_moveState = new MoveState(m_enemy, fleePath);
            m_enemy.CurrentState = m_moveState;
        }
        else
        {
            // Check if done following the flee path.
            if (m_moveState.completedPath)
            {
                // Reduce the goal value after completing this action
                m_stayAliveGoal.value = Mathf.Max(m_stayAliveGoal.value - 5f, 0f);
                isDone = true;
            }
        }
    }
}
