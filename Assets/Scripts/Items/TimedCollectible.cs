using UnityEngine;
using TMPro;

public abstract class TimedCollectible : Collectible
{
    [Header("Timer")]
    public float lifetime = 5f;
    
    protected float timer;
    protected TextMeshPro timerText;
    protected SpriteRenderer spriteRenderer;
    
    public override void Initialize(MazeData data, MazeRenderer renderer, Vector2Int pos)
    {
        base.Initialize(data, renderer, pos);
        timer = lifetime;
        
        
        GameObject textObj = new GameObject("Timer");
        textObj.transform.parent = transform;
        textObj.transform.localPosition = new Vector3(0, 0.5f, 0);
        
        timerText = textObj.AddComponent<TextMeshPro>();
        timerText.fontSize = 3;
        timerText.alignment = TextAlignmentOptions.Center;
        timerText.color = Color.white;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    protected virtual void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused)
            return;
        
        timer -= Time.deltaTime;
        
        
        int secondsLeft = Mathf.CeilToInt(timer);
        timerText.text = secondsLeft.ToString();
        
        
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
    
    protected virtual void Disappear()
    {
        mazeData.GetCell(gridPosition.x, gridPosition.y).Content = CellContent.Empty;

        timerText.text = "💨";
        Destroy(gameObject, 0.3f);
    }
}
