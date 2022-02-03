using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MazeController : MonoBehaviour
{
    public GameObject mazeWalls;
    public GameObject wallPrefab;
    public GameObject deathZonePrefab;
    public Maze maze;
    public NavMeshSurface navSurface;

    void Start()
    {
        maze = new Maze(10, 10);
        StartCoroutine(RebuildMaze());
    }

    public IEnumerator RebuildMaze()
    {
        foreach (Transform wall in mazeWalls.transform.Cast<Transform>().ToArray())
        {
            Destroy(wall.gameObject);
        }
        yield return new WaitForEndOfFrame();
        maze.Reset();
        maze.Generate();
        BuildWalls();
        GenerateDeathZones();
    }

    private void BuildWalls()
    {
        for (int j = 0; j < maze.cells.GetLength(1); j++)
        {
            for (int i = 0; i < maze.cells.GetLength(0); i++)
            {
                if (i < maze.cells.GetLength(0) - 1 && (maze.cells[i, j] & Maze.Side.South) == 0)
                {
                    var wall = Instantiate(
                        wallPrefab,
                        new Vector3(i * 2 - (maze.cells.GetLength(0) - 2), 0.75f, j * 2 - (maze.cells.GetLength(1) - 1)),
                        Quaternion.identity
                    );
                    wall.transform.parent = mazeWalls.transform;
                    wall.transform.localScale = new Vector3(0.2f, 1.5f, 2);
                }
                if (j < maze.cells.GetLength(1) - 1 && (maze.cells[i, j] & Maze.Side.East) == 0)
                {
                    var wall = Instantiate(
                        wallPrefab,
                        new Vector3(i * 2 - (maze.cells.GetLength(0) - 1), 0.75f, j * 2 - (maze.cells.GetLength(1) - 2)),
                        Quaternion.identity
                    );
                    wall.transform.parent = mazeWalls.transform;
                    wall.transform.localScale = new Vector3(2, 1.5f, 0.2f);
                }
            }
        }
        navSurface.BuildNavMesh();
    }

    private void GenerateDeathZones()
    {
        for (int i = 0; i < 10; i++)
        {
            int x = UnityEngine.Random.Range(1, maze.cells.GetLength(0));
            int y = UnityEngine.Random.Range(1, maze.cells.GetLength(1));
            var deathZone = Instantiate(
                deathZonePrefab,
                new Vector3(x * 2 - (maze.cells.GetLength(0) - 1), 0.01f, y * 2 - (maze.cells.GetLength(1) - 1)),
                Quaternion.identity
            );
            deathZone.transform.parent = mazeWalls.transform;
        }
    }
}

public class Maze
{
    [Flags]
    public enum Side
    {
        North = 1 << 1,
        South = 1 << 2,
        East = 1 << 3,
        West = 1 << 4
    }

    public Side[,] cells;
    private System.Random rnd;
    private Side[] sides = new Side[] { Side.East, Side.North, Side.South, Side.West };
    private Dictionary<Side, int> dx = new Dictionary<Side, int>
    {
        { Side.East, 0 },
        { Side.West, 0 },
        { Side.North, -1 },
        { Side.South, 1 },
    };
    private Dictionary<Side, int> dz = new Dictionary<Side, int>
    {
        { Side.East, 1 },
        { Side.West, -1 },
        { Side.North, 0 },
        { Side.South, 0 },
    };
    private Dictionary<Side, Side> opposite = new Dictionary<Side, Side>
    {
        { Side.East, Side.West },
        { Side.West, Side.East },
        { Side.South, Side.North },
        { Side.North, Side.South },
    };

    public Maze(int width, int height)
    {
        rnd = new System.Random();
        cells = new Side[width, height];
    }

    public void Reset()
    {
        for (int i = 0; i < cells.GetLength(0); i++)
            for (int j = 0; j < cells.GetLength(1); j++)
                cells[i, j] = 0;
    }

    public void Generate()
    {
        CarvePassageFrom(0, 0);
        AddPassages();
    }

    private void CarvePassageFrom(int cx, int cz)
    {
        Side[] directions = sides.OrderBy(item => rnd.Next()).ToArray();

        foreach (Side direction in directions)
        {
            int nx = cx + dx[direction], nz = cz + dz[direction];
            if (nx < 0 || nx >= cells.GetLength(0) || nz < 0 || nz >= cells.GetLength(1) || cells[nx, nz] != 0)
                continue;
            cells[cx, cz] |= direction;
            cells[nx, nz] |= opposite[direction];
            CarvePassageFrom(nx, nz);
        }
    }

    private void AddPassages()
    {
        for (int i = 0; i < 10; i++)
        {
            cells[rnd.Next(1, cells.GetLength(0)), rnd.Next(1, cells.GetLength(1))] = Side.East | Side.North | Side.South | Side.West;
        }
        cells[0, cells.GetLength(1) - 1] = Side.East | Side.North | Side.South | Side.West;
        cells[cells.GetLength(0) - 1, 0] = Side.East | Side.North | Side.South | Side.West;
    }
}
