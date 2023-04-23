// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.InputSources;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Input.Interfaces.Controllers;
using RealityToolkit.Input.Interfaces.Handlers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityToolkit.Input
{
    /// <summary>
    /// The Reality Toolkit's specific implementation of the <see cref="IInputService"/>
    /// </summary>
    [RuntimePlatform(typeof(AllPlatforms))]
    [System.Runtime.InteropServices.Guid("18C9CAF0-8D36-4ADD-BB49-CDF7561CF793")]
    public class InputService : BaseEventService, IInputService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The service display name.</param>
        /// <param name="priority">The service initialization priority.</param>
        /// <param name="profile">The service configuration profile.</param>
        public InputService(string name, uint priority, InputServiceProfile profile)
            : base(name, priority, profile)
        {
            if (profile.GazeProviderType?.Type == null)
            {
                throw new Exception($"The {nameof(IInputService)} is missing the required {nameof(profile.GazeProviderType)}!");
            }

            gazeProviderBehaviour = profile.GazeProviderBehaviour;
            gazeProviderType = profile.GazeProviderType.Type;
        }

        /// <inheritdoc />
        public event Action InputEnabled;

        /// <inheritdoc />
        public event Action InputDisabled;

        private readonly HashSet<IInputSource> detectedInputSources = new HashSet<IInputSource>();

        /// <inheritdoc />
        public IReadOnlyCollection<IInputSource> DetectedInputSources => detectedInputSources;

        private readonly HashSet<IController> detectedControllers = new HashSet<IController>();

        /// <inheritdoc />
        public IReadOnlyCollection<IController> DetectedControllers => detectedControllers;

        /// <inheritdoc />
        public IFocusProvider FocusProvider { get; private set; }

        /// <inheritdoc />
        public IGazeProvider GazeProvider { get; private set; }

#if UNITY_INPUT_SYSTEM_ACTIVE
        private Type InputModuleType => typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule);
#else
        private Type InputModuleType => typeof(StandaloneInputModule);
