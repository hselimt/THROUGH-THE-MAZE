using UnityEngine;

public class TeleporterEnemy : Enemy
{
    private float teleportCooldown = 3f;
    private float lastTeleportTime = 0f;
    
    public override void Initialize(MazeData data, MazeRenderer renderer, Vector2Int startPos, Transform player)
    {
        base.Initialize(data, renderer, startPos, player);
        moveSpeed = 0f;
        updateInterval = 0.5f;
    }
    
    protected override void DecideNextMove()
    {
        if (Time.time - lastTeleportTime >= teleportCooldown)
        {
            TeleportToRandomLocation();
            lastTeleportTime = Time.time;
        }
    }
    
    private void TeleportToRandomLocation()
    {
        Vector2Int newPos;
        int maxAttempts = 20;
        int attempts = 0;
        
        do
        {
            newPos = new Vector2Int(
                Random.Range(0, MazeData.MAZE_WIDTH),
                Random.Range(0, MazeData.MAZE_HEIGHT)
            );
            attempts++;
        }
        while (mazeData.GetCell(newPos.x, newPos.y).Content != CellContent.Empty && attempts < maxAttempts);
        
        if (attempts < maxAttempts)
        {
            TeleportTo(newPos);
        }
    }
}