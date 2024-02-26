using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentScript : MonoBehaviour
{
    [SerializeField] Camera m_maincamera;
    [SerializeField] Vector3 m_targetPosition;
    [SerializeField] float m_movespeed = 2.5f;

    public static AgentScript s_instance;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            m_targetPosition = m_maincamera.ScreenToWorldPoint(Input.mousePosition);
            m_targetPosition.z = 0;
            LookAt2D(m_targetPosition);
        }

        transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, m_movespeed * Time.deltaTime);
        
    }

    void LookAt2D(Vector3 target)
    {
        Vector3 LookDirection = target - transform.position;
        float angle = Mathf.Atan2(LookDirection.y, LookDirection.x) + Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
