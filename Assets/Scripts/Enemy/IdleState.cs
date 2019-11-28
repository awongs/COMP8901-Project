using UnityEngine;

public class IdleState : FiniteState
{
    public IdleState(Enemy enemy) : base(enemy)
    {
    }

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
        // Do nothing.
    }
}
