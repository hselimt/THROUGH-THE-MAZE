using UnityEngine;

public class FreezeBomb : Collectible
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
            pulseTime += Time.deltaTime * 2.5f;
            float t = Mathf.PingPong(pulseTime, 1f);
            
            sr.color = Color.Lerp(new Color(0.3f, 0.3f, 1f), new Color(0.7f, 0.9f, 1f), t);
        }
    }

    public override void Collect()
    {
        
        GameManager.Instance?.FreezeAllEnemies();

        GameManager.Instance?.AddScore(40);
        mazeData.GetCell(gridPosition.x, gridPosition.y).Content = CellContent.Empty;
        Destroy(gameObject);
    }
}
