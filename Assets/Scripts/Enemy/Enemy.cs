using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    // Goal definitions here.
    public const string STAY_ALIVE = "Stay Alive";
    public const string KILL_PLAYER = "Kill Player";

    // Reference to the pathfinder component.
    public Pathfinder pathfinder;

    // Reference to th rigid body component.
    public Rigidbody rigidBody;

    // How far this enemy can see.
    public float sightRange;

    // Array of visible colliders for the enemy agent.
    public Collider[] nearbyColliders = new Collider[10];

    // Current state of the enemy agent.
    private FiniteState m_currentState;
    public FiniteState CurrentState
    {
        get
        {
            return m_currentState;
        }
        set
        {
            // Exit the previous state.
            m_currentState.OnStateExit();

            // Set the new state.
            m_currentState = value;
            
            // Enter the new state.
            m_currentState.OnStateEnter();
        }
    }

    // Which way should the enemy agent be looking towards?
    private Vector3 m_targetDirection;

    // List of goals for this enemy agent.
    private List<Goal> m_goals;
    public List<Goal> Goals {
        get {
            return m_goals;
        }
    }

    // List of actions that can be performed.
    private List<Action> m_actions;

    // The current action to perform.
    private Action m_currentAction;

    private void Start()
    {
        m_currentState = new IdleState(this);
        
        // Initialize goals list.
        m_goals = new List<Goal>();
        m_goals.Add(new Goal(STAY_ALIVE, 10));
        m_goals.Add(new Goal(KILL_PLAYER, 0));

        // Initialize actions list.
        m_actions = new List<Action>();
        m_actions.Add(new ShootAction(this));
    }

    private void Update()
    {
        // Get all nearby colliders, ignoring level layer.
        Array.Clear(nearbyColliders, 0, nearbyColliders.Length);
        Physics.OverlapSphereNonAlloc(transform.position, sightRange, nearbyColliders, ~LayerMask.GetMask("Level"));

        // Run the current state.
        CurrentState.Run();
        
        // Run the current action.
        if (m_currentAction != null && !m_currentAction.isDone)
        {
            m_currentAction.Perform();
        }
        else
        {
            ChooseAction();
        }
    }

    public void Alert(Tile sourceTile)
    {
        if (CurrentState is IdleState)
        {
            CurrentState = new MoveState(this, pathfinder.CalculatePath(Level.RandomTileNear(sourceTile)));
        }
    }

    public void ChooseAction()
    {
        // Start with an idle action.
        Action bestAction = new IdleAction(this);
        bestAction.CalculateDiscontentment();

        // Find the best action by comparing discontentment values.
        foreach (Action action in m_actions)
        {
            if (action.CalculateDiscontentment() < bestAction.discontentment)
            {
                bestAction = action;
            }
        }

        // Choose the best action.
        if (bestAction is ShootAction)
        {
            m_currentAction = new ShootAction(this);
        }
    }

    public override void Die()
    {
        Destroy(gameObject);

        // Alert nearby enemies of death.
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, sightRange, ~LayerMask.GetMask("Level"));
        foreach (Collider c in nearbyColliders)
        {
            Enemy enemy = c.GetComponent<Enemy>();
            if (enemy != null)
            {
                Ray ray = new Ray(enemy.transform.position, transform.position - enemy.transform.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Make sure the other enemy is within sight.
                    if (hit.transform == transform && hit.distance <= enemy.sightRange)
                    {
                        int x = Mathf.RoundToInt(transform.position.x);
                        int y = Mathf.RoundToInt(Mathf.Abs(transform.position.z));
                        enemy.Alert(Level.TileAt(x, y));
                    }
                }
            }
        }
    }
}
