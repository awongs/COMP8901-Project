public abstract class Action
{
    // The discontentment value for this action.
    public float discontentment;

    // Is the action done running?
    public bool isDone;

    // Reference to the enemy that this action belongs to.
    protected Enemy m_enemy;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="enemy">Reference to the enemy.</param>
    public Action(Enemy enemy)
    {
        m_enemy = enemy;
    }

    /// <summary>
    /// Checks if this action can be performed.
    /// </summary>
    /// <returns>True if the action can be performed, otherwise false.</returns>
    public abstract bool CheckPrecondition();

    /// <summary>
    /// Calculates the discontentment value of this action.
    /// </summary>
    /// <returns>The discontement value.</returns>
    public abstract float CalculateDiscontentment();

    /// <summary>
    /// Performs the action.
    /// </summary>
    public abstract void Perform();
}
