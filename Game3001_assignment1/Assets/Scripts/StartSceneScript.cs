using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneScript : MonoBehaviour
{
    public Text text; // Reference to the Text component
    public Button startButton; // Reference to the Button component
    public AudioSource backgroundMusic; // Reference to the AudioSource for background music

    void Start()
    {
        // Add listener to the button
        startButton.onClick.AddListener(ChangeGameScene);
    }

    public void ChangeGameScene()
    {
        // Play the background music if assigned
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
            // Ensure that the background music persists across scene changes
            DontDestroyOnLoad(backgroundMusic.gameObject);
        }

        // Destroy assigned text and button
        Destroy(text.gameObject);
        Destroy(startButton.gameObject);

        // Load "Play" Scene
        SceneManager.LoadScene("PlayScene");
    }
}
