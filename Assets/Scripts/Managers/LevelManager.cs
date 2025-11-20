using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject hunterEnemyPrefab;
    public GameObject patrolEnemyPrefab;
    public GameObject teleporterEnemyPrefab;
    public GameObject spawnerEnemyPrefab;
    public GameObject crystalPrefab;
    public GameObject healthPickupPrefab;
    public GameObject shieldPrefab;
    public GameObject ghostPowerUpPrefab;
    public GameObject speedBoostPrefab;
    public GameObject freezeBombPrefab;
    
    [Header("References")]
    public MazeRenderer mazeRenderer;
    
    private MazeData currentMazeData;
    private GameObject playerInstance;
    private int currentLevel = 1;
    private const int MAX_DIFFICULTY_LEVEL = 5;

    private List<Enemy> activeEnemies = new List<Enemy>();
    private List<Collectible> activeCollectibles = new List<Collectible>();

    private float collectibleSpawnTimer = 0f;
    private const float COLLECTIBLE_SPAWN_INTERVAL = 12f;
    
    public void StartLevel(int level)
    {
        currentLevel = level;
        
        ClearAllImmediate();

        MazeGenerator generator = new MazeGenerator(0);
        currentMazeData = generator.Generate();
        mazeRenderer.RenderMaze(currentMazeData);
        
        SpawnPlayer();
        SpawnEnemiesForLevel(level);
        SpawnCrystals();
        SpawnPickups();
        
        GameManager.Instance?.SetTotalCrystals(20);
    }

    private void ClearAllImmediate()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null && enemy.gameObject != null)
                DestroyImmediate(enemy.gameObject);
        }
        activeEnemies.Clear();
        
        foreach (var collectible in activeCollectibles)
        {
            if (collectible != null && collectible.gameObject != null)
                DestroyImmediate(collectible.gameObject);
        }
        activeCollectibles.Clear();
        
        if (playerInstance != null)
        {
            Destroy(playerInstance);
            playerInstance = null;
        }

        if (currentMazeData != null)
        {
            for (int x = 0; x < MazeData.MAZE_WIDTH; x++)
            {
                for (int y = 0; y < MazeData.MAZE_HEIGHT; y++)
                {
                    MazeCell cell = currentMazeData.GetCell(x, y);
                    if (cell != null)
                    {
                        cell.Content = CellContent.Empty;
                    }
                }
            }
        }
    }
    
    public void NextLevel()
    {
        currentLevel++;
        StartLevel(currentLevel);
    }

    private void Update()
    {
        if (currentLevel >= MAX_DIFFICULTY_LEVEL)
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsGamePaused && !GameManager.Instance.IsGameOver)
            {
                collectibleSpawnTimer += Time.deltaTime;

                if (collectibleSpawnTimer >= COLLECTIBLE_SPAWN_INTERVAL)
                {
                    collectibleSpawnTimer = 0f;
                    SpawnRandomCollectible();
                }
            }
        }
    }

    private void SpawnRandomCollectible()
    {
        int choice = Random.Range(0, 5);

        Vector2Int pos = GetRandomEmptyPosition();
        Vector3 worldPos = mazeRenderer.GetWorldPosition(pos.x, pos.y);
        GameObject spawnedObj = null;
        Collectible collectible = null;

        switch (choice)
        {
            case 0:
                spawnedObj = Instantiate(crystalPrefab, worldPos, Quaternion.identity);
                spawnedObj.transform.localScale = Vector3.one * 0.6f;
                collectible = spawnedObj.GetComponent<Crystal>();
                if (collectible != null)
                {
                    collectible.Initialize(currentMazeData, mazeRenderer, pos);
                    GameManager.Instance?.IncreaseTotalCrystals();
                }
                break;

            case 1:
                spawnedObj = Instantiate(healthPickupPrefab, worldPos, Quaternion.identity);
                spawnedObj.transform.localScale = Vector3.one * 0.6f;
                collectible = spawnedObj.GetComponent<HealthPickup>();
                if (collectible != null)
                    collectible.Initialize(currentMazeData, mazeRenderer, pos);
                break;

            case 2:
                spawnedObj = Instantiate(shieldPrefab, worldPos, Quaternion.identity);
                spawnedObj.transform.localScale = Vector3.one * 0.6f;
                collectible = spawnedObj.GetComponent<Shield>();
                if (collectible != null)
                    collectible.Initialize(currentMazeData, mazeRenderer, pos);
                break;

            case 3:
                spawnedObj = Instantiate(speedBoostPrefab, worldPos, Quaternion.identity);
                spawnedObj.transform.localScale = Vector3.one * 0.6f;
                collectible = spawnedObj.GetComponent<SpeedBoost>();
                if (collectible != null)
                    collectible.Initialize(currentMazeData, mazeRenderer, pos);
                break;

            case 4:
                spawnedObj = Instantiate(freezeBombPrefab, worldPos, Quaternion.identity);
                spawnedObj.transform.localScale = Vector3.one * 0.6f;
                collectible = spawnedObj.GetComponent<FreezeBomb>();
                if (collectible != null)
                    collectible.Initialize(currentMazeData, mazeRenderer, pos);
                break;
        }

        if (collectible != null)
        {
            activeCollectibles.Add(collectible);
        }
    }

    private void SpawnPlayer()
    {
        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }
        
        Vector2Int startPos = new Vector2Int(0, 0);
        Vector3 worldPos = mazeRenderer.GetWorldPosition(startPos.x, startPos.y);
        playerInstance = Instantiate(playerPrefab, worldPos, Quaternion.identity);

        Rigidbody2D rb = playerInstance.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = playerInstance.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        CircleCollider2D collider = playerInstance.GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = playerInstance.AddComponent<CircleCollider2D>();
        }
        collider.radius = 0.2f;
        collider.isTrigger = true;

        playerInstance.transform.localScale = Vector3.one * 0.7f;

        playerInstance.tag = "Player";
        
        PlayerController controller = playerInstance.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.Initialize(currentMazeData, startPos, this);
            currentMazeData.GetCell(startPos.x, startPos.y).Content = CellContent.Player;
        }
    }
    
    private void SpawnEnemiesForLevel(int level)
    {
        int hunters = 0;
        int patrols = 0;
        int teleporters = 0;
        int spawners = 0;
        
        if (level == 1)
        {
            hunters = 1;
            patrols = 1;
        }
        else if (level == 2)
        {
            hunters = 2;
            patrols = 1;
        }
        else if (level == 3)
        {
            hunters = 2;
            patrols = 1;
            teleporters = 1;
        }
        else if (level == 4)
        {
            hunters = 2;
            patrols = 1;
            teleporters = 1;
            spawners = 1;
        }
        else
        {
            hunters = 1;
            patrols = 1;
            teleporters = 2;
            spawners = 2;
        }

        for (int i = 0; i < hunters; i++)
        {
            SpawnEnemy(hunterEnemyPrefab);
        }

        for (int i = 0; i < patrols; i++)
        {
            SpawnEnemy(patrolEnemyPrefab);
        }

        for (int i = 0; i < teleporters; i++)
        {
            SpawnEnemy(teleporterEnemyPrefab);
        }

        for (int i = 0; i < spawners; i++)
        {
            SpawnEnemy(spawnerEnemyPrefab);
        }
    }
    
    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null) return;

        bool isHunter = (enemyPrefab == hunterEnemyPrefab);
        Vector2Int spawnPos = isHunter ? GetRandomEmptyPositionFarFromPlayer(6f) : GetRandomEmptyPosition();
        Vector3 worldPos = mazeRenderer.GetWorldPosition(spawnPos.x, spawnPos.y);
        
        GameObject enemyObj = Instantiate(enemyPrefab, worldPos, Quaternion.identity);
        enemyObj.tag = "Enemy";

        Rigidbody2D rb = enemyObj.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = enemyObj.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        CircleCollider2D collider = enemyObj.GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = enemyObj.AddComponent<CircleCollider2D>();
        }
        collider.radius = 0.2f;
        collider.isTrigger = true;

        enemyObj.transform.localScale = Vector3.one * 0.7f;

        if (playerInstance == null)
        {
            playerInstance = GameObject.FindGameObjectWithTag("Player");
        }

        Transform playerTransform = playerInstance != null ? playerInstance.transform : null;

        Enemy enemyComponent = null;

        HunterEnemy hunter = enemyObj.GetComponent<HunterEnemy>();
        if (hunter != null)
        {
            hunter.Initialize(currentMazeData, mazeRenderer, spawnPos, playerTransform);
            enemyComponent = hunter;
        }

        PatrolEnemy patrol = enemyObj.GetComponent<PatrolEnemy>();
        if (patrol != null)
        {
            patrol.Initialize(currentMazeData, mazeRenderer, spawnPos, playerTransform);
            enemyComponent = patrol;
        }

        TeleporterEnemy teleporter = enemyObj.GetComponent<TeleporterEnemy>();
        if (teleporter != null)
        {
            teleporter.Initialize(currentMazeData, mazeRenderer, spawnPos, playerTransform);
            enemyComponent = teleporter;
        }

        SpawnerEnemy spawner = enemyObj.GetComponent<SpawnerEnemy>();
        if (spawner != null)
        {
            spawner.hunterPrefab = hunterEnemyPrefab;
            spawner.Initialize(currentMazeData, mazeRenderer, spawnPos, playerTransform);
            enemyComponent = spawner;
        }
        
        if (enemyComponent != null)
        {
            activeEnemies.Add(enemyComponent);
        }
    }

    private void SpawnCrystals()
    {
        int crystalCount = 20;

        for (int i = 0; i < crystalCount; i++)
        {
            Vector2Int pos = GetRandomEmptyPosition();
            Vector3 worldPos = mazeRenderer.GetWorldPosition(pos.x, pos.y);

            GameObject crystalObj = Instantiate(crystalPrefab, worldPos, Quaternion.identity);

            crystalObj.transform.localScale = Vector3.one * 0.6f;

            Crystal crystal = crystalObj.GetComponent<Crystal>();

            if (crystal != null)
            {
                crystal.Initialize(currentMazeData, mazeRenderer, pos);
                activeCollectibles.Add(crystal);
                currentMazeData.GetCell(pos.x, pos.y).Content = CellContent.Crystal;
            }
        }
    }
    
    private void SpawnPickups()
    {
        int healthCount = Random.Range(2, 4);
        for (int i = 0; i < healthCount; i++)
        {
            Vector2Int pos = GetRandomEmptyPosition();
            Vector3 worldPos = mazeRenderer.GetWorldPosition(pos.x, pos.y);

            GameObject healthObj = Instantiate(healthPickupPrefab, worldPos, Quaternion.identity);
            healthObj.transform.localScale = Vector3.one * 0.6f;
            HealthPickup health = healthObj.GetComponent<HealthPickup>();

            if (health != null)
            {
                health.Initialize(currentMazeData, mazeRenderer, pos);
                activeCollectibles.Add(health);
                currentMazeData.GetCell(pos.x, pos.y).Content = CellContent.Empty;
            }
        }

        Vector2Int shieldPos = GetRandomEmptyPosition();
        Vector3 shieldWorldPos = mazeRenderer.GetWorldPosition(shieldPos.x, shieldPos.y);

        GameObject shieldObj = Instantiate(shieldPrefab, shieldWorldPos, Quaternion.identity);
        shieldObj.transform.localScale = Vector3.one * 0.6f;
        Shield shield = shieldObj.GetComponent<Shield>();

        if (shield != null)
        {
            shield.Initialize(currentMazeData, mazeRenderer, shieldPos);
            activeCollectibles.Add(shield);
            currentMazeData.GetCell(shieldPos.x, shieldPos.y).Content = CellContent.Empty;
        }

        if (speedBoostPrefab != null)
        {
            Vector2Int speedPos = GetRandomEmptyPosition();
            Vector3 speedWorldPos = mazeRenderer.GetWorldPosition(speedPos.x, speedPos.y);
            GameObject speedObj = Instantiate(speedBoostPrefab, speedWorldPos, Quaternion.identity);
            speedObj.transform.localScale = Vector3.one * 0.6f;

            SpriteRenderer speedSr = speedObj.GetComponent<SpriteRenderer>();
            if (speedSr == null)
            {
                speedSr = speedObj.AddComponent<SpriteRenderer>();
                speedSr.sprite = CreateSimpleSprite();
                speedSr.color = new Color(1f, 0.5f, 0f, 0.8f);
            }

            SpeedBoost speed = speedObj.GetComponent<SpeedBoost>();
            if (speed == null)
                speed = speedObj.AddComponent<SpeedBoost>();

            speed.Initialize(currentMazeData, mazeRenderer, speedPos);
            activeCollectibles.Add(speed);
            currentMazeData.GetCell(speedPos.x, speedPos.y).Content = CellContent.Empty;
        }

        if (freezeBombPrefab != null)
        {
            Vector2Int freezePos = GetRandomEmptyPosition();
            Vector3 freezeWorldPos = mazeRenderer.GetWorldPosition(freezePos.x, freezePos.y);
            GameObject freezeObj = Instantiate(freezeBombPrefab, freezeWorldPos, Quaternion.identity);
            freezeObj.transform.localScale = Vector3.one * 0.6f;

            SpriteRenderer freezeSr = freezeObj.GetComponent<SpriteRenderer>();
            if (freezeSr == null)
            {
                freezeSr = freezeObj.AddComponent<SpriteRenderer>();
                freezeSr.sprite = CreateSimpleSprite();
                freezeSr.color = new Color(0.3f, 0.5f, 1f, 0.8f);
            }

            FreezeBomb freeze = freezeObj.GetComponent<FreezeBomb>();
            if (freeze == null)
                freeze = freezeObj.AddComponent<FreezeBomb>();

            freeze.Initialize(currentMazeData, mazeRenderer, freezePos);
            activeCollectibles.Add(freeze);
            currentMazeData.GetCell(freezePos.x, freezePos.y).Content = CellContent.Empty;
        }
    }

    public void SpawnCrystalsAndPickups()
    {
        SpawnCrystals();
        SpawnPickups();
        
        GameManager.Instance?.SetTotalCrystals(20);
    }
    
    public Vector2Int GetRandomEmptyPosition()
    {
        int maxAttempts = 100;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            int x = Random.Range(0, MazeData.MAZE_WIDTH);
            int y = Random.Range(0, MazeData.MAZE_HEIGHT);

            MazeCell cell = currentMazeData.GetCell(x, y);

            if (cell != null && cell.Content == CellContent.Empty)
            {
                return new Vector2Int(x, y);
            }

            attempts++;
        }

        return new Vector2Int(1, 1);
    }

    public Vector2Int GetRandomEmptyPositionFarFromPlayer(float minDistance = 5f)
    {
        int maxAttempts = 100;
        int attempts = 0;
        Vector2Int playerPos = new Vector2Int(0, 0);

        while (attempts < maxAttempts)
        {
            int x = Random.Range(0, MazeData.MAZE_WIDTH);
            int y = Random.Range(0, MazeData.MAZE_HEIGHT);

            MazeCell cell = currentMazeData.GetCell(x, y);

            if (cell != null && cell.Content == CellContent.Empty)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(playerPos.x, playerPos.y));
                if (distance >= minDistance)
                {
                    return new Vector2Int(x, y);
                }
            }

            attempts++;
        }

        return GetRandomEmptyPosition();
    }

    private Sprite CreateSimpleSprite()
    {
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= radius)
                {
                    pixels[y * size + x] = Color.white;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
    
    public void ClearEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null && enemy.gameObject != null)
                Destroy(enemy.gameObject);
        }
        activeEnemies.Clear();
        
        if (currentMazeData != null)
        {
            for (int x = 0; x < MazeData.MAZE_WIDTH; x++)
            {
                for (int y = 0; y < MazeData.MAZE_HEIGHT; y++)
                {
                    MazeCell cell = currentMazeData.GetCell(x, y);
                    if (cell != null && (cell.Content == CellContent.Enemy || cell.Content == CellContent.PatrolEnemy))
                    {
                        cell.Content = CellContent.Empty;
                    }
                }
            }
        }
    }
    
    public void ClearCollectibles()
    {
        foreach (var collectible in activeCollectibles)
        {
            if (collectible != null && collectible.gameObject != null)
                Destroy(collectible.gameObject);
        }
        activeCollectibles.Clear();
        
        if (currentMazeData != null)
        {
            for (int x = 0; x < MazeData.MAZE_WIDTH; x++)
            {
                for (int y = 0; y < MazeData.MAZE_HEIGHT; y++)
                {
                    MazeCell cell = currentMazeData.GetCell(x, y);
                    if (cell != null && cell.Content == CellContent.Crystal)
                    {
                        cell.Content = CellContent.Empty;
                    }
                }
            }
        }
    }
    
    public GameObject GetPlayer()
    {
        return playerInstance;
    }
    
    public MazeData GetCurrentMazeData()
    {
        return currentMazeData;
    }
    
    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy != null && !activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public bool IsMaxDifficulty()
    {
        return currentLevel >= MAX_DIFFICULTY_LEVEL;
    }

    public Collectible[] GetActiveCollectibles()
    {
        activeCollectibles.RemoveAll(c => c == null);
        return activeCollectibles.ToArray();
    }

    public Enemy[] GetActiveEnemies()
    {
        activeEnemies.RemoveAll(e => e == null);
        return activeEnemies.ToArray();
    }
}