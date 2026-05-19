using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float moveDistance = 1f; // Distance moved per step
    public float moveSpeed = 5f;    // Speed of movement

    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        // Only allow new movement when not already moving
        if (!isMoving)
        {
            Vector3 moveDirection = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                moveDirection = Vector3.up;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                moveDirection = Vector3.down;
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                moveDirection = Vector3.left;
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                moveDirection = Vector3.right;
            }

            if (moveDirection != Vector3.zero)
            {
                targetPosition = transform.position + moveDirection * moveDistance;
                isMoving = true;
            }
        }

        // Smooth movement toward target position
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Stop moving when target is reached
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }
}
