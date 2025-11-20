using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    
    [Header("References")]
    public MazeRenderer mazeRenderer;

    private Vector2Int currentGridPos;
    private Vector2Int targetGridPos;
    private bool isMoving = false;
    private MazeData mazeData;
    private LevelManager levelManager;

    private SwipeDetector.SwipeDirection currentDirection = SwipeDetector.SwipeDirection.None;

    private bool isGhostMode = false;
    private float ghostModeTimer = 0f;
    private const float GHOST_MODE_DURATION = 5f;

    private bool isSpeedBoosted = false;
    private float speedBoostTimer = 0f;
    private const float SPEED_BOOST_DURATION = 4f;
    private float baseSpeed;
    private SpriteRenderer sr;
    
    public void Initialize(MazeData data, Vector2Int startPos, LevelManager manager = null)
    {
        mazeData = data;
        currentGridPos = startPos;
        targetGridPos = startPos;
        levelManager = manager;

        if (mazeRenderer == null)
        {
            mazeRenderer = FindFirstObjectByType<MazeRenderer>();
        }

        if (levelManager == null)
        {
            levelManager = FindFirstObjectByType<LevelManager>();
        }

        if (mazeRenderer != null)
        {
            transform.position = mazeRenderer.GetWorldPosition(startPos.x, startPos.y);
        }

        sr = GetComponent<SpriteRenderer>();
        baseSpeed = moveSpeed;

        isGhostMode = false;
        isSpeedBoosted = false;
        ghostModeTimer = 0f;
        speedBoostTimer = 0f;

        mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.Player;
    }
        
    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused)
            return;

        UpdatePowerUpTimers();

        if (isMoving)
        {
            MoveToTarget();
        }
        else
        {
            HandleKeyboardInput();

            if (currentDirection != SwipeDetector.SwipeDirection.None)
            {
                TryMove(currentDirection);
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            currentDirection = SwipeDetector.SwipeDirection.Up;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            currentDirection = SwipeDetector.SwipeDirection.Down;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            currentDirection = SwipeDetector.SwipeDirection.Left;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            currentDirection = SwipeDetector.SwipeDirection.Right;
        }
    }

    private void UpdatePowerUpTimers()
    {
        if (isGhostMode)
        {
            ghostModeTimer -= Time.deltaTime;
            if (ghostModeTimer <= 0f)
            {
                DeactivateGhostMode();
            }
        }

        if (isSpeedBoosted)
        {
            speedBoostTimer -= Time.deltaTime;
            if (speedBoostTimer <= 0f)
            {
                DeactivateSpeedBoost();
            }
        }
    }
    
    public void OnSwipe(SwipeDetector.SwipeDirection direction)
    {
        if (!GameManager.Instance.IsGamePaused)
        {
            currentDirection = direction;
        }
    }
    
    private void TryMove(SwipeDetector.SwipeDirection direction)
    {
        Vector2Int newPos = currentGridPos;
        
        switch (direction)
        {
            case SwipeDetector.SwipeDirection.Up:
                newPos.y += 1;
                break;
            case SwipeDetector.SwipeDirection.Down:
                newPos.y -= 1;
                break;
            case SwipeDetector.SwipeDirection.Left:
                newPos.x -= 1;
                break;
            case SwipeDetector.SwipeDirection.Right:
                newPos.x += 1;
                break;
        }
        
        if (mazeData.CanMoveTo(currentGridPos.x, currentGridPos.y, newPos.x, newPos.y))
        {
            mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.Empty;

            targetGridPos = newPos;
            isMoving = true;
        }
    }
    
    private void MoveToTarget()
    {
        Vector3 targetWorldPos = mazeRenderer.GetWorldPosition(targetGridPos.x, targetGridPos.y);
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWorldPos) < 0.01f)
        {
            transform.position = targetWorldPos;
            currentGridPos = targetGridPos;
            isMoving = false;

            mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.Player;

            GameManager.Instance.CheckPlayerCollision(currentGridPos);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !isGhostMode)
        {
            if (other.GetComponent<PatrolEnemy>() == null)
            {
                GameManager.Instance?.PlayerHitByEnemy();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !isGhostMode)
        {
            if (other.GetComponent<PatrolEnemy>() == null)
            {
                GameManager.Instance?.PlayerHitByEnemy();
            }
        }
    }
    
    public Vector2Int GetGridPosition()
    {
        return currentGridPos;
    }
    
    public void TeleportTo(Vector2Int newPos)
    {
        mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.Empty;

        currentGridPos = newPos;
        targetGridPos = newPos;
        transform.position = mazeRenderer.GetWorldPosition(newPos.x, newPos.y);
        isMoving = false;

        currentDirection = SwipeDetector.SwipeDirection.None;

        mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.Player;
    }

    public void ActivateGhostMode()
    {
        isGhostMode = true;
        ghostModeTimer = GHOST_MODE_DURATION;

        if (sr != null)
        {
            sr.color = new Color(0f, 1f, 1f, 0.6f);
        }

        UIManager.Instance?.ShowSpeedBonus("GHOST MODE!");
    }

    private void DeactivateGhostMode()
    {
        isGhostMode = false;
        ghostModeTimer = 0f;

        if (sr != null)
        {
            sr.color = Color.white;
        }
    }

    public void ActivateSpeedBoost()
    {
        isSpeedBoosted = true;
        speedBoostTimer = SPEED_BOOST_DURATION;

        moveSpeed = baseSpeed * 2f;

        if (sr != null)
        {
            sr.color = new Color(1f, 0.7f, 0.3f);
        }

        UIManager.Instance?.ShowSpeedBonus("SPEED BOOST!");
    }

    private void DeactivateSpeedBoost()
    {
        isSpeedBoosted = false;
        speedBoostTimer = 0f;

        moveSpeed = baseSpeed;

        if (sr != null && !isGhostMode)
        {
            sr.color = Color.white;
        }
    }
}