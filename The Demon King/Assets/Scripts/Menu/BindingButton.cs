using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BindingButton : MonoBehaviour
{
    [HideInInspector] public TMP_Text bindingButton;
    [HideInInspector] public int bindingIndex;
    public InputActionReference inputAction;
    // Start is called before the first frame update
    void Start()
    {
       bindingIndex = inputAction.action.GetBindingIndexForControl(inputAction.action.controls[0]);

       bindingButton = GetComponentInChildren<TMP_Text>();
        
       bindingButton.text = InputControlPath.ToHumanReadableString(
           inputAction.action.bindings[bindingIndex].effectivePath,
           InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    public void UpdateButtonTxt()
    {
        bindingButton.text = InputControlPath.ToHumanReadableString(
            inputAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
}
