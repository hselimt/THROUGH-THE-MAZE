using UnityEngine;

public class BackgroundEffect : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private Material backgroundMaterial;
    
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            backgroundMaterial = sr.material;
        }
    }
    
    void Update()
    {
        if (backgroundMaterial != null)
        {
            Vector2 offset = backgroundMaterial.mainTextureOffset;
            offset.y += scrollSpeed * Time.deltaTime;
            backgroundMaterial.mainTextureOffset = offset;
        }
    }
}