using UnityEngine;

public class SpeedBoost : Collectible
{
    private SpriteRenderer sr;
    private float pulseTime = 0f;
    private float nextUpdateTime = 0f;
    private const float UPDATE_INTERVAL = 0.05f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        nextUpdateTime = Time.time + Random.Range(0f, UPDATE_INTERVAL);
    }

    private void Update()
    {
        
        if (Time.time < nextUpdateTime) return;
        nextUpdateTime = Time.time + UPDATE_INTERVAL;

        if (sr != null)
        {
            pulseTime += Time.deltaTime * 4f; 
            float t = Mathf.PingPong(pulseTime, 1f);
            
            sr.color = Color.Lerp(new Color(1f, 0.5f, 0f), new Color(1f, 0f, 0f), t);
        }
    }

    public override void Collect()
    {
        
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.ActivateSpeedBoost();
        }

        GameManager.Instance?.AddScore(25);
        mazeData.GetCell(gridPosition.x, gridPosition.y).Content = CellContent.Empty;
        Destroy(gameObject);
    }
}
