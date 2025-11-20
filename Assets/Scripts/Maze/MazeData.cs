using UnityEngine;

public class MazeData
{
    public const int MAZE_WIDTH = 12;
    public const int MAZE_HEIGHT = 20;

    public MazeCell[,] Cells { get; private set; }

    public MazeData()
    {
        Cells = new MazeCell[MAZE_WIDTH, MAZE_HEIGHT];
        InitializeCells();
    }
    
    private void InitializeCells()
    {
        for (int x = 0; x < MAZE_WIDTH; x++)
        {
            for (int y = 0; y < MAZE_HEIGHT; y++)
            {
                Cells[x, y] = new MazeCell(x, y);
            }
        }
    }
    
    public MazeCell GetCell(int x, int y)
    {
        if (x < 0 || x >= MAZE_WIDTH || y < 0 || y >= MAZE_HEIGHT)
            return null;
        return Cells[x, y];
    }
    
    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < MAZE_WIDTH && y >= 0 && y < MAZE_HEIGHT;
    }
    
    public bool CanMoveTo(int fromX, int fromY, int toX, int toY)
    {
        if (!IsValidPosition(toX, toY))
            return false;
        
        MazeCell fromCell = GetCell(fromX, fromY);
        MazeCell toCell = GetCell(toX, toY);
        
        if (fromCell == null || toCell == null)
            return false;
        
        
        int dx = toX - fromX;
        int dy = toY - fromY;
        
        if (dx == 1 && fromCell.RightWall) return false;
        if (dx == -1 && fromCell.LeftWall) return false;
        if (dy == 1 && fromCell.TopWall) return false;
        if (dy == -1 && fromCell.BottomWall) return false;

        
        
        return toCell.Content == CellContent.Empty ||
            toCell.Content == CellContent.Crystal ||
            toCell.Content == CellContent.Enemy;
    }
}