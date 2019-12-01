public abstract class Action
{
    // The discontentment value for this action.
    public float discontentment;

    // Is the action done running?
    public bool isDone;

    // Reference to the enemy that this action belongs to.
    protected Enemy m_enemy;

    public Action(Enemy enemy)
    {
        m_enemy = enemy;
    }

    public abstract float CalculateDiscontentment();
    public abstract void Perform();
}
