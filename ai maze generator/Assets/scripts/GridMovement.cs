using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class GridMovement : MonoBehaviour
{
    public float moveDistance = 1f;
    public float moveSpeed = 6f;

    private bool isMoving = false;
    private Vector3 targetPosition;
    private int hits = 0;
    private int maxHits = 3;

    private Vector3 startPosition;

    private ScreenFlash screenFlash;

    void Start()
    {
        targetPosition = transform.position;
        startPosition = transform.position;

        screenFlash = FindObjectOfType<ScreenFlash>();
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
                    
                    Debug.Log("Hit wall! Lives left: " + (maxHits - hits));

                    // Reset after 3 hits
                    if (hits >= maxHits)
                    {
                        Debug.Log("Too many hits! Resetting player.");

                        transform.position = startPosition;
                        targetPosition = startPosition;

                        hits = 0;
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
    }
    public void SetStartPosition(Vector3 newStart)
{
    startPosition = newStart;
}
}
