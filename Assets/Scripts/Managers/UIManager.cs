using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("HUD")]
    public GameObject hudPanel;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreHUDText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI crystalsText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI comboText;
    public Button pauseButton;
    
    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI mainMenuHighScoreText;
    public Button playButton;
    public Button quitButton;
    
    [Header("Pause Menu")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button pauseMainMenuButton;
    
    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI newRecordText;
    public Button restartButton;
    public Button gameOverMainMenuButton;

    [Header("Floating Text")]
    public TextMeshProUGUI comboFloatingText;
    public TextMeshProUGUI speedBonusText;

    private int currentHighScore = 0;
    private Coroutine comboTextCoroutine;
    private Coroutine speedBonusCoroutine;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        SetupButtons();
        ShowMainMenu();

        if (titleText != null)
            StartCoroutine(AnimateTitle());

        if (comboFloatingText != null)
            comboFloatingText.gameObject.SetActive(false);
        if (speedBonusText != null)
            speedBonusText.gameObject.SetActive(false);
        if (newRecordText != null)
            newRecordText.gameObject.SetActive(false);
    }
    
    private void SetupButtons()
    {
        playButton?.onClick.AddListener(OnPlayButton);
        quitButton?.onClick.AddListener(OnQuitButton);
        resumeButton?.onClick.AddListener(OnResumeButton);
        pauseMainMenuButton?.onClick.AddListener(OnMainMenuButton);
        restartButton?.onClick.AddListener(OnRestartButton);
        gameOverMainMenuButton?.onClick.AddListener(OnMainMenuButton);
    }
    
    private IEnumerator AnimateTitle()
    {
        while (titleText != null && mainMenuPanel.activeSelf)
        {
            float scale = 1f + Mathf.Sin(Time.unscaledTime * 2f) * 0.1f;
            titleText.transform.localScale = Vector3.one * scale;
            yield return null;
        }
    }

    public void ShowMainMenu()
    {
        mainMenuPanel?.SetActive(true);
        pauseMenuPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        hudPanel?.SetActive(false);
        GameObject.Find("MenuStarfield")?.SetActive(true);
        GameObject.Find("BackgroundGrid")?.SetActive(false);

        Time.timeScale = 0f;

        if (titleText != null)
            StartCoroutine(AnimateTitle());

        UpdateMainMenuHighScore();
    }

    private void UpdateMainMenuHighScore()
    {
        if (mainMenuHighScoreText != null && SaveManager.Instance != null)
        {
            int highScore = SaveManager.Instance.GetHighScore();
            mainMenuHighScoreText.text = $"<color=#FFD700>HIGH SCORE -</color><size=140%><color=#00FFFF><b>{highScore:N0}</b></color></size>";
        }
    }
    
    public void OnPlayButton()
    {
        mainMenuPanel?.SetActive(false);
        hudPanel?.SetActive(true);

        GameObject.Find("MenuStarfield")?.SetActive(true);
        GameObject.Find("BackgroundGrid")?.SetActive(true);

        UpdateHighScoreHUD();

        Time.timeScale = 1f;
        GameManager.Instance?.StartGame();
    }
    
    public void OnQuitButton()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OnPauseButton()
    {
        GameManager.Instance?.SetPaused(true);
        ShowPauseMenu();
    }
    
    public void ShowPauseMenu()
    {
        pauseMenuPanel?.SetActive(true);
        hudPanel?.SetActive(false);
        Time.timeScale = 0f;
    }
    
    public void HidePauseMenu()
    {
        pauseMenuPanel?.SetActive(false);
        hudPanel?.SetActive(true);
        Time.timeScale = 1f;
    }
    
    public void OnResumeButton()
    {
        HidePauseMenu();
        GameManager.Instance?.SetPaused(false);
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 0f;

        pauseMenuPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        hudPanel?.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPaused(false);
        }

        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.ClearEnemies();
            levelManager.ClearCollectibles();
        }

        GameObject wallsParent = GameObject.Find("Walls");
        if (wallsParent != null)
            Destroy(wallsParent);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            Destroy(player);

        ShowMainMenu();
    }
    
    public void HideGameOver()
    {
        gameOverPanel?.SetActive(false);
    }
    
    public void OnRestartButton()
    {
        gameOverPanel?.SetActive(false);
        hudPanel?.SetActive(true);

        UpdateHighScoreHUD();

        Time.timeScale = 1f;
        GameManager.Instance?.RestartGame();
    }

    public void UpdateLevel(int level)
    {
        if (levelText != null)
        {
            if (level < 5)
                levelText.text = $"<color=#FFD700>LVL{level}</color>";
            else
                levelText.text = $"<color=#FFD700>LVLMAX</color>";
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
        }
    }

    public void UpdateHighScoreHUD()
    {
        if (highScoreHUDText != null && SaveManager.Instance != null)
        {
            int highScore = SaveManager.Instance.GetHighScore();
            highScoreHUDText.text = $"<color=#FFD700>HIGHEST</color>\n<size=120%><b>{highScore:N0}</b></size>";
        }
    }

    public void UpdateHealth(int health)
    {
        if (healthText != null)
        {
            string hearts = "<color=#FF0000>";
            for (int i = 0; i < health; i++)
                hearts += "♥";
            hearts += "</color>";
            healthText.text = $"{hearts.TrimEnd()}";

            if (health <= 1)
            {
                StartCoroutine(PulseDangerWarning());
            }
        }
    }

    private IEnumerator PulseDangerWarning()
    {
        if (healthText == null) yield break;

        float elapsed = 0f;
        while (elapsed < 2f)
        {
            float scale = 1f + Mathf.Sin(elapsed * 10f) * 0.15f;
            healthText.transform.localScale = Vector3.one * scale;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        healthText.transform.localScale = Vector3.one;
    }

    public void UpdateCrystals(int collected, int total)
    {
        if (crystalsText != null)
        {
            float progress = (float)collected / 9f;
            string color = progress < 0.33f ? "#FF4444" : progress < 0.66f ? "#FFFF44" : "#44FF44";
            crystalsText.text = $"<color={color}><size=140%><b>{collected}</b></size></color><color=#888888>/9</color>";
        }
    }

    public void UpdateCombo(int multiplier)
    {
        if (comboText != null)
        {
            if (multiplier > 1)
            {
                string color = multiplier >= 4 ? "#FF00FF" : multiplier >= 3 ? "#FF8800" : "#FFFF00";
                comboText.text = $"<color={color}><size=120%><b>×{multiplier}</b></size> COMBO!</color>";
                comboText.gameObject.SetActive(true);
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }

    public void ShowComboText(int multiplier)
    {
        if (comboFloatingText != null)
        {
            if (comboTextCoroutine != null)
                StopCoroutine(comboTextCoroutine);
        }
    }

    public void ShowSpeedBonus(string text)
    {
        if (speedBonusText != null)
        {
            if (speedBonusCoroutine != null)
                StopCoroutine(speedBonusCoroutine);
            speedBonusCoroutine = StartCoroutine(ShowFloatingText(speedBonusText, text, 2f));
        }
    }

    private IEnumerator ShowFloatingText(TextMeshProUGUI textObj, string message, float duration)
    {
        textObj.text = message;
        textObj.gameObject.SetActive(true);

        Color color = textObj.color;
        color.a = 0f;
        textObj.color = color;

        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / 0.3f);
            textObj.color = color;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(duration - 0.6f);

        elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / 0.3f);
            textObj.color = color;
            yield return null;
        }

        textObj.gameObject.SetActive(false);
    }

    public void ShowGameOver(int finalScore, int highScore, bool isNewRecord)
    {
        gameOverPanel?.SetActive(true);
        hudPanel?.SetActive(false);
        pauseMenuPanel?.SetActive(false);
        mainMenuPanel?.SetActive(false);

        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = $"<color=#FFD700>FINAL SCORE</color>\n<size=300%><color=#00FFFF><b>{finalScore:N0}</b></color></size>";
        }

        if (highScoreText != null)
        {
            string recordColor = isNewRecord ? "#00FF00" : "#FFD700";
            highScoreText.text = $"<color={recordColor}>HIGH SCORE- <b>{highScore:N0}</b></color>";
        }

        if (newRecordText != null)
        {
            newRecordText.gameObject.SetActive(isNewRecord);
            if (isNewRecord)
            {
                newRecordText.text = "<color=#FF00FF><b>NEW RECORD!</b></color> <color=#FFFF00><size=150%>★★★</size></color>";
                StartCoroutine(AnimateNewRecord());
            }
        }

        Time.timeScale = 0f;
    }

    private IEnumerator AnimateNewRecord()
    {
        if (newRecordText == null) yield break;

        while (newRecordText.gameObject.activeSelf)
        {
            float scale = 1f + Mathf.Sin(Time.unscaledTime * 5f) * 0.15f;
            newRecordText.transform.localScale = Vector3.one * scale;
            yield return null;
        }
    }

    private void Update()
    {
        if (timerText != null && GameManager.Instance != null)
        {
            float time = GameManager.Instance.CurrentLevelTime;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);

            string color = "#FFFFFF";
            if (time < 30f)
                color = "#00FF00";
            else if (time < 45f)
                color = "#FFFF00";
            else if (time < 60f)
                color = "#FFA500";
            else
                color = "#FF4444";

            timerText.text = $"<color=#888888>⏱</color> <color={color}><b>{minutes:00}:{seconds:00}</b></color>";
        }
    }
}