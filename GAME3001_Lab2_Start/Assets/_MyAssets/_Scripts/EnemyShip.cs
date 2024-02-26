using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyShip : MonoBehaviour
{
    public float speed = 2.0f; // Adjust the speed as needed
    private bool hasStartedMovement = false;
    


    

    void Update()
    {

       
        if (!hasStartedMovement)
        {
            // Rotate the enemy ship by 180 degrees around the z-axis
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            hasStartedMovement = true;
        }
        else
        {
            // Move the enemy ship downwards
            Vector3 movement = new Vector3(0, speed * Time.deltaTime, 0);
            transform.Translate(movement);

            // Check if the enemy ship is out of the screen bounds
            if (transform.position.y < -6.3f)
            {
                // Destroy the enemy ship if it goes out of bounds
                Destroy(gameObject);
            }
        }


       
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object is the player's ship
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        

        }
    }

   
}
