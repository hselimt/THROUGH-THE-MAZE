using UnityEngine;

public enum CellContent
{
    Empty,
    Player,
    Enemy,
    PatrolEnemy,  
    Crystal,
    Health,
    PortalToken,
    Shield,
    PortalA,
    PortalB
}

public class MazeCell
{
    public int X { get; set; }
    public int Y { get; set; }
    
    
    public bool TopWall { get; set; }
    public bool RightWall { get; set; }
    public bool BottomWall { get; set; }
    public bool LeftWall { get; set; }
    
    public bool Visited { get; set; } 
    public CellContent Content { get; set; }
    
    public MazeCell(int x, int y)
    {
        X = x;
        Y = y;
        
        
        TopWall = true;
        RightWall = true;
        BottomWall = true;
        LeftWall = true;
        
        Visited = false;
        Content = CellContent.Empty;
    }
    
    public bool IsFullyWalled()
    {
        return TopWall && RightWall && BottomWall && LeftWall;
    }
    
    public bool IsWalkable()
    {
        return Content != CellContent.Enemy && Content != CellContent.PatrolEnemy;
    }
}
