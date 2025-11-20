using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ModernButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Colors")]
    public Color normalColor = new Color(0.12f, 0.66f, 0.88f);
    public Color hoverColor = new Color(0.2f, 0.8f, 1f);
    public Color pressColor = new Color(0.08f, 0.5f, 0.7f);
    
    private Image buttonImage;
    private Shadow shadow;
    private Vector3 originalPosition;
    private Vector2 originalShadowDistance;
    
    void Start()
    {
        buttonImage = GetComponent<Image>();
        originalPosition = transform.localPosition;
        
        
        shadow = gameObject.GetComponent<Shadow>();
        if (shadow == null)
            shadow = gameObject.AddComponent<Shadow>();
        
        shadow.effectColor = new Color(0, 0, 0, 0.6f);
        shadow.effectDistance = new Vector2(0, -8);
        originalShadowDistance = shadow.effectDistance;
        
        if (buttonImage != null)
            buttonImage.color = normalColor;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.color = hoverColor;
        
        transform.localScale = Vector3.one * 1.05f;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.color = normalColor;
        
        transform.localScale = Vector3.one;
        transform.localPosition = originalPosition;
        
        if (shadow != null)
            shadow.effectDistance = originalShadowDistance;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.color = pressColor;
        
        transform.localPosition = originalPosition + new Vector3(0, -6, 0);
        transform.localScale = Vector3.one * 0.98f;
        
        if (shadow != null)
            shadow.effectDistance = new Vector2(0, -3);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.color = hoverColor;
        
        transform.localPosition = originalPosition;
        transform.localScale = Vector3.one * 1.05f;
        
        if (shadow != null)
            shadow.effectDistance = originalShadowDistance;
    }
}