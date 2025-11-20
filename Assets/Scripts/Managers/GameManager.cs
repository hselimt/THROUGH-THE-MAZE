using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("References")]
    public LevelManager levelManager;

    private PlayerHealth playerHealth;
    private int crystalsCollected = 0;
    private int totalCrystals = 0;
    private int currentScore = 0;
    private bool isGameOver = false;
    private bool isGamePaused = false;

    private bool isDamageCooldown = false;

    private int comboCount = 0;
    private float lastCrystalTime = 0f;
    private const float COMBO_WINDOW = 2f;
    private int comboMultiplier = 1;

    private int highScore = 0;

    private float levelStartTime = 0f;
    private float currentLevelTime = 0f;

    public bool IsGamePaused => isGamePaused;
    public bool IsGameOver => isGameOver;
    public int ComboMultiplier => comboMultiplier;
    public float CurrentLevelTime => currentLevelTime;
    public int HighScore => highScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Application.targetFrameRate = 60;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SaveManager.Instance != null)
        {
            highScore = SaveManager.Instance.GetHighScore();
        }
    }

    private void Update()
    {
        if (isGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            if (isGamePaused)
            {
                UIManager.Instance?.OnResumeButton();
            }
            else
            {
                SetPaused(true);
                UIManager.Instance?.ShowPauseMenu();
            }
        }

        if (!isGamePaused)
        {
            currentLevelTime = Time.time - levelStartTime;
        }

        if (comboCount > 0 && Time.time - lastCrystalTime > COMBO_WINDOW)
        {
            ResetCombo();
        }
    }
    
    public void StartGame()
    {
        isGameOver = false;
        isGamePaused = false;
        isDamageCooldown = false;
        crystalsCollected = 0;
        currentScore = 0;

        ResetCombo();
        levelStartTime = Time.time;
        currentLevelTime = 0f;

        levelManager.StartLevel(1);

        GameObject player = levelManager.GetPlayer();
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerHealth.Initialize(1);
        }

        UIManager.Instance?.UpdateLevel(1);
        UIManager.Instance?.UpdateHealth(1);
        UIManager.Instance?.UpdateScore(0);
        UIManager.Instance?.UpdateCrystals(0, 9);
        UIManager.Instance?.UpdateCombo(0);
        UIManager.Instance?.HideGameOver();
    }
    
    public void SetPaused(bool paused)
    {
        isGamePaused = paused;
        Time.timeScale = paused ? 0f : 1f;
    }
    
    public void CollectCrystal(Crystal crystal)
    {
        crystalsCollected++;

        float timeSinceLastCrystal = Time.time - lastCrystalTime;
        if (timeSinceLastCrystal <= COMBO_WINDOW && comboCount > 0)
        {
            comboCount++;
            comboMultiplier = 1 + (comboCount / 2);
        }
        else
        {
            comboCount = 1;
            comboMultiplier = 1;
        }
        lastCrystalTime = Time.time;

        int crystalPoints = 10 * comboMultiplier;
        currentScore += crystalPoints;

        UIManager.Instance?.UpdateCrystals(crystalsCollected, totalCrystals);
        UIManager.Instance?.UpdateScore(currentScore);
        UIManager.Instance?.UpdateCombo(comboMultiplier);

        if (comboMultiplier > 1)
        {
            UIManager.Instance?.ShowComboText(comboMultiplier);
        }

        if (crystalsCollected >= 9)
        {
            LevelComplete();
        }
    }

    private void ResetCombo()
    {
        comboCount = 0;
        comboMultiplier = 1;
        UIManager.Instance?.UpdateCombo(0);
    }
    
    public void SetTotalCrystals(int total)
    {
        totalCrystals = total;
        crystalsCollected = 0;
        UIManager.Instance?.UpdateCrystals(crystalsCollected, totalCrystals);
    }

    public void IncreaseTotalCrystals()
    {
        totalCrystals++;
        UIManager.Instance?.UpdateCrystals(crystalsCollected, totalCrystals);
    }

    public void CheckPlayerCollision(Vector2Int gridPos)
    {
        if (levelManager == null) return;

        Collectible[] collectibles = levelManager.GetActiveCollectibles();

        foreach (Collectible collectible in collectibles)
        {
            if (collectible.GetGridPosition() == gridPos)
            {
                collectible.Collect();
            }
        }
    }

    public void PlayerHitByEnemy()
    {
        if (isDamageCooldown) return;

        isDamageCooldown = true;

        if (playerHealth != null)
        {
            playerHealth.TakeDamage();
        }

        StartCoroutine(ResetDamageCooldownCoroutine());
    }

    private System.Collections.IEnumerator ResetDamageCooldownCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        isDamageCooldown = false;
    }
    
    public void RespawnPlayer()
    {
        GameObject player = levelManager.GetPlayer();
        if (player != null)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.TeleportTo(new Vector2Int(0, 0));
        }
    }
    
    public void GameOver()
    {
        isGameOver = true;
        SetPaused(true);

        bool isNewHighScore = false;
        if (SaveManager.Instance != null)
        {
            isNewHighScore = SaveManager.Instance.UpdateHighScore(currentScore);
            if (isNewHighScore)
            {
                highScore = currentScore;
                UIManager.Instance?.UpdateHighScoreHUD();
            }
        }

        UIManager.Instance?.ShowGameOver(currentScore, highScore, isNewHighScore);
    }
    
    public void RestartGame()
    {
        SetPaused(false);
        isDamageCooldown = false;
        levelManager.ClearEnemies();
        levelManager.ClearCollectibles();
        StartGame();
    }
    
    private void LevelComplete()
    {
        int currentLevel = levelManager.GetCurrentLevel();

        int levelBonus = 100;

        int timeBonus = 0;
        if (currentLevelTime < 30f)
        {
            timeBonus = 100;
        }
        else if (currentLevelTime < 45f)
        {
            timeBonus = 50;
        }
        else if (currentLevelTime < 60f)
        {
            timeBonus = 25;
        }

        currentScore += levelBonus + timeBonus;
        UIManager.Instance?.UpdateScore(currentScore);
        crystalsCollected = 0;

        ResetCombo();

        if (currentLevel < 5)
        {
            StartCoroutine(StartNextLevelCoroutine());
        }
        else
        {
            StartCoroutine(RespawnCrystalsOnlyCoroutine());
        }
    }

    private System.Collections.IEnumerator StartNextLevelCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        levelStartTime = Time.time;
        currentLevelTime = 0f;

        levelManager.NextLevel();

        GameObject player = levelManager.GetPlayer();
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerHealth.Initialize(1);
        }

        int newLevel = levelManager.GetCurrentLevel();
        UIManager.Instance?.UpdateLevel(newLevel);
        UIManager.Instance?.UpdateHealth(1);
    }

    private System.Collections.IEnumerator RespawnCrystalsOnlyCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        levelStartTime = Time.time;
        currentLevelTime = 0f;

        levelManager.ClearCollectibles();
        levelManager.SpawnCrystalsAndPickups();

        int currentLevel = levelManager.GetCurrentLevel();
        UIManager.Instance?.UpdateLevel(currentLevel);
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        UIManager.Instance?.UpdateScore(currentScore);
    }

    public void FreezeAllEnemies()
    {
        if (levelManager != null)
        {
            Enemy[] enemies = levelManager.GetActiveEnemies();
            foreach (Enemy enemy in enemies)
            {
                if (enemy != null)
                {
                    enemy.Freeze(3f); 
                }
            }
        }
    }
}