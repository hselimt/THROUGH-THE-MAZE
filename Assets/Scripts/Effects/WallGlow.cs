using UnityEngine;

public class WallGlow : MonoBehaviour
{
    public Color baseColor = Color.cyan;
    public float pulseSpeed = 1f;
    public float minAlpha = 0.2f;
    public float maxAlpha = 0.5f;

    private SpriteRenderer sr;
    private float nextUpdateTime = 0f;
    private const float UPDATE_INTERVAL = 0.05f; 

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        
        nextUpdateTime = Time.time + Random.Range(0f, UPDATE_INTERVAL);
    }

    void Update()
    {
        
        if (Time.time < nextUpdateTime) return;
        nextUpdateTime = Time.time + UPDATE_INTERVAL;

        if (sr != null)
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha,
                (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);

            sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }
    }
}