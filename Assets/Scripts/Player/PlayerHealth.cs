using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private int currentHealth;
    private bool hasShield = false;
    private float shieldDuration = 5f;
    private float shieldTimer = 0f;
    
    private bool isInvulnerable = false;
    private float invulnerabilityDuration = 0.5f;
    private float invulnerabilityTimer = 0f;
    
    public void Initialize(int startingHealth)
    {
        currentHealth = startingHealth;
        hasShield = false;
        
        isInvulnerable = false;
        invulnerabilityTimer = 0f;
    }
    
    public int GetHealth()
    {
        return currentHealth;
    }
    
    public void AddHealth(int amount)
    {
        currentHealth += amount;
        UIManager.Instance?.UpdateHealth(currentHealth);
    }
    
    public void TakeDamage()
    {
        if (isInvulnerable)
            return;
        
        if (hasShield)
        {
            RemoveShield();
            isInvulnerable = true;
            invulnerabilityTimer = invulnerabilityDuration;
            return;
        }
        
        currentHealth--;
        UIManager.Instance?.UpdateHealth(currentHealth);
        
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;
        
        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            GameManager.Instance.RespawnPlayer();
        }
    }
    
    public void ActivateShield()
    {
        hasShield = true;
        shieldTimer = shieldDuration;
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(0.46f, 1f, 0.01f);
        }
    }
    
    public void RemoveShield()
    {
        hasShield = false;
        shieldTimer = 0f;
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.white;
        }
    }
    
    public bool HasShield()
    {
        return hasShield;
    }
    
    private void Update()
    {
        if (hasShield)
        {
            shieldTimer -= Time.deltaTime;
            
            if (shieldTimer <= 0)
            {
                RemoveShield();
            }
        }
        
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            
            if (invulnerabilityTimer <= 0)
            {
                isInvulnerable = false;
            }
        }
    }
}