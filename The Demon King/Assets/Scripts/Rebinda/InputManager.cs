using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static CharacterInputs inputActions;
    public static event Action rebindComplete;
    public static event Action rebindCanceled;
    public static event Action<InputAction, int> rebindStarted;

    //private List<InputBinding>
    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new CharacterInputs();
        }
        foreach (var action in inputActions)
        {
            LoadBindingOverride(action.name);
        }
    }
    public static void StartRebind(string actionName, int bindingIndex, TMP_Text statusText, bool excludeMouse)
    {
        InputAction action = inputActions.asset.FindAction(actionName);

        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Couldn't find action or binding");
            return;
        }
        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isComposite)
            {
                OnRebind(action, bindingIndex, statusText, true, excludeMouse);
            }
        }
        else
        {
            OnRebind(action, bindingIndex, statusText, false, excludeMouse);
        }
    }

    private static void OnRebind(InputAction actionToRebind, int bindingIndex, TMP_Text statusText, bool allCompositeParts, bool excludeMouse)
    {
        if (actionToRebind == null || bindingIndex < 0)
        {
            return;
        }
        statusText.text = $"Press a{ actionToRebind.expectedControlType}";
        actionToRebind.Disable();
        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);

        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable();
            if (allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isComposite)
                {
                    OnRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse);
                }
            }
            SaveBindingOverride(actionToRebind);
            operation.Dispose();

            rebindComplete?.Invoke();


        });

        rebind.OnCancel(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();
            rebindCanceled?.Invoke();

        });

        rebind.WithCancelingThrough("<Mouse/Keyboard/escape>");

        if (excludeMouse)
        {
            rebind.WithControlsExcluding("Mouse");
        }
        rebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start();
    }
    public static string GetBindingName(string actionName, int bindingIndex)
    {
        if (inputActions == null)
        {
            inputActions = new CharacterInputs();
        }
        InputAction action = inputActions.asset.FindAction(actionName);
        return action.GetBindingDisplayString(bindingIndex);
    }
    private static void SaveBindingOverride(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }
    public static void LoadBindingOverride(string actionName)
    {
        if (inputActions == null)
        {
            inputActions = new CharacterInputs();
        }
        InputAction action = inputActions.asset.FindAction(actionName);

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
            {
                action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
            }
        }
    }

    public static void ResetBinding(string actionName, int bindingIndex)
    {
        InputAction action = inputActions.asset.FindAction(actionName);
        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = 0; i < action.bindings.Count && action.bindings[i].isComposite; i++)
            {
                action.RemoveBindingOverride(i);
            }
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
        }

        SaveBindingOverride(action);
    }

    public static void ResetBindings()
    {

        foreach (var actionName in inputActions)
        {
            InputAction action = inputActions.asset.FindAction(actionName.name);

            for (int i = 0; i != action.bindings.Count; i++)
            {
                if (action.bindings[i].isComposite)
                {
                    for (int j = 0; j < action.bindings.Count && action.bindings[j].isComposite; j++)
                    {
                        action.RemoveBindingOverride(j);
                    }
                }
                else
                {
                    action.RemoveBindingOverride(i);
                }

                SaveBindingOverride(action);
            }
        }
    
    }
    private void CheckOtherActionBindings()
    {
        foreach (var actionName in inputActions)
        {
            InputAction action = inputActions.asset.FindAction(actionName.name);

            for (int i = 0; i != action.bindings.Count; i++)
            {
                if (action.bindings[i].isComposite)
                {
                    for (int j = 0; j < action.bindings.Count && action.bindings[j].isComposite; j++)
                    {
                       
                    }
                }
                else
                {
                }

                SaveBindingOverride(action);
            }
        }
    }

}