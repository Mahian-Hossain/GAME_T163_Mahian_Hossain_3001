using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject targetPrefab;
    public GameObject agentPrefab;
    public GameObject obstaclePrefab;
    public AudioSource music;
    public GameObject instructionText;

    private GameObject target;
    private GameObject agent;
    private GameObject obstacle;

    private Vector2 screenBounds;

    void Start()
    {
        screenBounds = new Vector2(5.6f, 4.2f);
        music.Play();
        instructionText.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartSeeking();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartFleeing();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartArrival();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartObstacleAvoidance();
            
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ClearScene();
        }
    }

    void StartSeeking()
    {
        ClearScene();
        instructionText.SetActive(false);
        target = Instantiate(targetPrefab, GetRandomPosition(), Quaternion.identity);
        agent = Instantiate(agentPrefab, GetRandomEdgePosition(), Quaternion.identity);
        agent.transform.up = agent.transform.position - target.transform.position;
    }

    void StartFleeing()
    {
        ClearScene();
        instructionText.SetActive(false);
        target = Instantiate(targetPrefab, GetRandomPosition(), Quaternion.identity);
        agent = Instantiate(agentPrefab, GetRandomPosition(), Quaternion.identity);
        agent.transform.up = agent.transform.position - target.transform.position;
    }

    void StartArrival()
    {
        ClearScene();
        instructionText.SetActive(false);
        target = Instantiate(targetPrefab, GetRandomPosition(), Quaternion.identity);
        agent = Instantiate(agentPrefab, GetRandomEdgePosition(), Quaternion.identity);
        Starship starship = agent.GetComponent<Starship>();
        starship.TargetPosition = target.transform.position;
        starship.IsArrival = true;
    }

    void StartObstacleAvoidance()
    {
        ClearScene();
        instructionText.SetActive(false);
        target = Instantiate(targetPrefab, GetRandomPosition(), Quaternion.identity);
        agent = Instantiate(agentPrefab, GetRandomEdgePosition(), Quaternion.identity);
         obstacle = Instantiate(obstaclePrefab, GetRandomEdgePosition(), Quaternion.identity);
 
        Starship starship = agent.GetComponent<Starship>();
        starship.TargetPosition = target.transform.position;
        starship.IsArrival = true;
    }

    void ClearScene()
    {
        instructionText.SetActive(true);
        if (target != null)
            Destroy(target);
        if (agent != null)
            Destroy(agent);
        if (obstacle != null)
            Destroy(obstacle);
    }

    Vector2 GetRandomPosition()
    {
        return new Vector2(Random.Range(-screenBounds.x, screenBounds.x), Random.Range(-screenBounds.y, screenBounds.y));
    }

    Vector2 GetRandomEdgePosition()
    {
        int side = Random.Range(0, 4); // 0: Top, 1: Right, 2: Bottom, 3: Left
        float x = 0f, y = 0f;

        switch (side)
        {
            case 0:
                x = Random.Range(-screenBounds.x, screenBounds.x);
                y = screenBounds.y;
                break;
            case 1:
                x = screenBounds.x;
                y = Random.Range(-screenBounds.y, screenBounds.y);
                break;
            case 2:
                x = Random.Range(-screenBounds.x, screenBounds.x);
                y = -screenBounds.y;
                break;
            case 3:
                x = -screenBounds.x;
                y = Random.Range(-screenBounds.y, screenBounds.y);
                break;
        }

        return new Vector2(x, y);
    }
}
