using UnityEngine;

public abstract class FiniteState
{
    // Reference to the enemy that this state belongs to.
    protected Enemy m_enemy;

    public FiniteState(Enemy enemy)
    {
        m_enemy = enemy;
    }

    /// <summary>
    /// Called when this state is set on the enemy agent.
    /// </summary>
    public abstract void OnStateEnter();

    /// <summary>
    /// Runs the behaviours that are associated with this state.
    /// </summary>
    public abstract void Run();

    /// <summary>
    /// Called when this state is replaced on the enemy agent.
    /// </summary>
    public abstract void OnStateExit();
}
