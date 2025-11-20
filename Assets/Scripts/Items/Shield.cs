using UnityEngine;

public class Shield : Collectible
{
    private SpriteRenderer sr;
    private float pulseTime = 0f;
    
    public override void Initialize(MazeData data, MazeRenderer renderer, Vector2Int pos)
    {
        base.Initialize(data, renderer, pos);
        mazeData.GetCell(pos.x, pos.y).Content = CellContent.Shield;
        sr = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        if (sr != null)
        {
            pulseTime += Time.deltaTime * 2f;
            float t = Mathf.PingPong(pulseTime, 1f);
            sr.color = Color.Lerp(Color.yellow, Color.white, t);
        }
    }
    
    public override void Collect()
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.ActivateShield();
            GameManager.Instance.AddScore(20);
        }
        
        mazeData.GetCell(gridPosition.x, gridPosition.y).Content = CellContent.Empty;
        Destroy(gameObject);
    }
}