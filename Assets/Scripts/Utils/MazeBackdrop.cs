using UnityEngine;

public class MazeBackdrop : MonoBehaviour
{
    void Start()
    {
        CreateBackdrop();
    }
    
    void CreateBackdrop()
    {
        GameObject backdrop = new GameObject("MazeBackdrop");
        backdrop.transform.position = new Vector3(0, 0, 0.5f); 
        
        SpriteRenderer sr = backdrop.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite();
        sr.color = Color.black;
        sr.sortingOrder = -50; 
        
        
        backdrop.transform.localScale = new Vector3(14f, 22f, 1f);
    }
    
    Sprite CreateSquareSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
    }
}