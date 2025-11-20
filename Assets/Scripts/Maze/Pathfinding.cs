using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private MazeData mazeData;
    
    
    private static Stack<Node> nodePool = new Stack<Node>(100);
    private static List<Node> activeNodes = new List<Node>(100);
    
    public Pathfinding(MazeData data)
    {
        mazeData = data;
    }
    
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        if (!mazeData.IsValidPosition(start.x, start.y) || 
            !mazeData.IsValidPosition(target.x, target.y))
            return null;
        
        List<Node> openList = new List<Node>(50); 
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        
        Node startNode = GetNode(start, null, 0, GetHeuristic(start, target));
        openList.Add(startNode);
        activeNodes.Add(startNode);
        
        int iterations = 0;
        const int MAX_ITERATIONS = 200; 
        
        while (openList.Count > 0 && iterations < MAX_ITERATIONS)
        {
            iterations++;
            
            Node current = GetLowestFCost(openList);
            openList.Remove(current);
            closedSet.Add(current.Position);
            
            if (current.Position == target)
            {
                List<Vector2Int> path = ReconstructPath(current);
                ReturnNodesToPool();
                return path;
            }
            
            List<Vector2Int> neighbors = GetWalkableNeighbors(current.Position);
            
            foreach (Vector2Int neighbor in neighbors)
            {
                if (closedSet.Contains(neighbor))
                    continue;
                
                float newGCost = current.GCost + 1;
                
                Node neighborNode = openList.Find(n => n.Position == neighbor);
                
                if (neighborNode == null)
                {
                    neighborNode = GetNode(neighbor, current, newGCost, GetHeuristic(neighbor, target));
                    openList.Add(neighborNode);
                    activeNodes.Add(neighborNode);
                }
                else if (newGCost < neighborNode.GCost)
                {
                    neighborNode.GCost = newGCost;
                    neighborNode.Parent = current;
                }
            }
        }
        
        ReturnNodesToPool();
        return null;
    }
    
    private Node GetNode(Vector2Int position, Node parent, float gCost, float hCost)
    {
        Node node;
        if (nodePool.Count > 0)
        {
            node = nodePool.Pop();
            node.Position = position;
            node.Parent = parent;
            node.GCost = gCost;
            node.HCost = hCost;
        }
        else
        {
            node = new Node(position, parent, gCost, hCost);
        }
        return node;
    }
    
    private void ReturnNodesToPool()
    {
        foreach (Node node in activeNodes)
        {
            node.Parent = null; 
            if (nodePool.Count < 100) 
                nodePool.Push(node);
        }
        activeNodes.Clear();
    }
    
    private List<Vector2Int> GetWalkableNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>(4); 
        
        Vector2Int[] directions = {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };
        
        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = pos + dir;
            
            if (mazeData.CanMoveTo(pos.x, pos.y, neighbor.x, neighbor.y))
            {
                neighbors.Add(neighbor);
            }
        }
        
        return neighbors;
    }
    
    private float GetHeuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
    
    private Node GetLowestFCost(List<Node> list)
    {
        Node lowest = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].FCost < lowest.FCost)
                lowest = list[i];
        }
        return lowest;
    }
    
    private List<Vector2Int> ReconstructPath(Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node current = endNode;
        
        while (current != null)
        {
            path.Add(current.Position);
            current = current.Parent;
        }
        
        path.Reverse();
        return path;
    }
    
    private class Node
    {
        public Vector2Int Position;
        public Node Parent;
        public float GCost;
        public float HCost;
        public float FCost => GCost + HCost;
        
        public Node(Vector2Int position, Node parent, float gCost, float hCost)
        {
            Position = position;
            Parent = parent;
            GCost = gCost;
            HCost = hCost;
        }
    }
}