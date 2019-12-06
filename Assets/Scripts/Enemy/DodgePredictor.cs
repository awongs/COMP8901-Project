using System;
using System.Collections.Generic;
using UnityEngine;

public class DodgePredictor : MonoBehaviour
{
    // The amount to rotate a firing direction by when predicting dodges.
    public const float DODGE_ANGLE = 10.0f;

    // The maximum amount of history to look at for predicting dodges.
    private const int MAX_HISTORY_COUNT = 20;

    public enum Dodge
    {
        Left,
        Right,
        None
    }

    // List of dodges that have been recorded recently.
    public static List<Dodge> recentHistory;

    // Reference to the player game object.
    public GameObject player;

    public void Start()
    {
        // Initialize player reference.
        player = GameObject.Find("Player");

        // Initialize data structure.
        recentHistory = new List<Dodge>();
    }
    
    /// <summary>
    /// Records a dodge done by the player.
    /// </summary>
    /// <param name="bulletDirection">The direction of the bullet that was dodged.</param>
    public void RecordDodge(Vector3 bulletDirection)
    {
        // Remove a stale record if we have reached the max history count
        // to make room for this next record.
        if (recentHistory.Count == MAX_HISTORY_COUNT)
        {
            recentHistory.RemoveAt(0);
        }

        Vector3 playerDirection = player.transform.position - transform.position;

        // Flatten and normalize the player direction.
        playerDirection.y = 0;
        playerDirection = Vector3.Normalize(playerDirection);

        Vector3 perp = Vector3.Cross(bulletDirection, playerDirection);
        float side = Vector3.Dot(perp, Vector3.up);

        // Dot product between the cross vector and up is the cosine of the angle.
        // If the value is zero, it means that the bullet and player vectors are equal,
        // which shouldn't be possible so it isn't handled.
        bool right = side > 0f;

        if (right)
        {
            recentHistory.Add(Dodge.Right);
        }
        else
        {
            recentHistory.Add(Dodge.Left);
        }
    }

    /// <summary>
    /// Predicts a dodge.
    /// </summary>
    /// <returns>A dodge prediction using nGrams.</returns>
    public Dodge PredictDodge()
    {
        // Not enough data recorded, so don't return a prediction.
        if (recentHistory.Count < 2)
        {
            return Dodge.None;
        }

        Tuple<Dodge, Dodge> nGram = new Tuple<Dodge, Dodge>(recentHistory[recentHistory.Count - 1], recentHistory[recentHistory.Count - 2]);

        // Initialize a dictionary to store dodge counts from nGram matches.
        Dictionary<Dodge, int> dodgeCount = new Dictionary<Dodge, int>();
        dodgeCount[Dodge.Left] = 0;
        dodgeCount[Dodge.Right] = 0;

        // Loop through recent history and get dodge counts.
        for (int i = 0; i < recentHistory.Count - 2; i++)
        {
            if (recentHistory[i] == nGram.Item1 && recentHistory[i + 1] == nGram.Item2)
            {
                // Increment count for this dodge.
                dodgeCount[recentHistory[i + 2]]++;

                i += 2;
            }
        }

        // Calculate probability that the player will dodge left or right.
        int totalDodgeCount = dodgeCount[Dodge.Left] + dodgeCount[Dodge.Right];
        float leftProbability = (float)dodgeCount[Dodge.Left] / totalDodgeCount;

        // Random value from 0 to leftProbability means predict left, otherwise predict right.
        if (UnityEngine.Random.Range(0f, 1f) < leftProbability)
        {
            return Dodge.Left;
        }
        else
        {
            return Dodge.Right;
        }
    }
}
