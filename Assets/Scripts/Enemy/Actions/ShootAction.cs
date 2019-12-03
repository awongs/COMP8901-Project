using UnityEngine;

public class ShootAction : Action
{
    // The amount of time to wait before actually firing the gun.
    private const float SHOOT_DELAY = 0.2f;

    // The direction to shoot.
    public Vector3 direction;

    // The amount of time that has elapsed since this action began.
    private float m_elapsedTime;

    // Reference to the enemy agent's goal for attacking the player.
    private Goal m_attackPlayerGoal;

    // Reference to the enemy agent's dodge predictor component.
    private DodgePredictor m_dodgePredictor;

    public ShootAction(Enemy enemy, Vector3 direction) : base(enemy)
    {
        this.direction = direction;
        m_attackPlayerGoal = m_enemy.Goals.Find(item => item.name == Enemy.ATTACK_PLAYER);
        m_dodgePredictor = m_enemy.GetComponent<DodgePredictor>();
    }

    public override bool CheckPrecondition()
    {
        return true;
    }

    public override float CalculateDiscontentment()
    {
        discontentment = 0f;

        foreach (Goal goal in m_enemy.Goals)
        {
            // This action reduces the discontentment value of the attack player goal.
            if (goal.name == Enemy.ATTACK_PLAYER)
            {
                discontentment += Mathf.Pow(Mathf.Max(goal.value - 1f, 0), 2);
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
        m_elapsedTime += Time.deltaTime;

        if (m_elapsedTime >= SHOOT_DELAY)
        {
            // Predict dodge if the player is currently moving.
            if (m_enemy.sightedPlayer != null && m_enemy.sightedPlayer.isMoving)
            {
                DodgePredictor.Dodge prediction = m_dodgePredictor.PredictDodge();
                if (prediction == DodgePredictor.Dodge.Left)
                {
                    // Rotate the firing direction slightly to the left to try and compensate for left dodge.
                    direction = Quaternion.Euler(0, -DodgePredictor.DODGE_ANGLE, 0) * direction;
                }
                else if (prediction == DodgePredictor.Dodge.Right)
                {
                    // Rotate the firing direction slightly to the right to try and compensate for right dodge.
                    direction = Quaternion.Euler(0, DodgePredictor.DODGE_ANGLE, 0) * direction;
                }
            }

            m_enemy.gun.Fire(direction);
            isDone = true;

            // Reduce the goal value after performing this action.
            m_attackPlayerGoal.value = Mathf.Max(m_attackPlayerGoal.value - 5f, 0f);
        }
    }
}
