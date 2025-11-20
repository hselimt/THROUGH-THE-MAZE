using UnityEngine;

public class PatrolEnemy : Enemy
{
    private Vector2Int patrolDirection;
    private bool hasChosenDirection = false;
    
    public override void Initialize(MazeData data, MazeRenderer renderer, Vector2Int startPos, Transform player)
    {
        base.Initialize(data, renderer, startPos, player);
        moveSpeed = 2f;

        mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.PatrolEnemy;

        ChooseRandomDirection();
    }

    protected new bool TryMove(Vector2Int newPos)
    {
        if (base.TryMove(newPos))
        {
            mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.Empty;
            mazeData.GetCell(newPos.x, newPos.y).Content = CellContent.PatrolEnemy;
            return true;
        }
        return false;
    }

    protected override void MoveToTarget()
    {
        Vector3 targetWorldPos = mazeRenderer.GetWorldPosition(targetGridPos.x, targetGridPos.y);
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWorldPos) < 0.01f)
        {
            transform.position = targetWorldPos;
            currentGridPos = targetGridPos;
            isMoving = false;
        }
    }

    protected override void DecideNextMove()
    {
        if (!hasChosenDirection)
        {
            ChooseRandomDirection();
        }

        Vector2Int nextPos = currentGridPos + patrolDirection;

        if (!TryMove(nextPos))
        {
            ChooseRandomDirection();
        }
    }

    private void ChooseRandomDirection()
    {
        Vector2Int[] directions = {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0)
        };

        patrolDirection = directions[Random.Range(0, directions.Length)];
        hasChosenDirection = true;
    }
}