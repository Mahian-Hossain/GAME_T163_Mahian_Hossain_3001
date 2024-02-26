using UnityEngine;

public class Starship : AgentObject
{
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float avoidanceWeight;
    [SerializeField] float whiskerLength;
    [SerializeField] float whiskerAngle;

    private Rigidbody2D rb;
    private bool reachedTarget = false;

    new void Start()
    {
        base.Start();
        Debug.Log("Starting Starship.");
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (TargetPosition != null && !reachedTarget)
        {
            SeekForward();
            AvoidObstacles();
        }
    }

    private void AvoidObstacles()
    {
        CastWhisker(Vector2.up);

        if (ObstacleDetected(Vector2.up))
        {
            RotateCounterClockwise();
        }
    }

    private void CastWhisker(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, whiskerLength);
        Debug.DrawRay(transform.position, direction * whiskerLength, Color.red);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                HandleObstacle(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("AnotherTag"))
            {
                // Handle other types of objects based on their tags.
            }
        }
    }

    private void HandleObstacle(GameObject obstacle)
    {
        Renderer obstacleRenderer = obstacle.GetComponent<Renderer>();
        if (obstacleRenderer != null)
        {
            obstacleRenderer.material.color = Color.yellow;
        }

        Vector2 directionAwayFromObstacle = (transform.position - obstacle.transform.position).normalized;
        rb.velocity = directionAwayFromObstacle * movementSpeed;
    }

    private bool ObstacleDetected(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, whiskerLength);
        return (hit.collider != null && hit.collider.CompareTag("Obstacle"));
    }

    private void RotateCounterClockwise()
    {
        float rotationAmount = -rotationSpeed * avoidanceWeight * Time.deltaTime;
        transform.Rotate(Vector3.forward, rotationAmount);
    }

    private void SeekForward()
    {
        Vector2 directionToTarget = (TargetPosition - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg + 90.0f;
        float angleDifference = Mathf.DeltaAngle(targetAngle, transform.eulerAngles.z);
        float rotationStep = rotationSpeed * Time.deltaTime;
        float rotationAmount = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
        transform.Rotate(Vector3.forward, rotationAmount);
        rb.velocity = transform.up * movementSpeed;

        // Check if the distance to the target is below a threshold to stop rotation and movement.
        float distanceToTarget = Vector2.Distance(transform.position, TargetPosition);
        if (distanceToTarget < 0.5f)
        {
            reachedTarget = true;
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            GetComponent<AudioSource>().Play();
            // Consider using a sound manager instead of directly playing audio here.
        }
    }
}
