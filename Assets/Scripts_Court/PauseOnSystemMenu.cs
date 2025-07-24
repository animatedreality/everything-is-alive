using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseOnSystemMenu : MonoBehaviour
{
    void OnEnable()
    {
        Application.focusChanged += OnFocusChanged;
    }

    void OnDisable()
    {
        Application.focusChanged -= OnFocusChanged;
    }

    void OnFocusChanged(bool hasFocus)
    {
        if (!hasFocus)
        {
            PauseApp();
        }
        else
        {
            ResumeApp();
        }
    }

    void PauseApp()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Debug.Log("App paused due to system menu or HMD removal.");
        // You can broadcast to other systems or show a pause UI here
    }

    void ResumeApp()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Debug.Log("App resumed.");
        // Resume audio, animation, etc.
    }
}
