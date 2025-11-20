using UnityEngine;

public class Crystal : Collectible
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
            pulseTime += Time.deltaTime * 2f;
            float t = Mathf.PingPong(pulseTime, 1f);
            sr.color = Color.Lerp(Color.yellow, Color.white, t);
        }
    }
    
    public override void Initialize(MazeData data, MazeRenderer renderer, Vector2Int pos)
    {
        base.Initialize(data, renderer, pos);
        mazeData.GetCell(pos.x, pos.y).Content = CellContent.Crystal;
    }
    
    public override void Collect()
    {
        GameManager.Instance.CollectCrystal(this);
        mazeData.GetCell(gridPosition.x, gridPosition.y).Content = CellContent.Empty;
        Destroy(gameObject);
    }
}