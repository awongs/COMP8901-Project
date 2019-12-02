﻿using System;
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

        public void SetNewParent(Node newParent)
        {
            parent = newParent;
            g = newParent.g + 1;

            f = g + h;
        }
    }

    public List<Tile> CalculatePath(Tile targetTile)
    {
        if (targetTile.tileType == Tile.TileType.WALL)
        {
            //Debug.LogWarning("Tried to find a path that ends at a wall.");

            foreach (Tile tile in targetTile.neighbours.Values)
            {
                if (tile.tileType != Tile.TileType.WALL)
                {
                    targetTile = tile;
                }
            }
            //return null;
        }

        // Initialize list and queue.
        PriorityQueue<Node> nodeQueue = new PriorityQueue<Node>((first, second) => first.h < second.h);
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();

        // Put starting node into the queue.
        Tile startTile = Level.TileAt(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(Math.Abs(transform.position.z)));
        Node startNode = new Node(startTile, startTile, targetTile);
        nodeQueue.Enqueue(startNode, (int)startNode.f);
        closedNodes.Add(startNode);

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
                    Node neighbourNode = new Node(neighbour, currentNode, startTile, targetTile);
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
                    //currentNode.tile.GetComponent<Renderer>().material.color = Color.red;
                    currentNode = currentNode.parent;
                }

                // Reverse the list of tiles since we want a path from start to target.
                pathTiles.Reverse();
                //targetTile.GetComponent<Renderer>().material.color = Color.blue;
                return pathTiles;
            }

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);
        }

        Debug.Log("Couldn't find a path for some reason.");
        return null;
    }
}
