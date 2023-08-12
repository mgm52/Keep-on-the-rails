using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickForNextScene : MonoBehaviour
{
    public string nextSceneName;
    public AudioClip exitSound;
    public AudioClip entrySound;

    private AudioSource audioSource;
    
    public float delayUntilClickableS = 0f;

    private float startTime;

    public void Start()
    {
        if (entrySound != null || exitSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();

            if (entrySound != null)
            {
                audioSource.PlayOneShot(entrySound);
            }
        }
        startTime = Time.time;
    }

    // Detect if user presses "r", "a", "i", or "l"
    public void Update()
    {
        if (Time.time - startTime < delayUntilClickableS)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.L))
        {
            if (exitSound != null)
            {
                audioSource.PlayOneShot(exitSound);
            }
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
