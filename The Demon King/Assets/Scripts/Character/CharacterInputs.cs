// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Character/CharacterInputs.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @CharacterInputs : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @CharacterInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CharacterInputs"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""096ffd2a-b597-4ac2-bd94-373c67992f7f"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""cdfa34cf-a71a-407f-80dc-26e6758d6fa6"",
                    ""expectedControlType"": ""Vector2"",
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
                },
                {
                    ""name"": ""Ability1"",
                    ""type"": ""Button"",
                    ""id"": ""44bf9536-eccd-429e-9d81-516bcb994b2f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ability2"",
                    ""type"": ""Button"",
                    ""id"": ""73a8c356-bb7d-4f82-b35e-91f77d984f04"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""9f54eec8-8f50-473d-ac9e-b3d57880dfd9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Evolve"",
                    ""type"": ""Button"",
                    ""id"": ""13ef0cca-1f95-4722-9cd3-b2a15125191b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Devolve"",
                    ""type"": ""Button"",
                    ""id"": ""94f7094b-d82d-4e2f-a1fa-5ad69650bc72"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
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
                },
                {
                    ""name"": """",
                    ""id"": ""43a40432-dcaf-439f-9987-e211ca9bc4ec"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ability2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e5edd999-9aac-48df-bfa1-db28a9423971"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c231873-4597-452d-bb90-39d2e4e81156"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ability1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""07254b62-9c89-4b98-bbb4-10c2e4c8a076"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Evolve"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""358c9967-01b2-4f29-abd1-5a815e4c5f15"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Devolve"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerLook"",
            ""id"": ""1506ea37-ee2f-4a08-8afb-13ac23dee6cd"",
            ""actions"": [
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""f712900d-8bd1-47e9-b16d-87274ef56271"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3405cb66-5f89-4e0d-b01a-330bb7ec7f75"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""InvertVector2(invertX=false)"",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""DisplayScoreBoard"",
            ""id"": ""6faad154-8540-4496-b2c7-0627ded36c00"",
            ""actions"": [
                {
                    ""name"": ""DisplayScoreBoard"",
                    ""type"": ""Button"",
                    ""id"": ""c7dd31d6-ead3-4559-a1dc-2583b8f0fce6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""da02979f-1459-4154-9735-8d233f1181f5"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DisplayScoreBoard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Interact = m_Player.FindAction("Interact", throwIfNotFound: true);
        m_Player_Ability1 = m_Player.FindAction("Ability1", throwIfNotFound: true);
        m_Player_Ability2 = m_Player.FindAction("Ability2", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Evolve = m_Player.FindAction("Evolve", throwIfNotFound: true);
        m_Player_Devolve = m_Player.FindAction("Devolve", throwIfNotFound: true);
        // PlayerLook
        m_PlayerLook = asset.FindActionMap("PlayerLook", throwIfNotFound: true);
        m_PlayerLook_Look = m_PlayerLook.FindAction("Look", throwIfNotFound: true);
        // DisplayScoreBoard
        m_DisplayScoreBoard = asset.FindActionMap("DisplayScoreBoard", throwIfNotFound: true);
        m_DisplayScoreBoard_DisplayScoreBoard = m_DisplayScoreBoard.FindAction("DisplayScoreBoard", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Interact;
    private readonly InputAction m_Player_Ability1;
    private readonly InputAction m_Player_Ability2;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Evolve;
    private readonly InputAction m_Player_Devolve;
    public struct PlayerActions
    {
        private @CharacterInputs m_Wrapper;
        public PlayerActions(@CharacterInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Interact => m_Wrapper.m_Player_Interact;
        public InputAction @Ability1 => m_Wrapper.m_Player_Ability1;
        public InputAction @Ability2 => m_Wrapper.m_Player_Ability2;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Evolve => m_Wrapper.m_Player_Evolve;
        public InputAction @Devolve => m_Wrapper.m_Player_Devolve;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Interact.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                @Ability1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility1;
                @Ability1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility1;
                @Ability1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility1;
                @Ability2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility2;
                @Ability2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility2;
                @Ability2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility2;
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Evolve.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEvolve;
                @Evolve.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEvolve;
                @Evolve.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEvolve;
                @Devolve.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDevolve;
                @Devolve.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDevolve;
                @Devolve.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDevolve;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Ability1.started += instance.OnAbility1;
                @Ability1.performed += instance.OnAbility1;
                @Ability1.canceled += instance.OnAbility1;
                @Ability2.started += instance.OnAbility2;
                @Ability2.performed += instance.OnAbility2;
                @Ability2.canceled += instance.OnAbility2;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Evolve.started += instance.OnEvolve;
                @Evolve.performed += instance.OnEvolve;
                @Evolve.canceled += instance.OnEvolve;
                @Devolve.started += instance.OnDevolve;
                @Devolve.performed += instance.OnDevolve;
                @Devolve.canceled += instance.OnDevolve;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // PlayerLook
    private readonly InputActionMap m_PlayerLook;
    private IPlayerLookActions m_PlayerLookActionsCallbackInterface;
    private readonly InputAction m_PlayerLook_Look;
    public struct PlayerLookActions
    {
        private @CharacterInputs m_Wrapper;
        public PlayerLookActions(@CharacterInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Look => m_Wrapper.m_PlayerLook_Look;
        public InputActionMap Get() { return m_Wrapper.m_PlayerLook; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerLookActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerLookActions instance)
        {
            if (m_Wrapper.m_PlayerLookActionsCallbackInterface != null)
            {
                @Look.started -= m_Wrapper.m_PlayerLookActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_PlayerLookActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_PlayerLookActionsCallbackInterface.OnLook;
            }
            m_Wrapper.m_PlayerLookActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
            }
        }
    }
    public PlayerLookActions @PlayerLook => new PlayerLookActions(this);

    // DisplayScoreBoard
    private readonly InputActionMap m_DisplayScoreBoard;
    private IDisplayScoreBoardActions m_DisplayScoreBoardActionsCallbackInterface;
    private readonly InputAction m_DisplayScoreBoard_DisplayScoreBoard;
    public struct DisplayScoreBoardActions
    {
        private @CharacterInputs m_Wrapper;
        public DisplayScoreBoardActions(@CharacterInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @DisplayScoreBoard => m_Wrapper.m_DisplayScoreBoard_DisplayScoreBoard;
        public InputActionMap Get() { return m_Wrapper.m_DisplayScoreBoard; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DisplayScoreBoardActions set) { return set.Get(); }
        public void SetCallbacks(IDisplayScoreBoardActions instance)
        {
            if (m_Wrapper.m_DisplayScoreBoardActionsCallbackInterface != null)
            {
                @DisplayScoreBoard.started -= m_Wrapper.m_DisplayScoreBoardActionsCallbackInterface.OnDisplayScoreBoard;
                @DisplayScoreBoard.performed -= m_Wrapper.m_DisplayScoreBoardActionsCallbackInterface.OnDisplayScoreBoard;
                @DisplayScoreBoard.canceled -= m_Wrapper.m_DisplayScoreBoardActionsCallbackInterface.OnDisplayScoreBoard;
            }
            m_Wrapper.m_DisplayScoreBoardActionsCallbackInterface = instance;
            if (instance != null)
            {
                @DisplayScoreBoard.started += instance.OnDisplayScoreBoard;
                @DisplayScoreBoard.performed += instance.OnDisplayScoreBoard;
                @DisplayScoreBoard.canceled += instance.OnDisplayScoreBoard;
            }
        }
    }
    public DisplayScoreBoardActions @DisplayScoreBoard => new DisplayScoreBoardActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnAbility1(InputAction.CallbackContext context);
        void OnAbility2(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnEvolve(InputAction.CallbackContext context);
        void OnDevolve(InputAction.CallbackContext context);
    }
    public interface IPlayerLookActions
    {
        void OnLook(InputAction.CallbackContext context);
    }
    public interface IDisplayScoreBoardActions
    {
        void OnDisplayScoreBoard(InputAction.CallbackContext context);
    }
}
