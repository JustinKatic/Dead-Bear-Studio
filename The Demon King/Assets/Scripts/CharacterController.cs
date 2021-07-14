// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/CharacterController.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @CharacterController : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @CharacterController()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CharacterController"",
    ""maps"": [
        {
            ""name"": ""Character Inputs"",
            ""id"": ""096ffd2a-b597-4ac2-bd94-373c67992f7f"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""cdfa34cf-a71a-407f-80dc-26e6758d6fa6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""44bf9536-eccd-429e-9d81-516bcb994b2f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""b8aa9b9b-2bf8-4953-9193-6e5dc5d42b9b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2c231873-4597-452d-bb90-39d2e4e81156"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e9da84b3-841e-4001-ba0f-1b252b17cf81"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Walk"",
                    ""id"": ""6c746106-addd-44c2-8e7a-6c442b414e7c"",
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
                    ""id"": ""32c35042-8984-4945-911f-bf2b4d7adc9c"",
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
                    ""id"": ""6710d672-62ed-4672-93a5-93aac45f14e5"",
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
                    ""id"": ""6d3f6d08-97c3-4658-a7e6-52b636a9b44e"",
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
                    ""id"": ""cbe84efd-5e5f-4038-8eac-44f9756e8436"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Character Inputs
        m_CharacterInputs = asset.FindActionMap("Character Inputs", throwIfNotFound: true);
        m_CharacterInputs_Move = m_CharacterInputs.FindAction("Move", throwIfNotFound: true);
        m_CharacterInputs_Shoot = m_CharacterInputs.FindAction("Shoot", throwIfNotFound: true);
        m_CharacterInputs_Interact = m_CharacterInputs.FindAction("Interact", throwIfNotFound: true);
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

    // Character Inputs
    private readonly InputActionMap m_CharacterInputs;
    private ICharacterInputsActions m_CharacterInputsActionsCallbackInterface;
    private readonly InputAction m_CharacterInputs_Move;
    private readonly InputAction m_CharacterInputs_Shoot;
    private readonly InputAction m_CharacterInputs_Interact;
    public struct CharacterInputsActions
    {
        private @CharacterController m_Wrapper;
        public CharacterInputsActions(@CharacterController wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_CharacterInputs_Move;
        public InputAction @Shoot => m_Wrapper.m_CharacterInputs_Shoot;
        public InputAction @Interact => m_Wrapper.m_CharacterInputs_Interact;
        public InputActionMap Get() { return m_Wrapper.m_CharacterInputs; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CharacterInputsActions set) { return set.Get(); }
        public void SetCallbacks(ICharacterInputsActions instance)
        {
            if (m_Wrapper.m_CharacterInputsActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_CharacterInputsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_CharacterInputsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_CharacterInputsActionsCallbackInterface.OnMove;
                @Shoot.started -= m_Wrapper.m_CharacterInputsActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_CharacterInputsActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_CharacterInputsActionsCallbackInterface.OnShoot;
                @Interact.started -= m_Wrapper.m_CharacterInputsActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_CharacterInputsActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_CharacterInputsActionsCallbackInterface.OnInteract;
            }
            m_Wrapper.m_CharacterInputsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
            }
        }
    }
    public CharacterInputsActions @CharacterInputs => new CharacterInputsActions(this);
    public interface ICharacterInputsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
    }
}
