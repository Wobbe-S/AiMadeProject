using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 21;
    public int height = 21;

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject player;

    private int[,] maze;

    void Start()
    {
        GenerateNewMaze();
    }

    public void GenerateNewMaze()
    {
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
        maze[1, 0] = 0;
        maze[width - 2, height - 1] = 0;

        DrawMaze();

        // Move player to start
        player.transform.position = new Vector3(1, 1, -1);
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject prefab = maze[x, y] == 1 ? wallPrefab : floorPrefab;

                Instantiate(
                    prefab,
                    new Vector3(x, y, 0),
                    Quaternion.identity,
                    transform
                );
            }
        }
    }

    public bool IsGoal(Vector3 playerPos)
    {
        return Mathf.RoundToInt(playerPos.x) == width - 2 &&
               Mathf.RoundToInt(playerPos.y) == height - 1;
    }
}
