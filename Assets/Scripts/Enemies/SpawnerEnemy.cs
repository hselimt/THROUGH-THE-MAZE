using UnityEngine;
using System.Collections.Generic;

public class SpawnerEnemy : Enemy
{
    [Header("Spawning")]
    public GameObject hunterPrefab;
    public int maxMiniHunters = 3;
    public float spawnCooldown = 15f; 
    public float miniHunterLifetime = 12f; 

    private List<HunterEnemy> miniHunters = new List<HunterEnemy>();
    private List<float> miniHunterSpawnTimes = new List<float>();
    private float lastSpawnTime = 0f;
    
    
    public override void Initialize(MazeData data, MazeRenderer renderer, Vector2Int startPos, Transform player)
    {
        
        base.Initialize(data, renderer, startPos, null);
        moveSpeed = 0f;
        updateInterval = 1f;
    }
    
    protected override void DecideNextMove()
    {
        
        for (int i = miniHunters.Count - 1; i >= 0; i--)
        {
            if (miniHunters[i] == null)
            {
                miniHunters.RemoveAt(i);
                miniHunterSpawnTimes.RemoveAt(i);
            }
            else if (Time.time - miniHunterSpawnTimes[i] >= miniHunterLifetime)
            {
                
                if (miniHunters[i] != null && miniHunters[i].gameObject != null)
                {
                    
                    LevelManager levelManager = FindFirstObjectByType<LevelManager>();
                    if (levelManager != null)
                    {
                        levelManager.UnregisterEnemy(miniHunters[i]);
                    }

                    Destroy(miniHunters[i].gameObject);
                }
                miniHunters.RemoveAt(i);
                miniHunterSpawnTimes.RemoveAt(i);
            }
        }

        
        if (Time.time - lastSpawnTime >= spawnCooldown && miniHunters.Count < maxMiniHunters)
        {
            SpawnMiniHunter();
            lastSpawnTime = Time.time;
        }
    }
    
    private void SpawnMiniHunter()
    {
        Vector2Int spawnPos = FindEmptyAdjacentCell();

        if (spawnPos == new Vector2Int(-1, -1))
            return;

        Vector3 worldPos = mazeRenderer.GetWorldPosition(spawnPos.x, spawnPos.y);
        GameObject hunterObj = Instantiate(hunterPrefab, worldPos, Quaternion.identity);

        
        hunterObj.transform.localScale = Vector3.one * 0.7f;

        CircleCollider2D collider = hunterObj.GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = hunterObj.AddComponent<CircleCollider2D>();
        }
        collider.radius = 0.2f; 
        collider.isTrigger = true;
        hunterObj.tag = "Enemy";

        HunterEnemy hunter = hunterObj.GetComponent<HunterEnemy>();
        if (hunter != null)
        {
            Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            hunter.Initialize(mazeData, mazeRenderer, spawnPos, playerTransform);

            mazeData.GetCell(spawnPos.x, spawnPos.y).Content = CellContent.Enemy;
            miniHunters.Add(hunter);
            miniHunterSpawnTimes.Add(Time.time); 

            
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            if (levelManager != null)
            {
                levelManager.RegisterEnemy(hunter);
            }
        }
    }
    
    private Vector2Int FindEmptyAdjacentCell()
    {
        Vector2Int[] adjacentOffsets = {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0)
        };
        
        foreach (var offset in adjacentOffsets)
        {
            Vector2Int checkPos = currentGridPos + offset;
            
            if (mazeData.IsValidPosition(checkPos.x, checkPos.y))
            {
                MazeCell cell = mazeData.GetCell(checkPos.x, checkPos.y);
                if (cell.Content == CellContent.Empty)
                {
                    return checkPos;
                }
            }
        }
        
        return new Vector2Int(-1, -1);
    }

    
    private void OnDestroy()
    {
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();

        foreach (var hunter in miniHunters)
        {
            if (hunter != null && hunter.gameObject != null)
            {
                
                if (levelManager != null)
                {
                    levelManager.UnregisterEnemy(hunter);
                }

                Destroy(hunter.gameObject);
            }
        }
        miniHunters.Clear();
        miniHunterSpawnTimes.Clear();
    }
}