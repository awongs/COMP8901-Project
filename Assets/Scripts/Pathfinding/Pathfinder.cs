using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public delegate float Heuristic(Tile current, Tile target);

    private class Node
    {
        public Tile tile;

        // A* pathfinding variables.
        public float f;
        public float g;
        public float h;

        public Node parent;

        public Node(Tile tile, Tile startTile, Tile endTile)
        {
            this.tile = tile;
            g = Mathf.Abs(tile.x - startTile.x) + Mathf.Abs(tile.y - startTile.y);
            h = Mathf.Abs(endTile.x - tile.x) + Mathf.Abs(endTile.y - tile.y);
            f = g + h;
        }

        public Node(Tile tile, Node parent, Tile startTile, Tile endTile)
        {
            this.tile = tile;
            this.parent = parent;

            g = parent.g + 1;
            h = Mathf.Abs(endTile.x - tile.x) + Mathf.Abs(endTile.y - tile.y);
            f = g + h;
        }
    }

    // The current path to follow.
    public List<Tile> currentPath;
    public int currentPathIndex;

    public List<Tile> CalculatePath(Tile targetTile)
    {
        // Initialize list and queue.
        PriorityQueue<Node> priorityQueue = new PriorityQueue<Node>((first, second) => first.h < second.h);
        List<Tile> closedTiles = new List<Tile>();

        // Put starting node into the queue.
        Tile startTile = Level.TileAt((int)Math.Abs(transform.position.x), (int)Math.Abs(transform.position.z));
        Node startNode = new Node(startTile, startTile, targetTile);
        priorityQueue.Enqueue(startNode, (int)startNode.f);
        closedTiles.Add(startTile);

        // Keep searching until the queue is empty.
        while (priorityQueue.Count > 0)
        {
            Node currentNode = priorityQueue.Dequeue();

            // Enqueue each applicable neighbour tile.
            foreach (Tile neighbour in currentNode.tile.neighbours.Values)
            {
                // Don't consider walls or tiles that have already been visited.
                if (neighbour.tileType == Tile.TileType.WALL || closedTiles.Contains(neighbour))
                {
                    continue;
                }

                // Put neighbour node into the queue.
                Node neighbourNode = new Node(neighbour, currentNode, startTile, targetTile);
                priorityQueue.Enqueue(neighbourNode, (int)neighbourNode.f);
                closedTiles.Add(neighbour);
            }
            
            // Check if the solution has been found.
            if (currentNode.tile.Equals(targetTile))
            {
                // Construct path.
                List<Tile> pathTiles = new List<Tile>();
                while (currentNode != null && currentNode != startNode)
                {
                    pathTiles.Add(currentNode.tile);
                    //currentNode.tile.GetComponent<Renderer>().material.color = Color.red;
                    currentNode = currentNode.parent;
                }

                // Reverse the list of tiles since we want a path from start to target.
                pathTiles.Reverse();
                //targetTile.GetComponent<Renderer>().material.color = Color.blue;
                return pathTiles;
            }
        }

        Debug.Log("Couldn't find a path for some reason.");
        return null;
    }
}
