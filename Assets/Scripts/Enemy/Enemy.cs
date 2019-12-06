using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public enum Aggressiveness
    {
        Low,
        Normal,
        High
    }

    // Goal definitions here.
    public const string STAY_ALIVE = "Stay Alive";
    public const string ATTACK_PLAYER = "Attack Player";

    // Reference to the pathfinder component.
    public Pathfinder pathfinder;

    // Reference to th rigid body component.
    public Rigidbody rigidBody;

    // How far this enemy can see.
    public float sightRange;

    // Array of visible colliders for the enemy agent.
    public Collider[] nearbyColliders = new Collider[10];

    // Reference to the player if they are in sight.
    // If this is null, it means that this enemy can't see the player.
    public Player sightedPlayer;

    // The aggressiveness level of this enemy agent.
    public Aggressiveness aggressiveness;

    // The tile that this enemy agent started on.
    public Tile spawnTile;

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
    public Action CurrentAction {
        get 
        {
            return m_currentAction;
        }
    }

    private void Awake()
    {
        // Randomized aggressiveness.
        // Colour changes depending on aggressiveness value.
        aggressiveness = (Enemy.Aggressiveness)UnityEngine.Random.Range(0, 3);

        switch (aggressiveness)
        {
            case Enemy.Aggressiveness.Low:
                GetComponent<Renderer>().material.color = Color.blue;
                break;
            case Enemy.Aggressiveness.Normal:
                GetComponent<Renderer>().material.color = Color.magenta;
                break;
            case Enemy.Aggressiveness.High:
                GetComponent<Renderer>().material.color = Color.red;
                break;
        }
    }

    private void Start()
    {
        m_currentState = new IdleState(this);
        
        // Initialize goals list.
        m_goals = new List<Goal>();
        m_goals.Add(new Goal(STAY_ALIVE, 0));
        m_goals.Add(new Goal(ATTACK_PLAYER, 0));

        // Initialize actions list.
        m_actions = new List<Action>();
        m_actions.Add(new ShootAction(this, Vector3.zero));
        m_actions.Add(new FleeAction(this, Vector3.zero));
        m_actions.Add(new RetreatAction(this));

        // Record spawning tile.
        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(Mathf.Abs(transform.position.z));
        spawnTile = Level.TileAt(x, y);
    }

    private void Update()
    {
        // Don't update if the game is too laggy (< 10 FPS)
        if (Time.deltaTime > 0.1f)
        {
            return;
        }

        // Get all nearby colliders, ignoring level layer.
        Array.Clear(nearbyColliders, 0, nearbyColliders.Length);
        Physics.OverlapSphereNonAlloc(transform.position, sightRange, nearbyColliders, ~LayerMask.GetMask("Level"));

        // Reset player reference.
        sightedPlayer = null;

        // Check if the player is within sight range.
        foreach (Collider collider in nearbyColliders)
        {
            if (collider == null) { continue; }

            Player player = collider.GetComponent<Player>();
            if (player != null)
            {
                Vector3 direction = player.transform.position - transform.position;

                // Flatten and normalize the direction.
                direction.y = 0f;
                direction = Vector3.Normalize(direction);

                // Check if we can actually see the target.
                Ray ray = new Ray(transform.position, direction);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform == player.transform)
                    {
                        sightedPlayer = player;
                    }
                }
            }
        }

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
        // Only chase the player if currently idle and not low aggressiveness.
        if (CurrentState is IdleState && aggressiveness != Enemy.Aggressiveness.Low)
        {
            CurrentState = new MoveState(this, pathfinder.CalculatePath(Level.RandomTileNear(sourceTile)));
        }
    }

    public void ChooseAction()
    {
        // Default action is to do nothing (idle).
        Action bestAction = new IdleAction(this);
        bestAction.CalculateDiscontentment();

        // Find the best action by comparing discontentment values.
        foreach (Action action in m_actions)
        {
            if (action.CheckPrecondition() && action.CalculateDiscontentment() < bestAction.discontentment)
            {
                bestAction = action;
            }
        }

        // Choose the best action.
        if (bestAction is ShootAction)
        {
            m_currentAction = new ShootAction(this, transform.forward);
        }
        else if (bestAction is FleeAction)
        {
            m_currentAction = new FleeAction(this, -transform.forward);
        }
        else if (bestAction is RetreatAction)
        {
            m_currentAction = new RetreatAction(this);
        }
    }

    public override void Die()
    {
        GameObject.Find("Level").GetComponent<Respawner>().RecordDeath(spawnTile, aggressiveness, m_currentState is IdleState);
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
