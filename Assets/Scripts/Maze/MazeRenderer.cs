using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [Header("Maze Settings")]
    public float cellSize = 1f;
    public float wallThickness = 0.1f;
    
    [Header("Visual Settings")]
    public Sprite wallSprite;
    public Color wallColor = new Color(0f, 0.94f, 1f); 
    public Color backgroundColor = new Color(0.04f, 0.05f, 0.15f); 

    private MazeData mazeData;
    private GameObject wallsParent;

    
    private static Material sharedWallMaterial;
    private static Sprite cachedSquareSprite;
    
    public void RenderMaze(MazeData data)
    {
        mazeData = data;

        
        InitializeSharedResources();

        
        if (wallsParent != null)
            Destroy(wallsParent);

        wallsParent = new GameObject("Walls");
        wallsParent.transform.parent = transform;

        
        Camera.main.backgroundColor = backgroundColor;
        
        
        Vector3 offset = new Vector3(
            -MazeData.MAZE_WIDTH * cellSize / 2f + cellSize / 2f,
            -MazeData.MAZE_HEIGHT * cellSize / 2f + cellSize / 2f, 
            0
        );
        wallsParent.transform.position = offset;
        
        
        for (int x = 0; x < MazeData.MAZE_WIDTH; x++)
        {
            for (int y = 0; y < MazeData.MAZE_HEIGHT; y++)
            {
                RenderCell(x, y);
            }
        }
    }
    
    private void InitializeSharedResources()
    {
        
        if (sharedWallMaterial == null)
        {
            sharedWallMaterial = new Material(Shader.Find("Sprites/Default"));
            sharedWallMaterial.EnableKeyword("_EMISSION");
            sharedWallMaterial.SetColor("_EmissionColor", wallColor * 0.3f);
        }

        
        if (cachedSquareSprite == null)
        {
            cachedSquareSprite = CreateSquareSprite();
        }
    }

    private void RenderCell(int x, int y)
    {
        MazeCell cell = mazeData.GetCell(x, y);
        Vector3 cellPos = new Vector3(x * cellSize, y * cellSize, 0);
        
        
        if (cell.TopWall)
            CreateWall(cellPos + new Vector3(0, cellSize / 2f, 0), new Vector2(cellSize, wallThickness), "TopWall");
        
        if (cell.RightWall)
            CreateWall(cellPos + new Vector3(cellSize / 2f, 0, 0), new Vector2(wallThickness, cellSize), "RightWall");
        
        if (cell.BottomWall)
            CreateWall(cellPos + new Vector3(0, -cellSize / 2f, 0), new Vector2(cellSize, wallThickness), "BottomWall");
        
        if (cell.LeftWall)
            CreateWall(cellPos + new Vector3(-cellSize / 2f, 0, 0), new Vector2(wallThickness, cellSize), "LeftWall");
    }
    
    private void CreateWall(Vector3 position, Vector2 size, string wallName)
    {
        GameObject wall = new GameObject(wallName);
        wall.transform.parent = wallsParent.transform;
        wall.transform.localPosition = position;
        
        
        SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
        sr.sprite = wallSprite != null ? wallSprite : cachedSquareSprite;
        sr.color = wallColor;
        sr.sortingOrder = 0;

        
        sr.sharedMaterial = sharedWallMaterial;

        
        GameObject glowObj = new GameObject("Glow");
        glowObj.transform.parent = wall.transform;
        glowObj.transform.localPosition = Vector3.zero;
        glowObj.transform.localScale = new Vector3(1.3f, 1.3f, 1f); 

        SpriteRenderer glowSr = glowObj.AddComponent<SpriteRenderer>();
        glowSr.sprite = sr.sprite;
        glowSr.color = new Color(wallColor.r, wallColor.g, wallColor.b, 0.3f); 
        glowSr.sortingOrder = -1; 

        
        WallGlow glow = glowObj.AddComponent<WallGlow>();
        glow.baseColor = wallColor;
        
        
        wall.transform.localScale = new Vector3(size.x, size.y, 1);
        
        
        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        
        wall.tag = "Wall";
        wall.layer = LayerMask.NameToLayer("Default");
    }
    
    private Sprite CreateSquareSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
    }
    
    public Vector3 GetWorldPosition(int gridX, int gridY)
    {
        Vector3 offset = new Vector3(
            -MazeData.MAZE_WIDTH * cellSize / 2f + cellSize / 2f,
            -MazeData.MAZE_HEIGHT * cellSize / 2f + cellSize / 2f,
            0
        );
        return new Vector3(gridX * cellSize, gridY * cellSize, 0) + offset;
    }
    
    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        Vector3 offset = new Vector3(
            MazeData.MAZE_WIDTH * cellSize / 2f - cellSize / 2f,
            MazeData.MAZE_HEIGHT * cellSize / 2f - cellSize / 2f,
            0
        );
        Vector3 localPos = worldPos + offset;
        
        int x = Mathf.RoundToInt(localPos.x / cellSize);
        int y = Mathf.RoundToInt(localPos.y / cellSize);
        
        return new Vector2Int(x, y);
    }
}