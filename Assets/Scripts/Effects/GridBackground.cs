using UnityEngine;

public class GridBackground : MonoBehaviour
{
    public Sprite starSprite;
    public int starCount = 30;
    public float starSpeed = 0.3f;
    public float minStarSize = 0.03f;
    public float maxStarSize = 0.07f;
    public bool useCyanStars = false;
    public bool rotateStars = true;
    public float rotationSpeed = 2f;
    
    void Start()
    {
        CreateStarfield();
    }
    
    void Update()
    {
        if (rotateStars)
        {
            
            transform.Rotate(0, 0, rotationSpeed * Time.unscaledDeltaTime);
        }
    }
    
    void CreateStarfield()
    {
        
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        float paddingX = 5f;
        float paddingY = 5f;
        
        for (int i = 0; i < starCount; i++)
        {
            GameObject star = new GameObject("Star");
            star.transform.parent = transform;
            
            
            float x = Random.Range(-width / 2f - paddingX, width / 2f + paddingX);
            float y = Random.Range(-height / 2f - paddingY, height / 2f + paddingY);
            star.transform.position = new Vector3(x, y, 5);
            
            SpriteRenderer sr = star.AddComponent<SpriteRenderer>();
            sr.sprite = starSprite != null ? starSprite : CreateDotSprite();
            
            
            Color starColor = useCyanStars && Random.value > 0.7f ? 
                new Color(0f, 1f, 1f, 1f) : 
                new Color(1f, 1f, 1f, 1f);
            
            sr.color = new Color(starColor.r, starColor.g, starColor.b, Random.Range(0.3f, 0.8f));
            sr.sortingOrder = -100;
            
            float scale = Random.Range(minStarSize, maxStarSize);
            star.transform.localScale = Vector3.one * scale;
            
            StarTwinkle twinkle = star.AddComponent<StarTwinkle>();
            twinkle.twinkleSpeed = Random.Range(0.5f, 3f);
        }
    }
    
    Sprite CreateDotSprite()
    {
        Texture2D tex = new Texture2D(4, 4);
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                tex.SetPixel(x, y, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
    }
}

public class StarTwinkle : MonoBehaviour
{
    public float twinkleSpeed = 1f;
    private SpriteRenderer sr;
    private float baseAlpha;
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            baseAlpha = sr.color.a;
    }
    
    void Update()
    {
        if (sr != null)
        {
            float alpha = baseAlpha * (0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * twinkleSpeed));
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}