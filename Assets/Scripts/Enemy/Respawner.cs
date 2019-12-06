using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    // Enemy prefab.
    public GameObject enemyObj;

    // Class for storing enemy agent death information.
    public class DeathData
    {
        // The spawning position of the enemy agent.
        public Tile spawnTile;

        // How aggressive was the enemy agent that died?
        public Enemy.Aggressiveness aggressiveness;

        // Was the enemy idle when it died?
        public bool isIdle;

        public DeathData(Tile spawnTile, Enemy.Aggressiveness aggressiveness, bool isIdle)
        {
            this.spawnTile = spawnTile;
            this.aggressiveness = aggressiveness;
            this.isIdle = isIdle;
        }
    }

    // List of all recorded enemy agent death information.
    private List<DeathData> m_deathLog = new List<DeathData>();

    /// <summary>
    /// Records an enemy agent death.
    /// </summary>
    /// <param name="spawnTile">The tile that the enemy agent spawend on.</param>
    /// <param name="aggressiveness">The aggressiveness of the enemy agent.</param>
    /// <param name="isIdle">Whether or not the enemy agent was idle when it died.</param>
    public void RecordDeath(Tile spawnTile, Enemy.Aggressiveness aggressiveness, bool isIdle)
    {
        m_deathLog.Add(new DeathData(spawnTile, aggressiveness, isIdle));
        StartCoroutine(RespawnEnemy());
    }

    /// <summary>
    /// Respawns the enemy after a short delay.
    /// </summary>
    public IEnumerator RespawnEnemy()
    {
        // Wait a few seconds before respawning.
        yield return new WaitForSeconds(1f);

        // Instantiate the enemy agent.
        Enemy enemy = Instantiate(enemyObj, transform).GetComponent<Enemy>();

        // Loop until a tile is found that is either equally likely or less likely to have the agent die while idle.
        // Note: Very unoptimized. Could end up looping for a long time.
        Tile randomTile = Level.TileAt(Random.Range(1, 24), Random.Range(1, 24));
        while (!ClassifySpawnLocation(randomTile, enemy.aggressiveness) && randomTile.tileType != Tile.TileType.WALL) {
            randomTile = Level.TileAt(Random.Range(1, 24), Random.Range(1, 24));
            yield return null;
        }

        // Set position and spawn tile.
        enemy.transform.position = new Vector3(randomTile.x, enemyObj.transform.position.y, -randomTile.y);
        enemy.spawnTile = randomTile;
    }

    /// <summary>
    /// Checks if a given spawn tile and aggression level will more likely result in an idle death.
    /// </summary>
    /// <param name="spawnTile">The tile to spawn at.</param>
    /// <param name="aggressiveness">The aggressiveness of the enemy.</param>
    /// <returns>True if classifed as a likely idle death, otherwise false.</returns>
    public bool ClassifySpawnLocation(Tile spawnTile, Enemy.Aggressiveness aggressiveness)
    {
        // Counters for deaths that were in the idle state.
        int idleDeaths = 0;
        int lowAggroIdleDeaths = 0;
        int normalAggroIdleDeaths = 0;
        int highAggroIdleDeaths = 0;
        int spawnTileIdleDeaths = 0;

        // Counters for deaths that were not in the idle state.
        int nonIdleDeaths = 0;
        int lowAggroNonIdleDeaths = 0;
        int normalAggroNonIdleDeaths = 0;
        int highAggroNonIdleDeaths = 0;
        int spawnTileNonIdleDeaths = 0;

        // Gather death information.
        foreach (DeathData data in m_deathLog)
        {
            if (data.isIdle)
            {
                idleDeaths++;

                switch (data.aggressiveness)
                {
                    case Enemy.Aggressiveness.Low:
                        lowAggroIdleDeaths++;
                        break;
                    case Enemy.Aggressiveness.Normal:
                        normalAggroIdleDeaths++;
                        break;
                    case Enemy.Aggressiveness.High:
                        highAggroIdleDeaths++;
                        break;
                }

                if (data.spawnTile == spawnTile)
                {
                    spawnTileIdleDeaths++;
                }
            }
            else
            {
                nonIdleDeaths++;

                switch (data.aggressiveness)
                {
                    case Enemy.Aggressiveness.Low:
                        lowAggroNonIdleDeaths++;
                        break;
                    case Enemy.Aggressiveness.Normal:
                        normalAggroNonIdleDeaths++;
                        break;
                    case Enemy.Aggressiveness.High:
                        highAggroNonIdleDeaths++;
                        break;
                }

                if (data.spawnTile == spawnTile)
                {
                    spawnTileNonIdleDeaths++;
                }
            }
        }

        // Calculate the probability for dying in the idle state.
        float probabilityIdleDeath = (float)idleDeaths / m_deathLog.Count;

        // Calculate the probability for NOT dying in the idle state.
        float probabilityNonIdleDeath = (float)nonIdleDeaths / m_deathLog.Count;

        // Calculate the probability for the corresponding aggressiveness type to die in the idle state.
        float probabilityAggroIdleDeath = 0.0f;
        switch (aggressiveness)
        {
            case Enemy.Aggressiveness.Low:
                probabilityAggroIdleDeath = (float)lowAggroIdleDeaths / m_deathLog.Count;
                break;
            case Enemy.Aggressiveness.Normal:
                probabilityAggroIdleDeath = (float)normalAggroIdleDeaths / m_deathLog.Count;
                break;
            case Enemy.Aggressiveness.High:
                probabilityAggroIdleDeath = (float)highAggroIdleDeaths / m_deathLog.Count;
                break;
        }

        // Calculate the probability for the corresponding aggressiveness type to NOT die in the idle state.
        float probabilityNonAggroIdleDeath = 0.0f;
        switch (aggressiveness)
        {
            case Enemy.Aggressiveness.Low:
                probabilityNonAggroIdleDeath = (float)lowAggroNonIdleDeaths / m_deathLog.Count;
                break;
            case Enemy.Aggressiveness.Normal:
                probabilityNonAggroIdleDeath = (float)normalAggroNonIdleDeaths / m_deathLog.Count;
                break;
            case Enemy.Aggressiveness.High:
                probabilityNonAggroIdleDeath = (float)highAggroNonIdleDeaths / m_deathLog.Count;
                break;
        }

        // Calculate the probability that a enemy agent spawning on this exact tile will die in the idle state.
        float probabilitySpawnTileDeath = (float)spawnTileIdleDeaths / m_deathLog.Count;

        // Calculate the probability that a enemy agent spawning on this exact tile will NOT die in the idle state.
        float probabilityNonSpawnTileDeath = (float)spawnTileNonIdleDeaths / m_deathLog.Count;

        // Multiply the probabilities for naive bayes classification.
        float probabilityIdle = probabilityIdleDeath * probabilityAggroIdleDeath * probabilitySpawnTileDeath;
        float probabilityNonIdle = probabilityNonIdleDeath * probabilityNonAggroIdleDeath * probabilityNonSpawnTileDeath;
        
        // True means it is less likely that this spawn location and aggressiveness will result in an idle state death.
        return probabilityIdle <= probabilityNonIdle;
    }
}
