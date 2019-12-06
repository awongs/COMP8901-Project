public class IdleState : FiniteState
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="enemy">The enemy that owns this state.</param>
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
