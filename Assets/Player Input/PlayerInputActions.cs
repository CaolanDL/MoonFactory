//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Player Input/PlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""Default Controls"",
            ""id"": ""78cb8385-15cf-4a77-9bdc-d3c4c6b8ca46"",
            ""actions"": [
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""dd69b364-8546-4a05-82f6-5bc8c5a0539b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Exit Tool"",
                    ""type"": ""Button"",
                    ""id"": ""4526ccdb-3834-4023-beb5-94df31c8a2bf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pick"",
                    ""type"": ""Button"",
                    ""id"": ""c49dbca6-541e-4d6e-9518-3cb4ba26ec48"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Button"",
                    ""id"": ""c4439b84-9728-4292-b1b1-909980ef47b3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b921ba43-5ddb-4f26-ac88-83358cf2a250"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1044a024-c187-4dea-b6bc-2cbb88aa5c1e"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Exit Tool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""One Modifier"",
                    ""id"": ""86050476-2a1d-44c0-8ded-f36592e7a82d"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pick"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""c3eda2fa-8d62-4572-b6e2-4f08eab30242"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""8b49c0c9-58b6-4967-930f-16a35ccc4262"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""d621d752-b665-43ae-90e4-852a03bfbb85"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Camera Controls"",
            ""id"": ""53f65ddb-9ea1-43b4-bcb1-3e8ac133ddcb"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""60dce67f-8746-4f3b-9ad2-373fb98290c0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""56337558-7f91-4522-b2a6-9568feaaa3ae"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": ""Clamp(min=-1,max=1),Invert"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""688dd8ce-aba1-4a02-8f3b-84a6f4ea7248"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Double Touch Slide [Touch]"",
                    ""id"": ""132453fe-0acd-45b1-afa2-21621404c07f"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""5e3ce914-6eac-4b6d-b181-890fe273a43f"",
                    ""path"": ""<Touchscreen>/touch1/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""ceb05556-a7a3-4244-88be-bd43e5faf1fd"",
                    ""path"": ""<Touchscreen>/primaryTouch/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""a95cb8af-0169-4477-91a0-7295ffc9c9ee"",
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
                    ""id"": ""cb38617f-e643-4607-8ee6-9c2c338be990"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Desktop"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""eb59a2af-3df5-4313-b6db-07ccb918a041"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Desktop"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""eb62429c-0a5c-4b2b-98d8-a53c69ac67fd"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Desktop"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ab5d9e96-4bc1-400c-bf0a-a5c8329e4340"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Desktop"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""17e1be79-46d0-4eae-808c-d3686580ebd4"",
                    ""path"": ""<Touchscreen>/primaryTouch/delta"",
                    ""interactions"": """",
                    ""processors"": ""InvertVector2"",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI Controls"",
            ""id"": ""5be7f4d8-76a5-4849-8dd0-0d49f29dc029"",
            ""actions"": [
                {
                    ""name"": ""Click"",
                    ""type"": ""Button"",
                    ""id"": ""74b7f3c7-3760-4e97-b678-ed9ec4aa9f96"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Point"",
                    ""type"": ""PassThrough"",
                    ""id"": ""0f84f636-a42d-483a-b046-833d75700c1d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PointDelta"",
                    ""type"": ""PassThrough"",
                    ""id"": ""45a19f9b-4bcb-490a-9874-19fade2dcb81"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4ca38522-9887-4df8-bf08-505f226e0983"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a6d58b8f-a001-417a-a6e2-c8eaf7e7ef82"",
                    ""path"": ""<Touchscreen>/Press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""92d8a17b-a735-4509-a008-123e6006d3ea"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f3966d24-6787-4879-a755-b89091093aea"",
                    ""path"": ""<Touchscreen>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2cacb1c5-f056-4542-83d3-0502ac5ade63"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Desktop"",
            ""bindingGroup"": ""Desktop"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Default Controls
        m_DefaultControls = asset.FindActionMap("Default Controls", throwIfNotFound: true);
        m_DefaultControls_Select = m_DefaultControls.FindAction("Select", throwIfNotFound: true);
        m_DefaultControls_ExitTool = m_DefaultControls.FindAction("Exit Tool", throwIfNotFound: true);
        m_DefaultControls_Pick = m_DefaultControls.FindAction("Pick", throwIfNotFound: true);
        m_DefaultControls_Rotate = m_DefaultControls.FindAction("Rotate", throwIfNotFound: true);
        // Camera Controls
        m_CameraControls = asset.FindActionMap("Camera Controls", throwIfNotFound: true);
        m_CameraControls_Move = m_CameraControls.FindAction("Move", throwIfNotFound: true);
        m_CameraControls_Zoom = m_CameraControls.FindAction("Zoom", throwIfNotFound: true);
        // UI Controls
        m_UIControls = asset.FindActionMap("UI Controls", throwIfNotFound: true);
        m_UIControls_Click = m_UIControls.FindAction("Click", throwIfNotFound: true);
        m_UIControls_Point = m_UIControls.FindAction("Point", throwIfNotFound: true);
        m_UIControls_PointDelta = m_UIControls.FindAction("PointDelta", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Default Controls
    private readonly InputActionMap m_DefaultControls;
    private List<IDefaultControlsActions> m_DefaultControlsActionsCallbackInterfaces = new List<IDefaultControlsActions>();
    private readonly InputAction m_DefaultControls_Select;
    private readonly InputAction m_DefaultControls_ExitTool;
    private readonly InputAction m_DefaultControls_Pick;
    private readonly InputAction m_DefaultControls_Rotate;
    public struct DefaultControlsActions
    {
        private @PlayerInputActions m_Wrapper;
        public DefaultControlsActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Select => m_Wrapper.m_DefaultControls_Select;
        public InputAction @ExitTool => m_Wrapper.m_DefaultControls_ExitTool;
        public InputAction @Pick => m_Wrapper.m_DefaultControls_Pick;
        public InputAction @Rotate => m_Wrapper.m_DefaultControls_Rotate;
        public InputActionMap Get() { return m_Wrapper.m_DefaultControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DefaultControlsActions set) { return set.Get(); }
        public void AddCallbacks(IDefaultControlsActions instance)
        {
            if (instance == null || m_Wrapper.m_DefaultControlsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_DefaultControlsActionsCallbackInterfaces.Add(instance);
            @Select.started += instance.OnSelect;
            @Select.performed += instance.OnSelect;
            @Select.canceled += instance.OnSelect;
            @ExitTool.started += instance.OnExitTool;
            @ExitTool.performed += instance.OnExitTool;
            @ExitTool.canceled += instance.OnExitTool;
            @Pick.started += instance.OnPick;
            @Pick.performed += instance.OnPick;
            @Pick.canceled += instance.OnPick;
            @Rotate.started += instance.OnRotate;
            @Rotate.performed += instance.OnRotate;
            @Rotate.canceled += instance.OnRotate;
        }

        private void UnregisterCallbacks(IDefaultControlsActions instance)
        {
            @Select.started -= instance.OnSelect;
            @Select.performed -= instance.OnSelect;
            @Select.canceled -= instance.OnSelect;
            @ExitTool.started -= instance.OnExitTool;
            @ExitTool.performed -= instance.OnExitTool;
            @ExitTool.canceled -= instance.OnExitTool;
            @Pick.started -= instance.OnPick;
            @Pick.performed -= instance.OnPick;
            @Pick.canceled -= instance.OnPick;
            @Rotate.started -= instance.OnRotate;
            @Rotate.performed -= instance.OnRotate;
            @Rotate.canceled -= instance.OnRotate;
        }

        public void RemoveCallbacks(IDefaultControlsActions instance)
        {
            if (m_Wrapper.m_DefaultControlsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IDefaultControlsActions instance)
        {
            foreach (var item in m_Wrapper.m_DefaultControlsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_DefaultControlsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public DefaultControlsActions @DefaultControls => new DefaultControlsActions(this);

    // Camera Controls
    private readonly InputActionMap m_CameraControls;
    private List<ICameraControlsActions> m_CameraControlsActionsCallbackInterfaces = new List<ICameraControlsActions>();
    private readonly InputAction m_CameraControls_Move;
    private readonly InputAction m_CameraControls_Zoom;
    public struct CameraControlsActions
    {
        private @PlayerInputActions m_Wrapper;
        public CameraControlsActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_CameraControls_Move;
        public InputAction @Zoom => m_Wrapper.m_CameraControls_Zoom;
        public InputActionMap Get() { return m_Wrapper.m_CameraControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraControlsActions set) { return set.Get(); }
        public void AddCallbacks(ICameraControlsActions instance)
        {
            if (instance == null || m_Wrapper.m_CameraControlsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CameraControlsActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Zoom.started += instance.OnZoom;
            @Zoom.performed += instance.OnZoom;
            @Zoom.canceled += instance.OnZoom;
        }

        private void UnregisterCallbacks(ICameraControlsActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Zoom.started -= instance.OnZoom;
            @Zoom.performed -= instance.OnZoom;
            @Zoom.canceled -= instance.OnZoom;
        }

        public void RemoveCallbacks(ICameraControlsActions instance)
        {
            if (m_Wrapper.m_CameraControlsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICameraControlsActions instance)
        {
            foreach (var item in m_Wrapper.m_CameraControlsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CameraControlsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CameraControlsActions @CameraControls => new CameraControlsActions(this);

    // UI Controls
    private readonly InputActionMap m_UIControls;
    private List<IUIControlsActions> m_UIControlsActionsCallbackInterfaces = new List<IUIControlsActions>();
    private readonly InputAction m_UIControls_Click;
    private readonly InputAction m_UIControls_Point;
    private readonly InputAction m_UIControls_PointDelta;
    public struct UIControlsActions
    {
        private @PlayerInputActions m_Wrapper;
        public UIControlsActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Click => m_Wrapper.m_UIControls_Click;
        public InputAction @Point => m_Wrapper.m_UIControls_Point;
        public InputAction @PointDelta => m_Wrapper.m_UIControls_PointDelta;
        public InputActionMap Get() { return m_Wrapper.m_UIControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIControlsActions set) { return set.Get(); }
        public void AddCallbacks(IUIControlsActions instance)
        {
            if (instance == null || m_Wrapper.m_UIControlsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UIControlsActionsCallbackInterfaces.Add(instance);
            @Click.started += instance.OnClick;
            @Click.performed += instance.OnClick;
            @Click.canceled += instance.OnClick;
            @Point.started += instance.OnPoint;
            @Point.performed += instance.OnPoint;
            @Point.canceled += instance.OnPoint;
            @PointDelta.started += instance.OnPointDelta;
            @PointDelta.performed += instance.OnPointDelta;
            @PointDelta.canceled += instance.OnPointDelta;
        }

        private void UnregisterCallbacks(IUIControlsActions instance)
        {
            @Click.started -= instance.OnClick;
            @Click.performed -= instance.OnClick;
            @Click.canceled -= instance.OnClick;
            @Point.started -= instance.OnPoint;
            @Point.performed -= instance.OnPoint;
            @Point.canceled -= instance.OnPoint;
            @PointDelta.started -= instance.OnPointDelta;
            @PointDelta.performed -= instance.OnPointDelta;
            @PointDelta.canceled -= instance.OnPointDelta;
        }

        public void RemoveCallbacks(IUIControlsActions instance)
        {
            if (m_Wrapper.m_UIControlsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUIControlsActions instance)
        {
            foreach (var item in m_Wrapper.m_UIControlsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UIControlsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UIControlsActions @UIControls => new UIControlsActions(this);
    private int m_DesktopSchemeIndex = -1;
    public InputControlScheme DesktopScheme
    {
        get
        {
            if (m_DesktopSchemeIndex == -1) m_DesktopSchemeIndex = asset.FindControlSchemeIndex("Desktop");
            return asset.controlSchemes[m_DesktopSchemeIndex];
        }
    }
    public interface IDefaultControlsActions
    {
        void OnSelect(InputAction.CallbackContext context);
        void OnExitTool(InputAction.CallbackContext context);
        void OnPick(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
    }
    public interface ICameraControlsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnZoom(InputAction.CallbackContext context);
    }
    public interface IUIControlsActions
    {
        void OnClick(InputAction.CallbackContext context);
        void OnPoint(InputAction.CallbackContext context);
        void OnPointDelta(InputAction.CallbackContext context);
    }
}
