using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    /// <summary>
    /// Represents an A* Node for pathfinding.
    /// </summary>
    private class Node
    {
        public Tile tile;

        // A* pathfinding variables.
        public float f;
        public float g;
        public float h;

        public Node parent;

        /// <summary>
        /// Constructor for creating a node without a parent.
        /// </summary>
        /// <param name="tile">The node's tile.</param>
        /// <param name="startTile">The starting tile in the path.</param>
        /// <param name="endTile">The destination tile in the path.</param>
        public Node(Tile tile, Tile startTile, Tile endTile)
        {
            this.tile = tile;
            g = Mathf.Abs(tile.x - startTile.x) + Mathf.Abs(tile.y - startTile.y);
            h = Mathf.Abs(endTile.x - tile.x) + Mathf.Abs(endTile.y - tile.y); // Manhattan
            f = g + h;
        }

        /// <summary>
        /// Constructor for creating a node that has a parent.
        /// Inherits the g value from the parent.
        /// </summary>
        /// <param name="tile">The node's tile.</param>
        /// <param name="parent">The parent of the node.</param>
        /// <param name="endTile">The destination tile in the path.</param>
        public Node(Tile tile, Node parent, Tile endTile)
        {
            this.tile = tile;
            this.parent = parent;

            g = parent.g + 1;
            h = Mathf.Abs(endTile.x - tile.x) + Mathf.Abs(endTile.y - tile.y); // Manhattan
            f = g + h;
        }

        /// <summary>
        /// Setter for assigning a new parent node.
        /// The g value of the new parent is inherited.
        /// </summary>
        /// <param name="newParent"></param>
        public void SetNewParent(Node newParent)
        {
            parent = newParent;
            g = newParent.g + 1;

            f = g + h;
        }
    }

    /// <summary>
    /// Calculates a path from the enemy agent's current tile to the target tile.
    /// </summary>
    /// <param name="targetTile">The destination tile.</param>
    /// <returns>A path to the destination tile.</returns>
    public List<Tile> CalculatePath(Tile targetTile)
    {
        // If the target tile in a wall, try to set the target tile to one of its neighbours.
        if (targetTile.tileType == Tile.TileType.WALL)
        {
            foreach (Tile tile in targetTile.neighbours.Values)
            {
                if (tile.tileType != Tile.TileType.WALL)
                {
                    targetTile = tile;
                }
            }

            // Couldn't find a neighbour that wasnt a wall, so abort.
            if (targetTile.tileType == Tile.TileType.WALL)
            {
                Debug.Log("Invalid target tile.");
                return null;
            }
        }

        // Initialize list and queue.
        PriorityQueue<Node> nodeQueue = new PriorityQueue<Node>((first, second) => first.h < second.h);
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();

        // Put starting node into the queue.
        Tile startTile = Level.TileAt(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(Math.Abs(transform.position.z)));
        Node startNode = new Node(startTile, startTile, targetTile);
        nodeQueue.Enqueue(startNode, (int)startNode.f);
        openNodes.Add(startNode);

        // Keep searching until the queue is empty.
        while (nodeQueue.Count > 0)
        {
            Node currentNode = nodeQueue.Dequeue();

            // Enqueue each applicable neighbour tile.
            foreach (Tile neighbour in currentNode.tile.neighbours.Values)
            {
                // Don't consider walls.
                if (neighbour.tileType == Tile.TileType.WALL)
                {
                    continue;
                }

                // Don't consider tiles that have already been visited.
                if (closedNodes.Find(n => n.tile == neighbour) != null)
                {
                    continue;
                }

                Node openNode = openNodes.Find(n => n.tile == neighbour);
                if (openNode != null)
                {
                    // Replace parent if the current node is closer.
                    if (currentNode.g + 1 < openNode.g)
                    {
                        openNode.SetNewParent(currentNode);

                        // Requeue the node.
                        nodeQueue.Remove(openNode);
                        nodeQueue.Enqueue(openNode, (int)openNode.f);
                    }
                }
                else
                {
                    // Put neighbour node into the queue.
                    Node neighbourNode = new Node(neighbour, currentNode, targetTile);
                    nodeQueue.Enqueue(neighbourNode, (int)neighbourNode.f);
                    openNodes.Add(neighbourNode);
                }
            }
            
            // Check if the solution has been found.
            if (currentNode.tile.Equals(targetTile))
            {
                // Construct path.
                List<Tile> pathTiles = new List<Tile>();
                while (currentNode != null && currentNode != startNode)
                {
                    pathTiles.Add(currentNode.tile);
                    currentNode = currentNode.parent;
                }

                // Reverse the list of tiles since we want a path from start to target.
                pathTiles.Reverse();
                return pathTiles;
            }

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);
        }

        Debug.Log("Couldn't find a path.");
        return null;
    }
}
