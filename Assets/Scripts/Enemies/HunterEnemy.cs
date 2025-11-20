using UnityEngine;
using System.Collections.Generic;

public class HunterEnemy : Enemy
{
    private Pathfinding pathfinding;
    private List<Vector2Int> currentPath;
    private int pathIndex = 0;
    private float pathRecalculateTime = 0f;
    private const float PATH_RECALCULATE_INTERVAL = 0.4f;
    
    public override void Initialize(MazeData data, MazeRenderer renderer, Vector2Int startPos, Transform player)
    {
        base.Initialize(data, renderer, startPos, player);
        pathfinding = new Pathfinding(mazeData);
        updateInterval = 0.25f;
        moveSpeed = 4f;
    }
    
    protected override void DecideNextMove()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (playerTransform == null) return;
        }

        if (cachedPlayer == null)
        {
            cachedPlayer = playerTransform.GetComponent<PlayerController>();
            if (cachedPlayer == null) return;
        }
        
        Vector2Int playerGridPos = cachedPlayer.GetGridPosition();
        
        if (Time.time >= pathRecalculateTime)
        {
            pathRecalculateTime = Time.time + PATH_RECALCULATE_INTERVAL;
            
            currentPath = pathfinding.FindPath(currentGridPos, playerGridPos);
            pathIndex = 0;
        }
        
        if (currentPath != null && currentPath.Count > 1)
        {
            if (pathIndex < currentPath.Count - 1)
            {
                pathIndex++;
                TryMove(currentPath[pathIndex]);
            }
            else
            {
                currentPath = null;
            }
        }
    }
}