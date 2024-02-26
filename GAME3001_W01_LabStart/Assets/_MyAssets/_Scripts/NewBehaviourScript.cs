using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Transform island;
    [SerializeField]
    private float cloaseDistance = 2.0f;
    private Vector3 targetPosition=Vector3.zero;
    private float closeDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            LookAt2D(targetPosition);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed *Time.deltaTime);
        

        float distanceToIsland = Vector3.Distance(transform.position, island.position);
        if (distanceToIsland < closeDistance)
        {
            ChangeToThirdScene();
        }
    }

    void LookAt2D(Vector3 target)
    {
        Vector3 LookDirection = target - transform.position;
        float angle = Mathf.Atan2(LookDirection.y, LookDirection.x) + Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void ChangeToThirdScene()
    {
        SceneLoader.LoadSceneByIndex(2);
    }
}
