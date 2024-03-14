using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySceneScript : MonoBehaviour
{
    public GameObject actorAgentPrefab; // Prefab for instantiating actor/agent
    public GameObject targetPrefab; // Prefab for instantiating target
    public GameObject obstaclePrefab; // Prefab for instantiating obstacle
    public Text instructionText;
    public AudioSource soundEffectPlayer;
    public AudioClip clickSound;
    public AudioClip backgroundMusic;

    private GameObject actorAgentInstance; // Instance of the actor/agent
    private GameObject targetInstance; // Instance of the target
    private GameObject obstacleInstance; // Instance of the obstacle
    private bool musicStarted = false;

    private Vector3 minBounds;
    private Vector3 maxBounds;

    
    void Start()
    {
        instructionText.text = "Press 1: Seek\nPress 2: Flee\nPress 3: Arrival\nPress 4: Obstacle Avoidance\nPress 5: Reset";

        // Screen boundaries
        minBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        maxBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (!musicStarted && backgroundMusic != null)
        {
            soundEffectPlayer.clip = backgroundMusic;
            soundEffectPlayer.Play();
            musicStarted = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Seek();
            PlaySoundEffect(clickSound);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Flee();
            PlaySoundEffect(clickSound);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Arrival();
            PlaySoundEffect(clickSound);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ObstacleAvoidance();
            PlaySoundEffect(clickSound);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ResetScene();
            PlaySoundEffect(clickSound);
        }
    }

    private void Seek()
    {
        DestroyInstances();

        actorAgentInstance = Instantiate(actorAgentPrefab, GetRandomSpawnPoint(), Quaternion.identity);
        targetInstance = Instantiate(targetPrefab, GetRandomSpawnPoint(), Quaternion.identity);

        Vector3 direction = (targetInstance.transform.position - actorAgentInstance.transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        actorAgentInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        float speed = 10f; 
        actorAgentInstance.GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    private void Flee()
    {
        DestroyInstances();

        // Instantiate actor/agent and target
        Vector3 spawnPoint = GetRandomSpawnPoint();
        actorAgentInstance = Instantiate(actorAgentPrefab, spawnPoint, Quaternion.identity);
        targetInstance = Instantiate(targetPrefab, spawnPoint + new Vector3(1f, 1f, 0f), Quaternion.identity);

        // Move actor/agent away from the target
        Vector3 direction = (actorAgentInstance.transform.position - targetInstance.transform.position).normalized;

        // Set agent's rotation to face away from the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        actorAgentInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    
        float speed = 10f;

        actorAgentInstance.GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    private void Arrival()
    {
        DestroyInstances();

        // Instantiate actor/agent and target
        actorAgentInstance = Instantiate(actorAgentPrefab, GetRandomSpawnPoint(), Quaternion.identity);
        targetInstance = Instantiate(targetPrefab, GetRandomSpawnPoint(), Quaternion.identity);

        Vector3 direction = (targetInstance.transform.position - actorAgentInstance.transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        actorAgentInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        float speed = 10f; 
        float arrivalRadius = 1f;
        float distanceToTarget = Vector3.Distance(actorAgentInstance.transform.position, targetInstance.transform.position);
        if (distanceToTarget > arrivalRadius)
        {
            actorAgentInstance.GetComponent<Rigidbody2D>().velocity = direction * speed;
        }
        else
        {
            // Slow down the agent
            float desiredSpeed = speed * (distanceToTarget / arrivalRadius);
            actorAgentInstance.GetComponent<Rigidbody2D>().velocity = direction * desiredSpeed;
        }
    }

    private void ObstacleAvoidance()
    {
        DestroyInstances();

        // Instantiate actor/agent, target, and obstacle
        Vector3 agentSpawnPoint = GetRandomSpawnPoint();
        Vector3 targetSpawnPoint = GetRandomSpawnPoint();
        Vector3 obstacleSpawnPoint = GetRandomSpawnPoint();

        actorAgentInstance = Instantiate(actorAgentPrefab, agentSpawnPoint, Quaternion.identity);
        targetInstance = Instantiate(targetPrefab, targetSpawnPoint, Quaternion.identity);
        obstacleInstance = Instantiate(obstaclePrefab, obstacleSpawnPoint, Quaternion.identity);

     
        Vector3 targetDirection = (targetInstance.transform.position - actorAgentInstance.transform.position).normalized;

      
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        actorAgentInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Implement obstacle avoidance logic
        Vector3 obstacleDirection = (obstacleInstance.transform.position - actorAgentInstance.transform.position).normalized;
        float obstacleDistance = Vector3.Distance(actorAgentInstance.transform.position, obstacleInstance.transform.position);
        float avoidanceRadius = 2f; 
        if (obstacleDistance < avoidanceRadius)
        {
            Vector3 avoidanceDirection = new Vector3(-obstacleDirection.y, obstacleDirection.x, 0f);

            targetDirection = (targetDirection + avoidanceDirection).normalized;
        }

        // Speed
        float speed = 10f;

        
        actorAgentInstance.GetComponent<Rigidbody2D>().velocity = targetDirection * speed;
    }

    private void ResetScene()
    {
        DestroyInstances();
    }

    private Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(minBounds.x, maxBounds.x), Random.Range(minBounds.y, maxBounds.y), 0f);
    }

    private void DestroyInstances()
    {
        // Destroy previous instances if they exist
        if (actorAgentInstance != null)
            Destroy(actorAgentInstance);
        if (targetInstance != null)
            Destroy(targetInstance);
        if (obstacleInstance != null)
            Destroy(obstacleInstance);
    }

    private void PlaySoundEffect(AudioClip clip)
    {
        if (clip != null && soundEffectPlayer != null)
        {
            soundEffectPlayer.clip = clip;
            soundEffectPlayer.Play();
        }
    }
}
