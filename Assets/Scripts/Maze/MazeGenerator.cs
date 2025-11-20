using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator
{
    private MazeData mazeData;

    public MazeGenerator(int seed)
    {
        
    }

    public MazeData Generate()
    {
        mazeData = new MazeData();
        LoadHardcodedMaze();
        return mazeData;
    }

    private void LoadHardcodedMaze()
    {
        string[] horizontalWalls = new string[]
        {
            "############",  
            "..#.....##..",
            ".#.#..##...#",
            "..##....#...",
            "#...##...#..",
            "..#......#.#",
            ".#..##.#....",
            "..##..#...#.",
            ".#....##....",
            "....##...#..",  
            "#.....##....",
            "..#.....##..",
            ".#.#..##...#",
            "..##....#...",
            "#...##...#..",
            "..#.....#..#",
            ".#..##.#....",
            "..##..#...#.",
            ".#....##....",
            "....##...#..",  
        };

        string[] verticalWalls = new string[]
        {
            "#...#.#...#",  
            ".#.###...#.",
            "#.#...#.#..",
            ".#.#.#.#.#.",
            "#.#.#.#.#.#",
            ".#.#.#.#.#.",
            "#.#.#.#.#.#",
            ".#.##..#.#.",
            "#.#.#.#.#.#",
            ".#.....#..#",  
            "#.#.#.#.#.#",
            ".#.#.#...#.",
            "#.#.#.#.#..",
            ".#.#.#.###.",
            "#.#.#.#.#.#",
            ".#.#.#.#.#.",
            "#.#.#.#.#.#",
            ".#.##..#.#.",
            "#.#.#.#.#.#",
            ".#.....#...",  
        };

        ApplyWalls(horizontalWalls, verticalWalls);
    }

    
    private void ApplyWalls(string[] horizontalWalls, string[] verticalWalls)
    {
        
        for (int y = 0; y < MazeData.MAZE_HEIGHT && y < horizontalWalls.Length; y++)
        {
            string row = horizontalWalls[y];
            for (int x = 0; x < MazeData.MAZE_WIDTH && x < row.Length; x++)
            {
                if (row[x] == '#')
                {
                    
                    mazeData.GetCell(x, y).BottomWall = true;
                    if (y > 0)
                        mazeData.GetCell(x, y - 1).TopWall = true;
                }
                else
                {
                    
                    mazeData.GetCell(x, y).BottomWall = false;
                    if (y > 0)
                        mazeData.GetCell(x, y - 1).TopWall = false;
                }
            }
        }
        
        
        for (int y = 0; y < MazeData.MAZE_HEIGHT && y < verticalWalls.Length; y++)
        {
            string row = verticalWalls[y];
            for (int x = 0; x < MazeData.MAZE_WIDTH - 1 && x < row.Length; x++)
            {
                if (row[x] == '#')
                {
                    
                    mazeData.GetCell(x, y).RightWall = true;
                    mazeData.GetCell(x + 1, y).LeftWall = true;
                }
                else
                {
                    
                    mazeData.GetCell(x, y).RightWall = false;
                    mazeData.GetCell(x + 1, y).LeftWall = false;
                }
            }
        }
    }
}