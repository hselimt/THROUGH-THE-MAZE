using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    protected Vector2Int gridPosition;
    protected MazeData mazeData;
    protected MazeRenderer mazeRenderer;
    
    public virtual void Initialize(MazeData data, MazeRenderer renderer, Vector2Int pos)
    {
        mazeData = data;
        mazeRenderer = renderer;
        gridPosition = pos;
        
        transform.position = mazeRenderer.GetWorldPosition(pos.x, pos.y);
    }
    
    public abstract void Collect();
    
    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }
}
