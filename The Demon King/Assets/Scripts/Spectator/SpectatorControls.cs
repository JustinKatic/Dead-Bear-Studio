// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Spectator/SpectatorControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @SpectatorControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @SpectatorControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""SpectatorControls"",
    ""maps"": [
        {
            ""name"": ""Movement"",
            ""id"": ""65212b11-0f0d-4e61-86a1-7e7c0140613a"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""36c5c6b3-f903-4ee1-973c-fc70a1be3b93"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""PassThrough"",
                    ""id"": ""b0636364-c60e-4d4b-9c05-80975123f7de"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Increase Speed"",
                    ""type"": ""Button"",
                    ""id"": ""8a8aec95-6b82-440b-8251-2457154db04d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Raise"",
                    ""type"": ""Value"",
                    ""id"": ""40aa6d33-3756-4f1c-a8bc-0b386c610ea9"",
                    ""expectedControlType"": ""Integer"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Lower"",
                    ""type"": ""Value"",
                    ""id"": ""bdb35713-aea1-4f8a-81ba-2c9f1ca7bb6d"",
                    ""expectedControlType"": ""Integer"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Position"",
                    ""id"": ""012c66f1-7269-4e39-8618-8adb2d06b632"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f1e5590e-5276-4ca6-aded-8c5ba5c7be9e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""6fb23e1e-4920-4b2a-a772-456f61ce7a6a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""50ba8745-b2df-4314-946e-478280e463f1"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9f52583d-2766-4aa9-bc8b-e8253ec7ba57"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""c6b5fc7c-e114-40c1-8ee7-debcfe552604"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""InvertVector2(invertX=false)"",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5f9ae2ed-cf11-484b-b7fc-53925542310c"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Increase Speed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""595e9fa9-8e16-407e-b57e-e0700fdda508"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Raise"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c929f440-3ffc-4b60-9af4-956977f99f0c"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Lower"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Movement
        m_Movement = asset.FindActionMap("Movement", throwIfNotFound: true);
        m_Movement_Move = m_Movement.FindAction("Move", throwIfNotFound: true);
        m_Movement_Rotate = m_Movement.FindAction("Rotate", throwIfNotFound: true);
        m_Movement_IncreaseSpeed = m_Movement.FindAction("Increase Speed", throwIfNotFound: true);
        m_Movement_Raise = m_Movement.FindAction("Raise", throwIfNotFound: true);
        m_Movement_Lower = m_Movement.FindAction("Lower", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Movement
    private readonly InputActionMap m_Movement;
    private IMovementActions m_MovementActionsCallbackInterface;
    private readonly InputAction m_Movement_Move;
    private readonly InputAction m_Movement_Rotate;
    private readonly InputAction m_Movement_IncreaseSpeed;
    private readonly InputAction m_Movement_Raise;
    private readonly InputAction m_Movement_Lower;
    public struct MovementActions
    {
        private @SpectatorControls m_Wrapper;
        public MovementActions(@SpectatorControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Movement_Move;
        public InputAction @Rotate => m_Wrapper.m_Movement_Rotate;
        public InputAction @IncreaseSpeed => m_Wrapper.m_Movement_IncreaseSpeed;
        public InputAction @Raise => m_Wrapper.m_Movement_Raise;
        public InputAction @Lower => m_Wrapper.m_Movement_Lower;
        public InputActionMap Get() { return m_Wrapper.m_Movement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MovementActions set) { return set.Get(); }
        public void SetCallbacks(IMovementActions instance)
        {
            if (m_Wrapper.m_MovementActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnMove;
                @Rotate.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnRotate;
                @IncreaseSpeed.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnIncreaseSpeed;
                @IncreaseSpeed.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnIncreaseSpeed;
                @IncreaseSpeed.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnIncreaseSpeed;
                @Raise.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnRaise;
                @Raise.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnRaise;
                @Raise.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnRaise;
                @Lower.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnLower;
                @Lower.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnLower;
                @Lower.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnLower;
            }
            m_Wrapper.m_MovementActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @IncreaseSpeed.started += instance.OnIncreaseSpeed;
                @IncreaseSpeed.performed += instance.OnIncreaseSpeed;
                @IncreaseSpeed.canceled += instance.OnIncreaseSpeed;
                @Raise.started += instance.OnRaise;
                @Raise.performed += instance.OnRaise;
                @Raise.canceled += instance.OnRaise;
                @Lower.started += instance.OnLower;
                @Lower.performed += instance.OnLower;
                @Lower.canceled += instance.OnLower;
            }
        }
    }
    public MovementActions @Movement => new MovementActions(this);
    public interface IMovementActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
        void OnIncreaseSpeed(InputAction.CallbackContext context);
        void OnRaise(InputAction.CallbackContext context);
        void OnLower(InputAction.CallbackContext context);
    }
}
