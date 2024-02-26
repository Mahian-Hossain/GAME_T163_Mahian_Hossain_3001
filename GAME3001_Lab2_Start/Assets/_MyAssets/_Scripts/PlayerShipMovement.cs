using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerShipMovement : MonoBehaviour
{
    public float speed = 5f; // Adjust the speed as needed

    // Define movement boundaries
    public float minX = -6.3f;
    public float maxX = 6.3f;
    public float minY = -4.3f;
    public float maxY = 4.3f;
    public GameObject Game_status;

    public AudioClip collisionSound; // Assign the collision sound effect in the Inspector

    private AudioSource audioSource;


    void Start()
    {

        Game_status.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        // Player movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0) * speed * Time.deltaTime;

        // Calculate the new position
        Vector3 newPosition = transform.position + movement;

        // Clamp the new position within the boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        // Update the position
        transform.position = newPosition;

        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enimy"))
        {

            Invoke("ReloadScene", 2f);
            Game_status.SetActive(true);

            if (collisionSound != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }

        }
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
}