#endif

        private GazeProviderBehaviour gazeProviderBehaviour;
        private readonly Type gazeProviderType;
        private readonly Stack<GameObject> modalInputStack = new Stack<GameObject>();
        private readonly Stack<GameObject> fallbackInputStack = new Stack<GameObject>();

        /// <inheritdoc />
        public bool IsInputEnabled => disabledRefCount <= 0;

        private int disabledRefCount;

        private SourceStateEventData sourceStateEventData;
        private SourcePoseEventData<TrackingState> sourceTrackingEventData;
        private SourcePoseEventData<Vector2> sourceVector2EventData;
        private SourcePoseEventData<Vector3> sourcePositionEventData;
        private SourcePoseEventData<Quaternion> sourceRotationEventData;
        private SourcePoseEventData<Pose> sourcePoseEventData;

        private FocusEventData focusEventData;

        private InputEventData inputEventData;
        private MixedRealityPointerEventData pointerEventData;
        private PointerDragEventData pointerDragEventData;
        private PointerScrollEventData pointerScrollEventData;

        private InputEventData<float> floatInputEventData;
        private InputEventData<Vector2> vector2InputEventData;
        private InputEventData<Vector3> positionInputEventData;
        private InputEventData<Quaternion> rotationInputEventData;
        private InputEventData<Pose> poseInputEventData;

        private SpeechEventData speechEventData;
        private DictationEventData dictationEventData;

        /// <inheritdoc/>
        public bool TryGetInputSource(uint sourceId, out IInputSource inputSource)
        {
            foreach (var detectedInputSource in DetectedInputSources)
            {
                if (detectedInputSource.SourceId == sourceId)
                {
                    inputSource = detectedInputSource;
                    return true;
                }
            }

            inputSource = null;
            return false;
        }

        #region IGazeProvider options

        /// <inheritdoc />
        public void SetGazeProviderBehaviour(GazeProviderBehaviour gazeProviderBehaviour)
        {
            if (this.gazeProviderBehaviour == gazeProviderBehaviour)
            {
                return;
            }

            this.gazeProviderBehaviour = gazeProviderBehaviour;
            UpdateGazeProvider();
        }

        private void UpdateGazeProvider()
        {
            switch (gazeProviderBehaviour)
            {
                case GazeProviderBehaviour.Auto:
                    if (TryGetControllerWithPointersAttached(out _))
                    {
                        RemoveGazeProvider();
                    }
                    else
                    {
                        EnsureGazeProvider();
                    }
                    break;
                case GazeProviderBehaviour.Active:
                    EnsureGazeProvider();
                    break;
                case GazeProviderBehaviour.Inactive:
                    RemoveGazeProvider();
                    break;
            }
        }

        private void RemoveGazeProvider()
        {
            if (Camera.main.IsNull())
            {
                return;
            }

            var component = Camera.main.gameObject.GetComponent<IGazeProvider>() as Component;
            if (component.IsNotNull())
            {
                component.Destroy();
            }

            GazeProvider = null;
        }

        private void EnsureGazeProvider()
        {
            if (Camera.main.IsNull())
            {
                return;
            }

            GazeProvider = Camera.main.gameObject.EnsureComponent(gazeProviderType) as IGazeProvider;
        }

        private bool TryGetControllerWithPointersAttached(out IController controller)
        {
            if (detectedControllers != null && detectedControllers.Count > 0)
            {
                foreach (var detectedController in detectedControllers)
                {
                    if (detectedController.InputSource.Pointers != null && detectedController.InputSource.Pointers.Length > 0)
                    {
                        controller = detectedController;
                        return true;
                    }
                }
            }

            controller = null;
            return false;
        }

        #endregion IGazeProvider options

        #region IService Implementation

        /// <inheritdoc />
        /// <remarks>
        /// Input system is critical, so should be processed before all other managers
        /// </remarks>
        public override uint Priority => 1;

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            EnsureInputModuleSetup();

            if (Application.isPlaying)
            {
                var eventSystem = EventSystem.current;
                sourceStateEventData = new SourceStateEventData(eventSystem);

                sourceTrackingEventData = new SourcePoseEventData<TrackingState>(eventSystem);
                sourceVector2EventData = new SourcePoseEventData<Vector2>(eventSystem);
                sourcePositionEventData = new SourcePoseEventData<Vector3>(eventSystem);
                sourceRotationEventData = new SourcePoseEventData<Quaternion>(eventSystem);
                sourcePoseEventData = new SourcePoseEventData<Pose>(eventSystem);

                focusEventData = new FocusEventData(eventSystem);

                inputEventData = new InputEventData(eventSystem);
                pointerEventData = new MixedRealityPointerEventData(eventSystem);
                pointerDragEventData = new PointerDragEventData(eventSystem);
                pointerScrollEventData = new PointerScrollEventData(eventSystem);

                floatInputEventData = new InputEventData<float>(eventSystem);
                vector2InputEventData = new InputEventData<Vector2>(eventSystem);
                positionInputEventData = new InputEventData<Vector3>(eventSystem);
                rotationInputEventData = new InputEventData<Quaternion>(eventSystem);
                poseInputEventData = new InputEventData<Pose>(eventSystem);

                speechEventData = new SpeechEventData(eventSystem);
                dictationEventData = new DictationEventData(eventSystem);

                UpdateGazeProvider();
            }

            ServiceManager.Instance.TryGetService(out IFocusProvider focusProvider);
            FocusProvider = focusProvider;
        }

        private void EnsureInputModuleSetup()
        {
            var inputModules = UnityEngine.Object.FindObjectsOfType(InputModuleType);

            if (inputModules.Length == 0)
            {
                var eventSystemGameObject = UnityEngine.Object.FindObjectOfType<EventSystem>();
                if (eventSystemGameObject.IsNotNull())
                {
                    eventSystemGameObject.gameObject.EnsureComponent(InputModuleType);
                    Debug.Log($"There was no {InputModuleType.Name} in the scene. The {nameof(InputService)} requires one and added it to the {eventSystemGameObject.name} game object.");
                }
                else if (Camera.main.IsNotNull())
                {
                    Camera.main.gameObject.EnsureComponent(InputModuleType);
                    Debug.Log($"There was no {InputModuleType.Name} in the scene. The {nameof(InputService)} requires one and added it to the main camera.");
                }
                else
                {
                    var inputModuleObject = new GameObject(InputModuleType.Name, InputModuleType);
                    Debug.Log($"There was no {InputModuleType.Name} in the scene. The {nameof(InputService)} requires one and added it to the scene.");
                }
            }
            else if (inputModules.Length > 1)
            {
                Debug.LogError($"There is more than one {InputModuleType.Name} active in the scene. Please make sure only one instance of it exists as it may cause errors.");
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            InputEnabled?.Invoke();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            InputDisabled?.Invoke();
        }

        /// <inheritdoc />
        protected override void OnDispose(bool finalizing)
        {
            base.OnDispose(finalizing);

            if (finalizing)
            {
                RemoveGazeProvider();

                var inputModule = UnityEngine.Object.FindObjectOfType(InputModuleType);
                if (inputModule.IsNotNull())
                {
                    inputModule.Destroy();
                }
            }
        }

        #endregion IService Implementation

        #region IEventSystemManager Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            if (disabledRefCount > 0)
            {
                return;
            }

            Debug.Assert(eventData != null);
            var baseInputEventData = ExecuteEvents.ValidateEventData<BaseInputEventData>(eventData);
            Debug.Assert(baseInputEventData != null);
            Debug.Assert(!baseInputEventData.used);

            if (baseInputEventData.InputSource == null)
            {
                Debug.LogError($"Failed to find an input source for {baseInputEventData}");
                return;
            }

            // Sent the event to any POCO classes that have subscribed for events.
            // WARNING: This event should not be subscribed to by MonoBehaviours!
            // Use the InputHandler interfaces instead.
            OnInputEvent?.Invoke(baseInputEventData);

            // Send the event to global listeners
            base.HandleEvent(eventData, eventHandler);

            if (baseInputEventData.used)
            {
                // All global listeners get a chance to see the event,
                // but if any of them marked it used,
                // we stop the event from going any further.
                return;
            }

            if (baseInputEventData.InputSource.Pointers == null)
            {
                Debug.LogError($"InputSource {baseInputEventData.InputSource.SourceName} doesn't have any registered pointers! Input Sources without pointers should use the GazeProvider's pointer as a default fallback.");
                return;
            }

            var modalEventHandled = false;

            // Get the focused object for each pointer of the event source
            for (int i = 0; i < baseInputEventData.InputSource.Pointers.Length; i++)
            {
                var focusedObject = FocusProvider?.GetFocusedObject(baseInputEventData.InputSource.Pointers[i]);

                // Handle modal input if one exists
                if (modalInputStack.Count > 0 && !modalEventHandled)
                {
                    var modalInput = modalInputStack.Peek();

                    if (modalInput != null)
                    {
                        modalEventHandled = true;

                        // If there is a focused object in the hierarchy of the modal handler, start the event bubble there
                        if (focusedObject != null && focusedObject.transform.IsChildOf(modalInput.transform))
                        {
                            ExecuteEvents.ExecuteHierarchy(focusedObject, baseInputEventData, eventHandler);

                            if (baseInputEventData.used)
                            {
                                return;
                            }
                        }
                        // Otherwise, just invoke the event on the modal handler itself
                        else
                        {
                            ExecuteEvents.ExecuteHierarchy(modalInput, baseInputEventData, eventHandler);

                            if (baseInputEventData.used)
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("ModalInput GameObject reference was null!\nDid this GameObject get destroyed?");
                    }
                }

                // If event was not handled by modal, pass it on to the current focused object
                if (focusedObject != null)
                {
                    ExecuteEvents.ExecuteHierarchy(focusedObject, baseInputEventData, eventHandler);

                    if (baseInputEventData.used)
                    {
                        return;
                    }
                }
            }

            // If event was not handled by the focused object, pass it on to any fallback handlers
            if (fallbackInputStack.Count > 0)
            {
                var fallbackInput = fallbackInputStack.Peek();

                if (fallbackInput != null)
                {
                    ExecuteEvents.ExecuteHierarchy(fallbackInput, baseInputEventData, eventHandler);

                    if (baseInputEventData.used)
                    {
                        // return;
                    }
                }
            }
        }

        /// <summary>
        /// Register a <see cref="GameObject"/> to listen to events that will receive all input events, regardless
        /// of which other <see cref="GameObject"/>s might have handled the event beforehand.
        /// </summary>
        /// <remarks>Useful for listening to events when the <see cref="GameObject"/> is currently not being raycasted against by the <see cref="FocusProvider"/>.</remarks>
        /// <param name="listener">Listener to add.</param>
        public override void Register(GameObject listener)
        {
            base.Register(listener);
        }

        /// <summary>
        /// Unregister a <see cref="GameObject"/> from listening to input events.
        /// </summary>
        /// <param name="listener"></param>
        public override void Unregister(GameObject listener)
        {
            base.Unregister(listener);
        }

        #endregion IEventSystemManager Implementation

        #region Input Disabled Options

        /// <summary>
        /// Push a disabled input state onto the input manager.
        /// While input is disabled no events will be sent out and the cursor displays
        /// a waiting animation.
        /// </summary>
        public void PushInputDisable()
        {
            ++disabledRefCount;

            if (disabledRefCount == 1)
            {
                InputDisabled?.Invoke();

                if (GazeProvider != null)
                {
                    GazeProvider.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Pop disabled input state. When the last disabled state is
        /// popped off the stack input will be re-enabled.
        /// </summary>
        public void PopInputDisable()
        {
            --disabledRefCount;
            Debug.Assert(disabledRefCount >= 0, "Tried to pop more input disable than the amount pushed.");

            if (disabledRefCount == 0)
            {
                InputEnabled?.Invoke();

                if (GazeProvider != null)
                {
                    GazeProvider.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Clear the input disable stack, which will immediately re-enable UnityEngine.Input.
        /// </summary>
        public void ClearInputDisableStack()
        {
            bool wasInputDisabled = disabledRefCount > 0;
            disabledRefCount = 0;

            if (wasInputDisabled)
            {
                InputEnabled?.Invoke();

                if (GazeProvider != null)
                {
                    GazeProvider.Enabled = true;
                }
            }
        }

        #endregion Input Disabled Options

        #region Modal Input Options

        /// <summary>
        /// Push a game object into the modal input stack. Any input handlers
        /// on the game object are given priority to input events before any focused objects.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        public void PushModalInputHandler(GameObject inputHandler)
        {
            modalInputStack.Push(inputHandler);
        }

        /// <summary>
        /// Remove the last game object from the modal input stack.
        /// </summary>
        public void PopModalInputHandler()
        {
            if (modalInputStack.Count > 0)
            {
                modalInputStack.Pop();

            }
        }

        /// <summary>
        /// Clear all modal input handlers off the stack.
        /// </summary>
        public void ClearModalInputStack()
        {
            modalInputStack.Clear();
        }

        #endregion Modal Input Options

        #region Fallback Input Handler Options

        /// <summary>
        /// Push a game object into the fallback input stack. Any input handlers on
        /// the game object are given input events when no modal or focused objects consume the event.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        public void PushFallbackInputHandler(GameObject inputHandler)
        {
            fallbackInputStack.Push(inputHandler);
        }

        /// <summary>
        /// Remove the last game object from the fallback input stack.
        /// </summary>
        public void PopFallbackInputHandler()
        {
            fallbackInputStack.Pop();
        }

        /// <summary>
        /// Clear all fallback input handlers off the stack.
        /// </summary>
        public void ClearFallbackInputStack()
        {
            fallbackInputStack.Clear();
        }

        #endregion Fallback Input Handler Options

        #region IController Utilities

        /// <inheritdoc />
        public bool TryGetController(IInputSource inputSource, out IController outputController)
        {
            foreach (var controller in detectedControllers)
            {
                if (inputSource.SourceId == controller.InputSource.SourceId)
                {
                    outputController = controller;
                    return true;
                }
            }

            outputController = null;
            return false;
        }

        #endregion IController Utilities

        #region Input Events

        /// <inheritdoc />
        public event Action<BaseInputEventData> OnInputEvent;

        #region Input Source Events

        /// <inheritdoc />
        public uint GenerateNewSourceId()
        {
            var newId = (uint)UnityEngine.Random.Range(1, int.MaxValue);

            foreach (var inputSource in detectedInputSources)
            {
                if (inputSource.SourceId == newId)
                {
                    return GenerateNewSourceId();
                }
            }

            return newId;
        }

        /// <inheritdoc />
        public IInputSource RequestNewGenericInputSource(string name, IPointer[] pointers = null) => new BaseGenericInputSource(name, pointers);

        #region Input Source State Events

        /// <inheritdoc />
        public void RaiseSourceDetected(IInputSource source, IController controller = null)
        {
            // Create input event
            sourceStateEventData.Initialize(source, controller);

            Debug.Assert(!detectedInputSources.Contains(source), $"{source.SourceName} has already been registered with the Input Manager!");

            detectedInputSources.Add(source);

            if (controller != null)
            {
                detectedControllers.Add(controller);
            }

            FocusProvider?.OnSourceDetected(sourceStateEventData);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceDetectedEventHandler);

            UpdateGazeProvider();
        }

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceDetectedEventHandler =
            delegate (ISourceStateHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceDetected(casted);
            };

        /// <inheritdoc />
        public void RaiseSourceLost(IInputSource source, IController controller = null)
        {
            // Create input event
            sourceStateEventData.Initialize(source, controller);

            Debug.Assert(detectedInputSources.Contains(source), $"{source.SourceName} was never registered with the Input Manager!");

            detectedInputSources.Remove(source);

            if (controller != null)
            {
                detectedControllers.Remove(controller);
            }

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceLostEventHandler);

            FocusProvider?.OnSourceLost(sourceStateEventData);

            UpdateGazeProvider();
        }

        private static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceLostEventHandler =
            delegate (ISourceStateHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceLost(casted);
            };

        #endregion Input Source State Events

        #region Input Source Pose Events

        /// <inheritdoc />
        public void RaiseSourceTrackingStateChanged(IInputSource source, IController controller, TrackingState state)
        {
            // Create input event
            sourceTrackingEventData.Initialize(source, controller, state);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceTrackingEventData, OnSourceTrackingChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourcePoseHandler> OnSourceTrackingChangedEventHandler =
            delegate (ISourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<TrackingState>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseSourcePositionChanged(IInputSource source, IController controller, Vector2 position)
        {
            // Create input event
            sourceVector2EventData.Initialize(source, controller, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceVector2EventData, OnSourcePoseVector2ChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourcePoseHandler> OnSourcePoseVector2ChangedEventHandler =
            delegate (ISourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<Vector2>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseSourcePositionChanged(IInputSource source, IController controller, Vector3 position)
        {
            // Create input event
            sourcePositionEventData.Initialize(source, controller, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePositionEventData, OnSourcePositionChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourcePoseHandler> OnSourcePositionChangedEventHandler =
            delegate (ISourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<Vector3>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseSourceRotationChanged(IInputSource source, IController controller, Quaternion rotation)
        {
            // Create input event
            sourceRotationEventData.Initialize(source, controller, rotation);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceRotationEventData, OnSourceRotationChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourcePoseHandler> OnSourceRotationChangedEventHandler =
            delegate (ISourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<Quaternion>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseSourcePoseChanged(IInputSource source, IController controller, Pose position)
        {
            // Create input event
            sourcePoseEventData.Initialize(source, controller, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePoseEventData, OnSourcePoseChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<ISourcePoseHandler> OnSourcePoseChangedEventHandler =
            delegate (ISourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<Pose>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };

        #endregion Input Source Pose Events

        #endregion Input Source Events

        #region Focus Events

        /// <inheritdoc />
        public void RaisePreFocusChanged(IPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            focusEventData.Initialize(pointer, oldFocusedObject, newFocusedObject);

            // Raise Focus Events on the old and new focused objects.
            if (oldFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(oldFocusedObject, focusEventData, OnPreFocusChangedHandler);
            }

            if (newFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(newFocusedObject, focusEventData, OnPreFocusChangedHandler);
            }

            // Raise Focus Events on the pointers cursor if it has one.
            if (pointer.BaseCursor != null)
            {
                try
                {
                    // When shutting down a game, we can sometime get old references to game objects that have been cleaned up.
                    // We'll ignore when this happens.
                    ExecuteEvents.ExecuteHierarchy(pointer.BaseCursor.GameObjectReference, focusEventData, OnPreFocusChangedHandler);
                }
                catch (Exception)
                {
                    // ignored.
                }
            }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusChangedHandler> OnPreFocusChangedHandler =
            delegate (IFocusChangedHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnBeforeFocusChange(casted);
            };

        /// <inheritdoc />
        public void RaiseFocusChanged(IPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            focusEventData.Initialize(pointer, oldFocusedObject, newFocusedObject);

            // Raise Focus Events on the old and new focused objects.
            if (oldFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(oldFocusedObject, focusEventData, OnFocusChangedHandler);
            }

            if (newFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(newFocusedObject, focusEventData, OnFocusChangedHandler);
            }

            // Raise Focus Events on the pointers cursor if it has one.
            if (pointer.BaseCursor != null)
            {
                try
                {
                    // When shutting down a game, we can sometime get old references to game objects that have been cleaned up.
                    // We'll ignore when this happens.
                    ExecuteEvents.ExecuteHierarchy(pointer.BaseCursor.GameObjectReference, focusEventData, OnFocusChangedHandler);
                }
                catch (Exception)
                {
                    // ignored.
                }
            }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusChangedHandler> OnFocusChangedHandler =
            delegate (IFocusChangedHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnFocusChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseFocusEnter(IPointer pointer, GameObject focusedObject)
        {
            focusEventData.Initialize(pointer);

            ExecuteEvents.ExecuteHierarchy(focusedObject, focusEventData, OnFocusEnterEventHandler);
            ExecuteEvents.ExecuteHierarchy(focusedObject, focusEventData, ExecuteEvents.selectHandler);

            if (FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicEventData))
            {
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicEventData, ExecuteEvents.pointerEnterHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusHandler> OnFocusEnterEventHandler =
            delegate (IFocusHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnFocusEnter(casted);
            };

        /// <inheritdoc />
        public void RaiseFocusExit(IPointer pointer, GameObject unfocusedObject)
        {
            focusEventData.Initialize(pointer);

            ExecuteEvents.ExecuteHierarchy(unfocusedObject, focusEventData, OnFocusExitEventHandler);
            ExecuteEvents.ExecuteHierarchy(unfocusedObject, focusEventData, ExecuteEvents.deselectHandler);

            if (FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicEventData))
            {
                ExecuteEvents.ExecuteHierarchy(unfocusedObject, graphicEventData, ExecuteEvents.pointerExitHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusHandler> OnFocusExitEventHandler =
            delegate (IFocusHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnFocusExit(casted);
            };

        #endregion Focus Events

        #region Pointers

        #region Pointer Down

        private static readonly ExecuteEvents.EventFunction<IPointerHandler> OnPointerDownEventHandler =
            delegate (IPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<MixedRealityPointerEventData>(eventData);
                handler.OnPointerDown(casted);
            };

        /// <inheritdoc />
        public void RaisePointerDown(IPointer pointer, InputAction inputAction, IInputSource inputSource = null)
        {
            // Create input event
            pointerEventData.Initialize(pointer, inputAction, inputSource);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(pointerEventData, OnPointerDownEventHandler);

            var focusedObject = pointer.Result.CurrentPointerTarget;

            if (focusedObject != null &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicInputEventData, ExecuteEvents.pointerDownHandler);
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicInputEventData, ExecuteEvents.initializePotentialDrag);
            }
        }

        #endregion Pointer Down

        #region Pointer Click

        private static readonly ExecuteEvents.EventFunction<IPointerHandler> OnInputClickedEventHandler =
            delegate (IPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<MixedRealityPointerEventData>(eventData);
                handler.OnPointerClicked(casted);
            };

        /// <inheritdoc />
        public void RaisePointerClicked(IPointer pointer, InputAction inputAction, IInputSource inputSource = null)
        {
            // Create input event
            pointerEventData.Initialize(pointer, inputAction, inputSource);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(pointerEventData, OnInputClickedEventHandler);

            // NOTE: In Unity UI, a "click" happens on every pointer up, so we have RaisePointerUp call the pointerClickHandler.
        }

        #endregion Pointer Click

        #region Pointer Up

        private static readonly ExecuteEvents.EventFunction<IPointerHandler> OnPointerUpEventHandler =
            delegate (IPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<MixedRealityPointerEventData>(eventData);
                handler.OnPointerUp(casted);
            };

        /// <inheritdoc />
        public void RaisePointerUp(IPointer pointer, InputAction inputAction, IInputSource inputSource = null)
        {
            // Create input event
            pointerEventData.Initialize(pointer, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(pointerEventData, OnPointerUpEventHandler);

            var focusedObject = pointer.Result.CurrentPointerTarget;

            if (focusedObject != null &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicInputEventData, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicInputEventData, ExecuteEvents.pointerClickHandler);

                graphicInputEventData.Clear();
            }
        }

        #endregion Pointer Up

        private static readonly ExecuteEvents.EventFunction<IPointerScrollHandler> OnPointerScroll =
            delegate (IPointerScrollHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<PointerScrollEventData>(eventData);
                handler.OnPointerScroll(casted);
            };

        /// <inheritdoc />
        public void RaisePointerScroll(IPointer pointer, InputAction scrollAction, Vector2 scrollDelta, IInputSource inputSource = null)
        {
            pointerScrollEventData.Initialize(pointer, scrollAction, scrollDelta);

            HandleEvent(pointerScrollEventData, OnPointerScroll);

            var focusedObject = pointer.Result.CurrentPointerTarget;

            if (focusedObject != null &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                graphicInputEventData.scrollDelta = scrollDelta;
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicInputEventData, ExecuteEvents.scrollHandler);
            }
        }

        #region Pointer Dragging

        private static readonly ExecuteEvents.EventFunction<IPointerDragHandler> OnPointerDragBegin =
            delegate (IPointerDragHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<PointerDragEventData>(eventData);
                handler.OnPointerDragBegin(casted);
            };

        /// <inheritdoc />
        public void RaisePointerDragBegin(IPointer pointer, InputAction draggedAction, Vector3 dragDelta, IInputSource inputSource = null)
        {
            pointerDragEventData.Initialize(pointer, draggedAction, dragDelta);

            HandleEvent(pointerDragEventData, OnPointerDragBegin);

            var focusedObject = pointer.Result.CurrentPointerTarget;

            if (focusedObject != null &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                graphicInputEventData.pointerDrag = focusedObject;
                graphicInputEventData.useDragThreshold = false;
                graphicInputEventData.dragging = true;
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicInputEventData, ExecuteEvents.beginDragHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IPointerDragHandler> OnPointerDrag =
            delegate (IPointerDragHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<PointerDragEventData>(eventData);
                handler.OnPointerDrag(casted);
            };

        /// <inheritdoc />
        public void RaisePointerDrag(IPointer pointer, InputAction draggedAction, Vector3 dragDelta, IInputSource inputSource = null)
        {
            pointerDragEventData.Initialize(pointer, draggedAction, dragDelta);

            HandleEvent(pointerDragEventData, OnPointerDrag);

            var focusedObject = pointer.Result.CurrentPointerTarget;

            if (focusedObject != null &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicInputEventData, ExecuteEvents.dragHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IPointerDragHandler> OnPointerDragEnd =
            delegate (IPointerDragHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<PointerDragEventData>(eventData);
                handler.OnPointerDragEnd(casted);
            };

        /// <inheritdoc />
        public void RaisePointerDragEnd(IPointer pointer, InputAction draggedAction, Vector3 dragDelta, IInputSource inputSource = null)
        {
            pointerDragEventData.Initialize(pointer, draggedAction, dragDelta);

            HandleEvent(pointerDragEventData, OnPointerDragEnd);

            var focusedObject = pointer.Result.CurrentPointerTarget;

            if (focusedObject != null &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                graphicInputEventData.dragging = false;
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicInputEventData, ExecuteEvents.endDragHandler);
            }
        }

        #endregion Pointer Dragging

        #endregion Pointers

        #region Generic Input Events

        #region Input Down

        private static readonly ExecuteEvents.EventFunction<IInputHandler> OnInputDownEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputDown(casted);
            };

        /// <inheritdoc />
        public void RaiseOnInputDown(IInputSource source, InputAction inputAction)
        {
            RaiseOnInputDown(source, Handedness.None, inputAction);
        }

        /// <inheritdoc />
        public void RaiseOnInputDown(IInputSource source, Handedness handedness, InputAction inputAction)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            inputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

        #endregion Input Down

        #region Input Pressed

        /// <inheritdoc />
        public void RaiseOnInputPressed(IInputSource source, InputAction inputAction)
        {
            RaiseOnInputPressed(source, Handedness.None, inputAction);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IInputSource source, Handedness handedness, InputAction inputAction)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            floatInputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(floatInputEventData, SingleAxisInputEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IInputSource source, InputAction inputAction, float pressAmount)
        {
            RaiseOnInputPressed(source, Handedness.None, inputAction, pressAmount);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IInputSource source, Handedness handedness, InputAction inputAction, float pressAmount)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            floatInputEventData.Initialize(source, handedness, inputAction, pressAmount);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(floatInputEventData, SingleAxisInputEventHandler);
        }

        #endregion Input Pressed

        #region Input Up

        private static readonly ExecuteEvents.EventFunction<IInputHandler> OnInputUpEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputUp(casted);
            };

        /// <inheritdoc />
        public void RaiseOnInputUp(IInputSource source, InputAction inputAction)
        {
            RaiseOnInputUp(source, Handedness.None, inputAction);
        }

        /// <inheritdoc />
        public void RaiseOnInputUp(IInputSource source, Handedness handedness, InputAction inputAction)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            inputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

        #endregion Input Up

        #region Input Position Changed

        private static readonly ExecuteEvents.EventFunction<IInputHandler<float>> SingleAxisInputEventHandler =
            delegate (IInputHandler<float> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<float>>(eventData);
                handler.OnInputChanged(casted);
            };

        /// <inheritdoc />
        public void RaisePositionInputChanged(IInputSource source, InputAction inputAction, float inputPosition)
        {
            RaisePositionInputChanged(source, Handedness.None, inputAction, inputPosition);
        }

        /// <inheritdoc />
        public void RaisePositionInputChanged(IInputSource source, Handedness handedness, InputAction inputAction, float inputPosition)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            floatInputEventData.Initialize(source, handedness, inputAction, inputPosition);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(floatInputEventData, SingleAxisInputEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IInputHandler<Vector2>> OnTwoDoFInputChanged =
            delegate (IInputHandler<Vector2> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector2>>(eventData);
                handler.OnInputChanged(casted);
            };

        /// <inheritdoc />
        public void RaisePositionInputChanged(IInputSource source, InputAction inputAction, Vector2 inputPosition)
        {
            RaisePositionInputChanged(source, Handedness.None, inputAction, inputPosition);
        }

        /// <inheritdoc />
        public void RaisePositionInputChanged(IInputSource source, Handedness handedness, InputAction inputAction, Vector2 inputPosition)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            vector2InputEventData.Initialize(source, handedness, inputAction, inputPosition);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(vector2InputEventData, OnTwoDoFInputChanged);
        }

        private static readonly ExecuteEvents.EventFunction<IInputHandler<Vector3>> OnPositionInputChanged =
            delegate (IInputHandler<Vector3> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnInputChanged(casted);
            };

        /// <inheritdoc />
        public void RaisePositionInputChanged(IInputSource source, InputAction inputAction, Vector3 position)
        {
            RaisePositionInputChanged(source, Handedness.None, inputAction, position);
        }

        /// <inheritdoc />
        public void RaisePositionInputChanged(IInputSource source, Handedness handedness, InputAction inputAction, Vector3 position)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            positionInputEventData.Initialize(source, handedness, inputAction, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnPositionInputChanged);
        }

        #endregion Input Position Changed

        #region Input Rotation Changed

        private static readonly ExecuteEvents.EventFunction<IInputHandler<Quaternion>> OnRotationInputChanged =
            delegate (IInputHandler<Quaternion> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Quaternion>>(eventData);
                handler.OnInputChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseRotationInputChanged(IInputSource source, InputAction inputAction, Quaternion rotation)
        {
            RaiseRotationInputChanged(source, Handedness.None, inputAction, rotation);
        }

        /// <inheritdoc />
        public void RaiseRotationInputChanged(IInputSource source, Handedness handedness, InputAction inputAction, Quaternion rotation)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            rotationInputEventData.Initialize(source, handedness, inputAction, rotation);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnRotationInputChanged);
        }

        #endregion Input Rotation Changed

        #region Input Pose Changed

        private static readonly ExecuteEvents.EventFunction<IInputHandler<Pose>> OnPoseInputChanged =
            delegate (IInputHandler<Pose> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Pose>>(eventData);
                handler.OnInputChanged(casted);
            };

        /// <inheritdoc />
        public void RaisePoseInputChanged(IInputSource source, InputAction inputAction, Pose inputData)
        {
            RaisePoseInputChanged(source, Handedness.None, inputAction, inputData);
        }

        /// <inheritdoc />
        public void RaisePoseInputChanged(IInputSource source, Handedness handedness, InputAction inputAction, Pose inputData)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            poseInputEventData.Initialize(source, handedness, inputAction, inputData);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(poseInputEventData, OnPoseInputChanged);
        }

        #endregion Input Pose Changed

        #endregion Generic Input Events

        #region Gesture Events

        private static readonly ExecuteEvents.EventFunction<IGestureHandler> OnGestureStarted =
            delegate (IGestureHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnGestureStarted(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureStarted(IController controller, InputAction action)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));

            inputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action);
            HandleEvent(inputEventData, OnGestureStarted);
        }

        private static readonly ExecuteEvents.EventFunction<IGestureHandler> OnGestureUpdated =
            delegate (IGestureHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnGestureUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureUpdated(IController controller, InputAction action)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            inputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action);
            HandleEvent(inputEventData, OnGestureUpdated);
        }

        private static readonly ExecuteEvents.EventFunction<IGestureHandler<Vector2>> OnGestureVector2PositionUpdated =
            delegate (IGestureHandler<Vector2> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector2>>(eventData);
                handler.OnGestureUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureUpdated(IController controller, InputAction action, Vector2 inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            vector2InputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(vector2InputEventData, OnGestureVector2PositionUpdated);
        }

        private static readonly ExecuteEvents.EventFunction<IGestureHandler<Vector3>> OnGesturePositionUpdated =
            delegate (IGestureHandler<Vector3> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnGestureUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureUpdated(IController controller, InputAction action, Vector3 inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            positionInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(positionInputEventData, OnGesturePositionUpdated);
        }

        private static readonly ExecuteEvents.EventFunction<IGestureHandler<Quaternion>> OnGestureRotationUpdated =
            delegate (IGestureHandler<Quaternion> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Quaternion>>(eventData);
                handler.OnGestureUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureUpdated(IController controller, InputAction action, Quaternion inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            rotationInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(rotationInputEventData, OnGestureRotationUpdated);
        }

        private static readonly ExecuteEvents.EventFunction<IGestureHandler<Pose>> OnGesturePoseUpdated =
            delegate (IGestureHandler<Pose> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Pose>>(eventData);
                handler.OnGestureUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureUpdated(IController controller, InputAction action, Pose inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            poseInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(poseInputEventData, OnGesturePoseUpdated);
        }

        private static readonly ExecuteEvents.EventFunction<IGestureHandler> OnGestureCompleted =
            delegate (IGestureHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnGestureCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureCompleted(IController controller, InputAction action)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            inputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action);
            HandleEvent(inputEventData, OnGestureCompleted);
        }

        private static readonly ExecuteEvents.EventFunction<IGestureHandler<Vector2>> OnGestureVector2PositionCompleted =
            delegate (IGestureHandler<Vector2> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector2>>(eventData);
                handler.OnGestureCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureCompleted(IController controller, InputAction action, Vector2 inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            vector2InputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(vector2InputEventData, OnGestureVector2PositionCompleted);
        }

        private static readonly ExecuteEvents.EventFunction<IGestureHandler<Vector3>> OnGesturePositionCompleted =
            delegate (IGestureHandler<Vector3> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnGestureCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureCompleted(IController controller, InputAction action, Vector3 inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            positionInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(positionInputEventData, OnGesturePositionCompleted);
        }

        private static readonly ExecuteEvents.EventFunction<IGestureHandler<Quaternion>> OnGestureRotationCompleted =
            delegate (IGestureHandler<Quaternion> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Quaternion>>(eventData);
                handler.OnGestureCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureCompleted(IController controller, InputAction action, Quaternion inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            rotationInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(rotationInputEventData, OnGestureRotationCompleted);
        }

        private static readonly ExecuteEvents.EventFunction<IGestureHandler<Pose>> OnGesturePoseCompleted =
            delegate (IGestureHandler<Pose> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Pose>>(eventData);
                handler.OnGestureCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureCompleted(IController controller, InputAction action, Pose inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            poseInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(poseInputEventData, OnGesturePoseCompleted);
        }

        private static readonly ExecuteEvents.EventFunction<IGestureHandler> OnGestureCanceled =
            delegate (IGestureHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnGestureCanceled(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureCanceled(IController controller, InputAction action)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            inputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action);
            HandleEvent(inputEventData, OnGestureCanceled);
        }

        #endregion Gesture Events

        #region Speech Keyword Events

        private static readonly ExecuteEvents.EventFunction<ISpeechHandler> OnSpeechKeywordRecognizedEventHandler =
            delegate (ISpeechHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SpeechEventData>(eventData);
                handler.OnSpeechKeywordRecognized(casted);
            };

        /// <inheritdoc />
        public void RaiseSpeechCommandRecognized(IInputSource source, InputAction inputAction, RecognitionConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, string text)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            speechEventData.Initialize(source, inputAction, confidence, phraseDuration, phraseStartTime, text);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(speechEventData, OnSpeechKeywordRecognizedEventHandler);
        }

        #endregion Speech Keyword Events

        #region Dictation Events

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationHypothesisEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationHypothesis(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationHypothesis(IInputSource source, string dictationHypothesis, AudioClip dictationAudioClip = null)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            dictationEventData.Initialize(source, dictationHypothesis, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationHypothesisEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationResultEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationResult(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationResult(IInputSource source, string dictationResult, AudioClip dictationAudioClip = null)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationResultEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationCompleteEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationComplete(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationComplete(IInputSource source, string dictationResult, AudioClip dictationAudioClip)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationCompleteEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationErrorEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationError(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationError(IInputSource source, string dictationResult, AudioClip dictationAudioClip = null)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationErrorEventHandler);
        }

        #endregion Dictation Events

        #endregion Input Events
    }
}
