using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneScript : MonoBehaviour
{
    public string playScene;
    public AudioSource audioSource;

    void Start()
    {
        DontDestroyOnLoad(audioSource);
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayAudioContinuously()
    {
        // If the audio source is not playing, start playing it
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}