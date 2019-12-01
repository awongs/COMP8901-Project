using UnityEngine;

public class ShootAction : Action
{
    // The direction to shoot.
    public Vector3 direction;

    public ShootAction(Enemy enemy) : base(enemy)
    {
    }

    public override float CalculateDiscontentment()
    {
        discontentment = 0f;

        foreach (Goal goal in m_enemy.Goals)
        {
            // This action reduces the discontentment value of the attack player goal.
            if (goal.name == Enemy.KILL_PLAYER)
            {
                discontentment += Mathf.Pow(Mathf.Max(goal.value - 5f, 0), 2);
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
        m_enemy.gun.Fire(m_enemy.transform.forward);
        isDone = true;
    }
}
