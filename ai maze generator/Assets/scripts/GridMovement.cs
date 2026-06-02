using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class GridMovement : MonoBehaviour
{
    public float moveDistance = 1f;
    public float moveSpeed = 6f;

    private bool isMoving = false;
    private Vector3 targetPosition;
    private int hits = 0;
    private int maxHits = 3;
    private int lives = 3;
    private int maxLives = 3;

    private Vector3 startPosition;

    private ScreenFlash screenFlash;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI livesText;

    void Start()
    {
        targetPosition = transform.position;
        startPosition = transform.position;

        screenFlash = FindObjectOfType<ScreenFlash>();
        UpdateHealthUI();
        UpdateLivesUI();
    }
    void UpdateHealthUI()
    {
        healthText.text = "Health: " + (maxHits - hits);
    }
    void UpdateLivesUI()
    {
        livesText.text = "Lives: " + lives;
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector3 moveDirection = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                moveDirection = Vector3.up;

            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                moveDirection = Vector3.down;

            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                moveDirection = Vector3.left;

            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                moveDirection = Vector3.right;

            if (moveDirection != Vector3.zero)
            {
                Vector3 newPosition = transform.position + moveDirection * moveDistance;

                // Detect walls
                Collider2D hit = Physics2D.OverlapCircle(newPosition, 0.2f);

                // Move only if NOT a wall
                if (hit == null || !hit.CompareTag("Wall"))
                {
                    targetPosition = newPosition;
                    isMoving = true;
                }
                else
                {
                    // Player hit a wall
                    if (screenFlash != null)
                    {
                        StartCoroutine(screenFlash.Flash());
                    }
                    hits++;
                    UpdateHealthUI();

                    Debug.Log("Hit wall! Lives left: " + (maxHits - hits));

                    // Reset after 3 hits
                    if (hits >= maxHits)
                    {
                        lives--;

                        UpdateLivesUI();

                        Debug.Log("Lost a life!");

                        transform.position = startPosition;
                        targetPosition = startPosition;

                        hits = 0;

                        UpdateHealthUI();

                        // Game Over
                        if (lives <= 0)
                        {
                            Debug.Log("GAME OVER");

                            lives = maxLives;

                            UpdateLivesUI();

                            MazeGenerator maze = FindObjectOfType<MazeGenerator>();

                            maze.width = 17;
                            maze.height = 17;
                            maze.level = 1;

                            maze.GenerateMaze();
                        }
                    }
                }
            }
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;

                // Find the maze generator
                MazeGenerator maze = FindObjectOfType<MazeGenerator>();

                // Check if player reached the exit
                if (maze.IsGoal(transform.position))
                {
                    maze.NextLevel();
                }

            }

        }
        if (Time.timeScale == 0f)
            return;
    }
    public void SetStartPosition(Vector3 newStart)
{
    startPosition = newStart;
}
}
