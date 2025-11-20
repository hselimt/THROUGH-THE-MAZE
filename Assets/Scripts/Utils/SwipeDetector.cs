using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    public float minSwipeDistance = 50f;
    public bool useMouseForTesting = true;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool isSwiping = false;
    private bool swipeProcessed = false;

    private PlayerController playerController;
    
    public enum SwipeDirection
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        DetectSwipe();
    }
    
    private void DetectSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                isSwiping = true;
                swipeProcessed = false;
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping && !swipeProcessed)
            {
                endTouchPosition = touch.position;
                Vector2 swipeVector = endTouchPosition - startTouchPosition;

                if (swipeVector.magnitude >= minSwipeDistance)
                {
                    ProcessSwipe();
                    swipeProcessed = true;
                }
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping && !swipeProcessed)
            {
                endTouchPosition = touch.position;
                ProcessSwipe();
                isSwiping = false;
                swipeProcessed = false;
            }
        }
        else if (useMouseForTesting)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startTouchPosition = Input.mousePosition;
                isSwiping = true;
                swipeProcessed = false;
            }
            else if (Input.GetMouseButton(0) && isSwiping && !swipeProcessed)
            {
                endTouchPosition = Input.mousePosition;
                Vector2 swipeVector = endTouchPosition - startTouchPosition;

                if (swipeVector.magnitude >= minSwipeDistance)
                {
                    ProcessSwipe();
                    swipeProcessed = true;
                }
            }
            else if (Input.GetMouseButtonUp(0) && isSwiping && !swipeProcessed)
            {
                endTouchPosition = Input.mousePosition;
                ProcessSwipe();
                isSwiping = false;
                swipeProcessed = false;
            }
        }
    }
    
    private void ProcessSwipe()
    {
        Vector2 swipeVector = endTouchPosition - startTouchPosition;
        float swipeDistance = swipeVector.magnitude;
        
        if (swipeDistance < minSwipeDistance)
            return;
        
        SwipeDirection direction = GetSwipeDirection(swipeVector);

        if (direction != SwipeDirection.None && playerController != null)
        {
            playerController.OnSwipe(direction);
        }
    }
    
    private SwipeDirection GetSwipeDirection(Vector2 swipeVector)
    {
        swipeVector.Normalize();
        
        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            return swipeVector.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
        }
        else
        {
            return swipeVector.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
        }
    }
}