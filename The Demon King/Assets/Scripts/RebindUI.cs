using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindUI : MonoBehaviour
{
    [SerializeField]
    InputActionReference inputActionReference;

    [SerializeField]
    private bool excludeMouse = true;

    [Range(0, 10)]
    [SerializeField]
    private int selectedBinding;

    [SerializeField]
    private InputBinding.DisplayStringOptions displayStringOptions;

    [Header("Binding Info")]
    [SerializeField]
    private InputBinding inputBinding;

    private int bindingIndex;

    private string actionName;

    [Header("UI Fields")]
    [SerializeField]
    private TMP_Text actionText;
    [SerializeField]
    private Button rebindButton;
    [SerializeField]
    private TMP_Text rebindText;
    [SerializeField]
    private Button resetButton;

    private void OnEnable()
    {
        rebindButton.onClick.AddListener(() => DoRebind());
        resetButton.onClick.AddListener(() => ResetBinding());
        if (inputActionReference != null)
        {
            InputManager.LoadBindingOverride(actionName);
            GetBindingInfo();
            UpdateUI();
        }
        InputManager.rebindComplete += UpdateUI;
        InputManager.rebindCanceled += UpdateUI;

    }
    private void OnDisable()
    {
        InputManager.rebindComplete -= UpdateUI;
        InputManager.rebindCanceled -= UpdateUI;
    }

    private void ResetBinding()
    {
        UpdateUI();
    }

    private void DoRebind()
    {
        InputManager.StartRebind(actionName, bindingIndex, rebindText, excludeMouse);
    }

    private void OnValidate()
    {
        if (inputActionReference == null)
        {
            return;
        }
        GetBindingInfo();
        UpdateUI();
    }

    private void GetBindingInfo()
    {
        if (inputActionReference.action != null)
            actionName = inputActionReference.action.name;
        if (inputActionReference.action.bindings.Count > selectedBinding)
        {
            inputBinding = inputActionReference.action.bindings[selectedBinding];
            bindingIndex = selectedBinding;
        }

    }
    void UpdateUI()
    {
        if (actionText != null)
        {
            actionText.text = actionName;
        }

        if (Application.isPlaying)
        {
            rebindText.text = InputManager.GetBindingName(actionName, bindingIndex);
        }
        else
        {
            rebindText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex);
        }
    }
}
