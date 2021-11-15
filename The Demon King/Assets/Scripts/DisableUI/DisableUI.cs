using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableUI : MonoBehaviour
{
    public GameObject UIToToggle;


    private void Start()
    {
        InputManager.inputActions.DevShortCuts.ToggleUI.started += ToggleUI_performed;
    }

    private void ToggleUI_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (UIToToggle != null)
            UIToToggle.SetActive(!UIToToggle.activeSelf);
    }

    private void OnEnable()
    {
        InputManager.inputActions.DevShortCuts.Enable();
    }

    private void OnDestroy()
    {
        InputManager.inputActions.DevShortCuts.Disable();
        InputManager.inputActions.DevShortCuts.ToggleUI.started -= ToggleUI_performed;

    }
}
