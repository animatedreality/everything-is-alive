using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Samples;
using System.Threading.Tasks;

public class InputManager : MonoBehaviour
{
    bool rightControllerBButton, rightControllerAButton;
    bool leftControllerXButton;
    bool isSpawningCreature = false;

    private bool _inputEnabled = false;

    void Start()
    {
        if (XRSessionValidator.Instance != null)
        {
            XRSessionValidator.Instance.WaitForXRReady(OnXRReady);
        }
        else
        {
            Debug.LogWarning("[InputManager] XRSessionValidator not found. Input may not work correctly.");
            _inputEnabled = true;
        }
    }

    private void OnXRReady()
    {
        Debug.Log("[InputManager] XR session ready. Enabling input.");
        _inputEnabled = true;
    }

    void Update()
    {
        if (!_inputEnabled)
            return;

        rightControllerBButton = OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch);
        rightControllerAButton = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch);
        leftControllerXButton = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch);

        if (rightControllerBButton)
        {
            StartCoroutine(RightControllerBButtonCoroutine());
        }
        if (rightControllerAButton)
        {
            RightControllerAButton();
        }
        if (leftControllerXButton)
        {
            LeftControllerXButton();
        }
    }

    private IEnumerator RightControllerBButtonCoroutine()
    {
        if (isSpawningCreature)
        {
            yield break;
        }
        isSpawningCreature = true;
        Debug.Log("RightControllerBButton");
        if (GameSceneManager.i.currentSceneState == SceneState.INGAME)
        {
            var task = CreatureManager.i.SpawnCreatureAsync();
            while (!task.IsCompleted)
                yield return null;
        }
        isSpawningCreature = false;
    }

    void RightControllerAButton()
    {
        Debug.Log("RightControllerAButton");
        UIManager.i.ToggleGameplayHint();
    }

    void LeftControllerXButton()
    {
        Debug.Log("LeftControllerXButton");
        UIManager.i.ToggleMainMenu();
    }
}
