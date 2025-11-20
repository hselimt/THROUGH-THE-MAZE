using UnityEngine;

public class HealthPickup : TimedCollectible
{
    private SpriteRenderer sr;
    private float pulseTime = 0f;
    
    public override void Initialize(MazeData data, MazeRenderer renderer, Vector2Int pos)
    {
        base.Initialize(data, renderer, pos);
        mazeData.GetCell(pos.x, pos.y).Content = CellContent.Health;

        if (timerText != null)
            timerText.text = "❤️ " + Mathf.CeilToInt(timer);
        
        sr = GetComponent<SpriteRenderer>();
    }
    
    protected override void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused)
            return;
        
        
        if (sr != null)
        {
            pulseTime += Time.deltaTime * 2f;
            float t = Mathf.PingPong(pulseTime, 1f);
            sr.color = Color.Lerp(Color.yellow, Color.white, t);
        }
        
        timer -= Time.deltaTime;
        
        int secondsLeft = Mathf.CeilToInt(timer);
        timerText.text = "❤️ " + secondsLeft;
        
        if (timer <= 1f)
        {
            float blinkSpeed = 5f;
            bool visible = Mathf.PingPong(Time.time * blinkSpeed, 1f) > 0.5f;
            spriteRenderer.enabled = visible;
        }
        
        if (timer <= 0f)
        {
            Disappear();
        }
    }
    
    public override void Collect()
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.AddHealth(1);
            GameManager.Instance.AddScore(100);
        }
        
        mazeData.GetCell(gridPosition.x, gridPosition.y).Content = CellContent.Empty;
        Destroy(gameObject);
    }
}