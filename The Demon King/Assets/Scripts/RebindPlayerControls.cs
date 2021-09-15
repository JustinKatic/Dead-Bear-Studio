using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindPlayerControls : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpAction = null;
    [SerializeField] private InputActionReference evolveAction = null;
    [SerializeField] private InputActionReference emoteAction = null;
    [SerializeField] private InputActionReference openMenuAction = null;
    [SerializeField] private InputActionReference devourAction = null;
    [SerializeField] private InputActionReference shootAction = null;


    public InputActionAsset cc;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    
    private const string RebindsKey = "rebinds";

    // Start is called before the first frame update
    void Start()
    {

        string rebinds = PlayerPrefs.GetString(RebindsKey, string.Empty);

        if (string.IsNullOrEmpty(rebinds)) { return; }

        cc.LoadFromJson(rebinds);
        
    }
    public void Save()
    {
        string rebinds = cc.ToJson();

       PlayerPrefs.SetString(RebindsKey, rebinds);
    }
    public void StartRebinding(InputActionReference inputAction)
    {
        rebindingOperation = inputAction.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete(inputAction))
            .Start();
    }

    private void RebindComplete(InputActionReference inputAction)
    {
        int bindingIndex = inputAction.action.GetBindingIndexForControl(inputAction.action.controls[0]);

        rebindingOperation.Dispose();
    }
}
