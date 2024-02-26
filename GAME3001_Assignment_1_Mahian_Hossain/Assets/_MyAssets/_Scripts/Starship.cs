// Starship.cs

using UnityEngine;

public class Starship : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float whiskerLength;
    [SerializeField] float avoidanceWeight;

    public Vector3 TargetPosition { get; set; }
    public bool IsArrival { get; set; }
    public Transform Obstacle { get; set; }

    private Rigidbody2D rb;
    private Vector2 screenBounds;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        screenBounds = new Vector2(5.6f, 4.2f);
    }

    void Update()
    {
        if (TargetPosition != null)
        {
            if (!IsArrival)
                SeekForward();
            else
                Arrival();
        }
    }

    void FixedUpdate()
    {
        AvoidObstacles();
    }

    private void SeekForward()
    {
        Vector2 directionToTarget = (TargetPosition - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90.0f;
        float angleDifference = Mathf.DeltaAngle(targetAngle, transform.eulerAngles.z);
        float rotationStep = rotationSpeed * Time.deltaTime;
        float rotationAmount = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
        transform.Rotate(Vector3.forward, rotationAmount);

        // Move along the forward vector using Rigidbody2D.
        Vector2 newPosition = rb.position + (Vector2)transform.up * movementSpeed * Time.deltaTime;

        // Check if the new position is within the screen boundaries.
        newPosition.x = Mathf.Clamp(newPosition.x, -screenBounds.x, screenBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, -screenBounds.y, screenBounds.y);

        // Update the position.
        rb.MovePosition(newPosition);
    }

    private void Arrival()
    {
        Vector2 directionToTarget = (TargetPosition - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, TargetPosition);

        // If the distance is greater than a threshold, move towards the target.
        if (distance > whiskerLength)
        {
            float speedFactor = Mathf.Clamp01(distance / whiskerLength);
            Vector2 desiredVelocity = directionToTarget * movementSpeed * speedFactor;
            rb.velocity = desiredVelocity;
        }

        // Calculate the angle to rotate towards the target.
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90.0f;
        float angleDifference = Mathf.DeltaAngle(targetAngle, transform.eulerAngles.z);
        float rotationStep = rotationSpeed * Time.deltaTime;
        float rotationAmount = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
        transform.Rotate(Vector3.forward, rotationAmount);
    }

    private void AvoidObstacles()
{
    if (Obstacle != null)
    {
        // Calculate direction to the target.
        Vector2 directionToTarget = (TargetPosition - transform.position).normalized;

        // Cast a ray towards the target to check for obstacles.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, whiskerLength);

        // Check if the ray hits an obstacle.
        if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
        {
            // Calculate the direction perpendicular to the obstacle.
            Vector2 avoidanceDirection = Vector2.Perpendicular(hit.normal).normalized;

            // Rotate the agent towards the avoidance direction.
            float targetAngle = Mathf.Atan2(avoidanceDirection.y, avoidanceDirection.x) * Mathf.Rad2Deg - 90.0f;
            float angleDifference = Mathf.DeltaAngle(targetAngle, transform.eulerAngles.z);
            float rotationStep = rotationSpeed * Time.deltaTime;
            float rotationAmount = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
            transform.Rotate(Vector3.forward, rotationAmount);
        }
        else
        {
            // No obstacle detected, continue seeking towards the target.
            SeekForward();
        }
    }
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Target")
        {
            GetComponent<AudioSource>().Play();
            // Play sound effect
        }
    }
}
