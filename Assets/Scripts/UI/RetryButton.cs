using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryButton : MonoBehaviour
{
    // Reference to the level loader.
    public TiledMapLoader mapLoader;

    // Reference to the player.
    public Player player;

    public void ReloadLevel()
    {
        // Reset the level.
        mapLoader.ClearLevel();
        mapLoader.LoadLevel();

        // Reset the player.
        player.transform.position = player.spawnPosition;
        player.health = player.maxHealth;
        player.gameObject.SetActive(true);

        // Hide the death interface.
        transform.parent.gameObject.SetActive(false);
    }
}
