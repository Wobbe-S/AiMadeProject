using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 17;
    public int height = 17;
    public int level = 1;
    public int maxMazeSize = 37;

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject player;

    private Color wallColor;
    private int[,] maze;

    void Start()
    {
        GenerateMaze();
    }

    public void GenerateMaze()
    {
        //random wall color
        Color[] colors =
        {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            Color.magenta,
            Color.cyan
        };

        wallColor = colors[Random.Range(0, colors.Length)];
        Debug.Log("Generating maze...");
        // Delete old maze
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        maze = new int[width, height];

        // Fill with walls
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 1;
            }
        }

        GeneratePath(1, 1);
            
        // Create entrance and exit
        // Entrance (top left outside)
        maze[0, 1] = 0;

        // Exit (bottom right outside)
        maze[width - 1, height - 2] = 0;

        DrawMaze();

        // Move player to start
        Vector3 startPos = new Vector3(0, 1, -1);

        player.transform.position = startPos;

        GridMovement movement = player.GetComponent<GridMovement>();

        if (movement != null)
        {
            movement.SetStartPosition(startPos);
        }
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10);

        Camera.main.orthographicSize = Mathf.Max(width, height) / 2f;
    }

    void GeneratePath(int x, int y)
    {
        maze[x, y] = 0;

        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        Shuffle(directions);

        foreach (Vector2Int dir in directions)
        {
            int newX = x + dir.x * 2;
            int newY = y + dir.y * 2;

            if (newX > 0 && newX < width - 1 &&
                newY > 0 && newY < height - 1 &&
                maze[newX, newY] == 1)
            {
                maze[x + dir.x, y + dir.y] = 0;
                GeneratePath(newX, newY);
            }
        }
    }

    void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Vector2Int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void DrawMaze()
    {
        if (floorPrefab == null)
        {
            Debug.LogError("Floor Prefab is NULL!");
        }

        if (wallPrefab == null)
        {
            Debug.LogError("Wall Prefab is NULL!");
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject prefab = maze[x, y] == 1 ? wallPrefab : floorPrefab;

                GameObject tile = Instantiate(
                    prefab,
                    new Vector3(x, y, 0),
                    Quaternion.identity,
                    transform
                );

                if (maze[x, y] == 1)
                {
                    SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();

                    if (sr != null)
                    {
                        sr.color = wallColor;
                    }
                }
            }
        }
    }

    public bool IsGoal(Vector3 playerPos)
    {
        return Mathf.RoundToInt(playerPos.x) == width - 1 &&
               Mathf.RoundToInt(playerPos.y) == height - 2;
    }
    public void NextLevel()
    {
        if (width >= maxMazeSize && height >= maxMazeSize)
        {
            WinScreenManager win =
                FindFirstObjectByType<WinScreenManager>();

            if (win != null)
            {
                win.ShowWinScreen();
            }

            return;
        }

        width = Mathf.Min(width + 2, maxMazeSize);
        height = Mathf.Min(height + 2, maxMazeSize);

        level++;

        GenerateMaze();
    }
    public bool IsWalkable(int x, int y)
    {
        // Entrance
        if (x == 0 && y == 1)
            return true;

        // Exit
        if (x == width - 1 && y == height - 2)
            return true;

        // Border walls
        if (x == 0 || x == width - 1 ||
            y == 0 || y == height - 1)
            return false;

        return maze[x, y] == 0;
    }
}
