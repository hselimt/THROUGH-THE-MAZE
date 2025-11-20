using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    
    protected Vector2Int currentGridPos;
    protected Vector2Int targetGridPos;
    protected bool isMoving = false;
    protected MazeData mazeData;
    protected MazeRenderer mazeRenderer;
    protected Transform playerTransform;
    
    protected float updateInterval = 0.2f;
    protected float nextUpdateTime = 0f;

    protected PlayerController cachedPlayer;

    private bool isFrozen = false;
    private float freezeTimer = 0f;
    private SpriteRenderer sr;
    
    public virtual void Initialize(MazeData data, MazeRenderer renderer, Vector2Int startPos, Transform player)
    {
        mazeData = data;
        mazeRenderer = renderer;
        currentGridPos = startPos;
        targetGridPos = startPos;
        playerTransform = player;

        transform.position = mazeRenderer.GetWorldPosition(startPos.x, startPos.y);
        mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.Enemy;

        nextUpdateTime = Time.time + Random.Range(0f, updateInterval);

        if (playerTransform != null)
        {
            cachedPlayer = playerTransform.GetComponent<PlayerController>();
        }

        sr = GetComponent<SpriteRenderer>();

        isFrozen = false;
        freezeTimer = 0f;
    }
    
    protected virtual void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused)
            return;

        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f)
            {
                Unfreeze();
            }
            return;
        }

        if (isMoving)
        {
            MoveToTarget();
        }
        else if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;
            DecideNextMove();
        }
    }
    
    protected abstract void DecideNextMove();

    protected virtual void MoveToTarget()
    {
        Vector3 targetWorldPos = mazeRenderer.GetWorldPosition(targetGridPos.x, targetGridPos.y);
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWorldPos) < 0.01f)
        {
            transform.position = targetWorldPos;

            CellContent oldContent = mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content;
            if (oldContent == CellContent.Enemy || oldContent == CellContent.PatrolEnemy)
                mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.Empty;

            currentGridPos = targetGridPos;

            mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = oldContent;

            isMoving = false;
        }
    }
    
    protected bool TryMove(Vector2Int newPos)
    {
        if (mazeData.CanMoveTo(currentGridPos.x, currentGridPos.y, newPos.x, newPos.y))
        {
            targetGridPos = newPos;
            isMoving = true;
            return true;
        }
        return false;
    }
    
    public Vector2Int GetGridPosition()
    {
        return currentGridPos;
    }
    
    public void TeleportTo(Vector2Int newPos)
    {
        CellContent oldContent = mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content;
        if (oldContent == CellContent.Enemy || oldContent == CellContent.PatrolEnemy)
            mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.Empty;

        currentGridPos = newPos;
        targetGridPos = newPos;
        transform.position = mazeRenderer.GetWorldPosition(newPos.x, newPos.y);
        isMoving = false;

        mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = oldContent;
    }

    public void Freeze(float duration)
    {
        isFrozen = true;
        freezeTimer = duration;

        if (sr != null)
        {
            sr.color = new Color(0.5f, 0.7f, 1f);
        }
    }

    private void Unfreeze()
    {
        isFrozen = false;
        freezeTimer = 0f;

        if (sr != null)
        {
            sr.color = Color.white;
        }
    }
}