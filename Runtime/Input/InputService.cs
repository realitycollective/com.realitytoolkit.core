// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Controllers;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Handlers;
using RealityToolkit.Input.InputSources;
using RealityToolkit.Input.Interactables;
using RealityToolkit.Input.Interactors;
using RealityToolkit.Input.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEvents = UnityEngine.EventSystems;

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
            if (profile.IsNull())
            {
                throw new Exception($"The {nameof(InputService)} requires a profile to be configured!");
            }
            if (profile.GazeProviderType?.Type == null)
            {
                throw new Exception($"The {nameof(InputService)} is missing the required {nameof(profile.GazeProviderType)}!");
            }

            gazeProviderBehaviour = profile.GazeProviderBehaviour;
            gazeProviderType = profile.GazeProviderType.Type;
            interactors = new List<IInteractor>();
            interactables = new List<IInteractable>();
            DirectInteractionEnabled = profile.DirectInteraction;
            FarInteractionEnabled = profile.FarInteraction;
        }

        private readonly List<IInteractor> interactors;
        private readonly List<IInteractable> interactables;

        /// <inheritdoc />
        public event Action InputEnabled;

        /// <inheritdoc />
        public event Action InputDisabled;

        /// <inheritdoc/>
        public bool DirectInteractionEnabled { get; set; }

        /// <inheritdoc/>
        public bool FarInteractionEnabled { get; set; }

        private readonly HashSet<IInputSource> detectedInputSources = new HashSet<IInputSource>();

        /// <inheritdoc />
        public IReadOnlyCollection<IInputSource> DetectedInputSources => detectedInputSources;

        private readonly HashSet<IController> detectedControllers = new HashSet<IController>();

        /// <inheritdoc />
        public IReadOnlyCollection<IController> DetectedControllers => detectedControllers;

        /// <inheritdoc/>
        public IReadOnlyList<IInteractor> Interactors => interactors;

        /// <inheritdoc/>
        public IReadOnlyList<IInteractable> Interactables => interactables;

        /// <inheritdoc />
        public IFocusProvider FocusProvider { get; private set; }

        /// <inheritdoc />
        public IGazeProvider GazeProvider { get; private set; }

#if INPUT_SYSTEM_INSTALLED
        private Type InputModuleType => typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule);
#else
        private Type InputModuleType => typeof(UnityEngine.EventSystems.StandaloneInputModule);
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
        private PointerEventData pointerEventData;
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

        /// <inheritdoc/>
        public void Add(IInteractor interactor) => interactors.EnsureListItem(interactor);

        /// <inheritdoc/>
        public void Remove(IInteractor interactor) => interactors.SafeRemoveListItem(interactor);

        /// <inheritdoc/>
        public void Add(IInteractable interactable) => interactables.EnsureListItem(interactable);

        /// <inheritdoc/>
        public void Remove(IInteractable interactable) => interactables.SafeRemoveListItem(interactable);

        /// <inheritdoc/>
        public bool TryGetInteractablesByLabel(string label, out IEnumerable<IInteractable> interactables)
        {
            var results = this.interactables.Where(i => !string.IsNullOrWhiteSpace(label) && string.Equals(i.Label, label));
            if (results.Any())
            {
                interactables = results;
                return true;
            }

            interactables = null;
            return false;
        }

        /// <inheritdoc/>
        public bool TryGetInteractors(IInputSource inputSource, out IReadOnlyList<IInteractor> interactors)
        {
            interactors = this.interactors.Where(i => i.InputSource == inputSource).ToList();
            return interactors != null && interactors.Count > 0;
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
                var eventSystem = UnityEvents.EventSystem.current;
                sourceStateEventData = new SourceStateEventData(eventSystem);

                sourceTrackingEventData = new SourcePoseEventData<TrackingState>(eventSystem);
                sourceVector2EventData = new SourcePoseEventData<Vector2>(eventSystem);
                sourcePositionEventData = new SourcePoseEventData<Vector3>(eventSystem);
                sourceRotationEventData = new SourcePoseEventData<Quaternion>(eventSystem);
                sourcePoseEventData = new SourcePoseEventData<Pose>(eventSystem);

                focusEventData = new FocusEventData(eventSystem);

                inputEventData = new InputEventData(eventSystem);
                pointerEventData = new PointerEventData(eventSystem);
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
#if UNITY_2023_1_OR_NEWER
            var inputModules = UnityEngine.Object.FindObjectsByType(InputModuleType, FindObjectsSortMode.None);
            var eventSystemGameObject = UnityEngine.Object.FindFirstObjectByType<UnityEvents.EventSystem>();
#else
            var inputModules = UnityEngine.Object.FindObjectsOfType(InputModuleType);
            var eventSystemGameObject = UnityEngine.Object.FindObjectOfType<UnityEvents.EventSystem>();
#endif

            if (inputModules.Length == 0)
            {
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

            // Clean up functionality for projects switching between input systems, as the Event system needs to be pure to avoid slowdowns or impacts.
            // If the input module type is not the Standalone Input Module, remove the Standalone Input Module if it exists.
            if (InputModuleType != typeof(UnityEngine.EventSystems.StandaloneInputModule) && eventSystemGameObject.gameObject.TryGetComponent<UnityEngine.EventSystems.StandaloneInputModule>(out var oldInputModule))
            {
                oldInputModule.Destroy();
            }
            else
            {
#if UNITY_EDITOR
                // In case the new Input System Input Module was previously on the Event System, remove any MonoBehaviours with missing scripts, to keep the event system object clean.
                UnityEditor.GameObjectUtility.RemoveMonoBehavioursWithMissingScript(eventSystemGameObject.gameObject);
#endif
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

#if UNITY_2023_1_OR_NEWER
                var inputModule = UnityEngine.Object.FindFirstObjectByType(InputModuleType);
#else
                var inputModule = UnityEngine.Object.FindObjectOfType(InputModuleType);
#endif
                if (inputModule.IsNotNull())
                {
                    inputModule.Destroy();
                }
            }
        }

#endregion IService Implementation

        #region IEventSystemManager Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(UnityEvents.BaseEventData eventData, UnityEvents.ExecuteEvents.EventFunction<T> eventHandler)
        {
            if (disabledRefCount > 0)
            {
                return;
            }

            Debug.Assert(eventData != null);
            var baseInputEventData = UnityEvents.ExecuteEvents.ValidateEventData<BaseInputEventData>(eventData);
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
                        if (focusedObject.IsNotNull() && focusedObject.transform.IsChildOf(modalInput.transform))
                        {
                            InputServiceEventHandlers.Execute(focusedObject, baseInputEventData, eventHandler);

                            if (baseInputEventData.used)
                            {
                                return;
                            }
                        }
                        // Otherwise, just invoke the event on the modal handler itself
                        else
                        {
                            InputServiceEventHandlers.Execute(modalInput, baseInputEventData, eventHandler);

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
                if (focusedObject.IsNotNull())
                {
                    InputServiceEventHandlers.Execute(focusedObject, baseInputEventData, eventHandler);

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
                    InputServiceEventHandlers.Execute(fallbackInput, baseInputEventData, eventHandler);

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
        public IInputSource RequestNewGenericInputSource(string name, IInteractor[] pointers = null) => new BaseGenericInputSource(name, pointers);

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
            HandleEvent(sourceStateEventData, InputServiceEventHandlers.OnSourceDetectedEventHandler);

            UpdateGazeProvider();
        }

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
            HandleEvent(sourceStateEventData, InputServiceEventHandlers.OnSourceLostEventHandler);

            FocusProvider?.OnSourceLost(sourceStateEventData);

            UpdateGazeProvider();
        }

        #endregion Input Source State Events

        #region Input Source Pose Events

        /// <inheritdoc />
        public void RaiseSourceTrackingStateChanged(IInputSource source, IController controller, TrackingState state)
        {
            // Create input event
            sourceTrackingEventData.Initialize(source, controller, state);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceTrackingEventData, InputServiceEventHandlers.OnSourceTrackingChangedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseSourcePositionChanged(IInputSource source, IController controller, Vector2 position)
        {
            // Create input event
            sourceVector2EventData.Initialize(source, controller, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceVector2EventData, InputServiceEventHandlers.OnSourcePoseVector2ChangedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseSourcePositionChanged(IInputSource source, IController controller, Vector3 position)
        {
            // Create input event
            sourcePositionEventData.Initialize(source, controller, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePositionEventData, InputServiceEventHandlers.OnSourcePositionChangedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseSourceRotationChanged(IInputSource source, IController controller, Quaternion rotation)
        {
            // Create input event
            sourceRotationEventData.Initialize(source, controller, rotation);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceRotationEventData, InputServiceEventHandlers.OnSourceRotationChangedEventHandler);
        }

        /// <inheritdoc />
        public void RaiseSourcePoseChanged(IInputSource source, IController controller, Pose position)
        {
            // Create input event
            sourcePoseEventData.Initialize(source, controller, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePoseEventData, InputServiceEventHandlers.OnSourcePoseChangedEventHandler);
        }

        #endregion Input Source Pose Events

        #endregion Input Source Events

        #region Focus Events

        /// <inheritdoc />
        public void RaisePreFocusChanged(IInteractor pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            focusEventData.Initialize(pointer, oldFocusedObject, newFocusedObject);

            // Raise Focus Events on the old and new focused objects.
            if (oldFocusedObject.IsNotNull())
            {
                InputServiceEventHandlers.Execute(oldFocusedObject, focusEventData, InputServiceEventHandlers.OnPreFocusChangedHandler);
            }

            if (newFocusedObject.IsNotNull())
            {
                InputServiceEventHandlers.Execute(newFocusedObject, focusEventData, InputServiceEventHandlers.OnPreFocusChangedHandler);
            }

            // Raise Focus Events on the pointers cursor if it has one.
            if (pointer.BaseCursor != null)
            {
                try
                {
                    // When shutting down a game, we can sometime get old references to game objects that have been cleaned up.
                    // We'll ignore when this happens.
                    InputServiceEventHandlers.Execute(pointer.BaseCursor.GameObjectReference, focusEventData, InputServiceEventHandlers.OnPreFocusChangedHandler);
                }
                catch (Exception)
                {
                    // ignored.
                }
            }
        }

        /// <inheritdoc />
        public void RaiseFocusChanged(IInteractor pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            focusEventData.Initialize(pointer, oldFocusedObject, newFocusedObject);

            // Raise Focus Events on the old and new focused objects.
            if (oldFocusedObject.IsNotNull())
            {
                InputServiceEventHandlers.Execute(oldFocusedObject, focusEventData, InputServiceEventHandlers.OnFocusChangedHandler);
            }

            if (newFocusedObject.IsNotNull())
            {
                InputServiceEventHandlers.Execute(newFocusedObject, focusEventData, InputServiceEventHandlers.OnFocusChangedHandler);
            }

            // Raise Focus Events on the pointers cursor if it has one.
            if (pointer.BaseCursor != null)
            {
                try
                {
                    // When shutting down a game, we can sometime get old references to game objects that have been cleaned up.
                    // We'll ignore when this happens.
                    InputServiceEventHandlers.Execute(pointer.BaseCursor.GameObjectReference, focusEventData, InputServiceEventHandlers.OnFocusChangedHandler);
                }
                catch (Exception)
                {
                    // ignored.
                }
            }
        }

        /// <inheritdoc />
        public void RaiseFocusEnter(IInteractor pointer, GameObject focusedObject)
        {
            focusEventData.Initialize(pointer);

            InputServiceEventHandlers.Execute(focusedObject, focusEventData, InputServiceEventHandlers.OnFocusEnterEventHandler);
            InputServiceEventHandlers.Execute(focusedObject, focusEventData, UnityEvents.ExecuteEvents.selectHandler);

            if (FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicEventData))
            {
                InputServiceEventHandlers.Execute(focusedObject, graphicEventData, UnityEvents.ExecuteEvents.pointerEnterHandler);
            }
        }

        /// <inheritdoc />
        public void RaiseFocusExit(IInteractor pointer, GameObject unfocusedObject)
        {
            focusEventData.Initialize(pointer);

            InputServiceEventHandlers.Execute(unfocusedObject, focusEventData, InputServiceEventHandlers.OnFocusExitEventHandler);
            InputServiceEventHandlers.Execute(unfocusedObject, focusEventData, UnityEvents.ExecuteEvents.deselectHandler);

            if (FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicEventData))
            {
                InputServiceEventHandlers.Execute(unfocusedObject, graphicEventData, UnityEvents.ExecuteEvents.pointerExitHandler);
            }
        }

        #endregion Focus Events

        #region Pointers

        #region Pointer Down

        /// <inheritdoc />
        public void RaisePointerDown(IInteractor pointer, InputAction inputAction, IInputSource inputSource = null)
        {
            // Create input event
            pointerEventData.Initialize(pointer, inputAction, inputSource);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(pointerEventData, InputServiceEventHandlers.OnPointerDownEventHandler);

            var focusedObject = pointer.Result.CurrentTarget;

            if (focusedObject.IsNotNull() &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                InputServiceEventHandlers.Execute(focusedObject, graphicInputEventData, UnityEvents.ExecuteEvents.pointerDownHandler);
                InputServiceEventHandlers.Execute(focusedObject, graphicInputEventData, UnityEvents.ExecuteEvents.initializePotentialDrag);
            }
        }

        #endregion Pointer Down

        #region Pointer Click

        /// <inheritdoc />
        public void RaisePointerClicked(IInteractor pointer, InputAction inputAction, IInputSource inputSource = null)
        {
            // Create input event
            pointerEventData.Initialize(pointer, inputAction, inputSource);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(pointerEventData, InputServiceEventHandlers.OnInputClickedEventHandler);

            // NOTE: In Unity UI, a "click" happens on every pointer up, so we have RaisePointerUp call the pointerClickHandler.
        }

        #endregion Pointer Click

        #region Pointer Up

        /// <inheritdoc />
        public void RaisePointerUp(IInteractor pointer, InputAction inputAction, IInputSource inputSource = null)
        {
            // Create input event
            pointerEventData.Initialize(pointer, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(pointerEventData, InputServiceEventHandlers.OnPointerUpEventHandler);

            var focusedObject = pointer.Result.CurrentTarget;

            if (focusedObject.IsNotNull() &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                InputServiceEventHandlers.Execute(focusedObject, graphicInputEventData, UnityEvents.ExecuteEvents.pointerUpHandler);
                InputServiceEventHandlers.Execute(focusedObject, graphicInputEventData, UnityEvents.ExecuteEvents.pointerClickHandler);

                graphicInputEventData.Clear();
            }
        }

        #endregion Pointer Up

        /// <inheritdoc />
        public void RaisePointerScroll(IInteractor pointer, InputAction scrollAction, Vector2 scrollDelta, IInputSource inputSource = null)
        {
            pointerScrollEventData.Initialize(pointer, scrollAction, scrollDelta);

            HandleEvent(pointerScrollEventData, InputServiceEventHandlers.OnPointerScroll);

            var focusedObject = pointer.Result.CurrentTarget;

            if (focusedObject.IsNotNull() &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                graphicInputEventData.scrollDelta = scrollDelta;
                InputServiceEventHandlers.Execute(focusedObject, graphicInputEventData, UnityEvents.ExecuteEvents.scrollHandler);
            }
        }

        #region Pointer Dragging

        /// <inheritdoc />
        public void RaisePointerDragBegin(IInteractor pointer, InputAction draggedAction, Vector3 dragDelta, IInputSource inputSource = null)
        {
            pointerDragEventData.Initialize(pointer, draggedAction, dragDelta);

            HandleEvent(pointerDragEventData, InputServiceEventHandlers.OnPointerDragBegin);

            var focusedObject = pointer.Result.CurrentTarget;

            if (focusedObject.IsNotNull() &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                graphicInputEventData.pointerDrag = focusedObject;
                graphicInputEventData.useDragThreshold = false;
                graphicInputEventData.dragging = true;
                InputServiceEventHandlers.Execute(focusedObject, graphicInputEventData, UnityEvents.ExecuteEvents.beginDragHandler);
            }
        }

        /// <inheritdoc />
        public void RaisePointerDrag(IInteractor pointer, InputAction draggedAction, Vector3 dragDelta, IInputSource inputSource = null)
        {
            pointerDragEventData.Initialize(pointer, draggedAction, dragDelta);

            HandleEvent(pointerDragEventData, InputServiceEventHandlers.OnPointerDrag);

            var focusedObject = pointer.Result.CurrentTarget;

            if (focusedObject.IsNotNull() &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                InputServiceEventHandlers.Execute(focusedObject, graphicInputEventData, UnityEvents.ExecuteEvents.dragHandler);
            }
        }

        /// <inheritdoc />
        public void RaisePointerDragEnd(IInteractor pointer, InputAction draggedAction, Vector3 dragDelta, IInputSource inputSource = null)
        {
            pointerDragEventData.Initialize(pointer, draggedAction, dragDelta);

            HandleEvent(pointerDragEventData, InputServiceEventHandlers.OnPointerDragEnd);

            var focusedObject = pointer.Result.CurrentTarget;

            if (focusedObject.IsNotNull() &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                graphicInputEventData.dragging = false;
                InputServiceEventHandlers.Execute(focusedObject, graphicInputEventData, UnityEvents.ExecuteEvents.endDragHandler);
            }
        }

        #endregion Pointer Dragging

        #endregion Pointers

        #region Generic Input Events

        #region Input Down

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
            HandleEvent(inputEventData, InputServiceEventHandlers.OnInputDownEventHandler);
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
            HandleEvent(floatInputEventData, InputServiceEventHandlers.SingleAxisInputEventHandler);
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
            HandleEvent(floatInputEventData, InputServiceEventHandlers.SingleAxisInputEventHandler);
        }

        #endregion Input Pressed

        #region Input Up

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
            HandleEvent(inputEventData, InputServiceEventHandlers.OnInputUpEventHandler);
        }

        #endregion Input Up

        #region Input Position Changed

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
            HandleEvent(floatInputEventData, InputServiceEventHandlers.SingleAxisInputEventHandler);
        }

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
            HandleEvent(vector2InputEventData, InputServiceEventHandlers.OnTwoDoFInputChanged);
        }

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
            HandleEvent(positionInputEventData, InputServiceEventHandlers.OnPositionInputChanged);
        }

        #endregion Input Position Changed

        #region Input Rotation Changed

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
            HandleEvent(positionInputEventData, InputServiceEventHandlers.OnRotationInputChanged);
        }

        #endregion Input Rotation Changed

        #region Input Pose Changed

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
            HandleEvent(poseInputEventData, InputServiceEventHandlers.OnPoseInputChanged);
        }

        #endregion Input Pose Changed

        #endregion Generic Input Events

        #region Gesture Events

        /// <inheritdoc />
        public void RaiseGestureStarted(IController controller, InputAction action)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));

            inputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action);
            HandleEvent(inputEventData, InputServiceEventHandlers.OnGestureStarted);
        }

        /// <inheritdoc />
        public void RaiseGestureUpdated(IController controller, InputAction action)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            inputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action);
            HandleEvent(inputEventData, InputServiceEventHandlers.OnGestureUpdated);
        }

        /// <inheritdoc />
        public void RaiseGestureUpdated(IController controller, InputAction action, Vector2 inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            vector2InputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(vector2InputEventData, InputServiceEventHandlers.OnGestureVector2PositionUpdated);
        }

        /// <inheritdoc />
        public void RaiseGestureUpdated(IController controller, InputAction action, Vector3 inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            positionInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(positionInputEventData, InputServiceEventHandlers.OnGesturePositionUpdated);
        }

        /// <inheritdoc />
        public void RaiseGestureUpdated(IController controller, InputAction action, Quaternion inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            rotationInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(rotationInputEventData, InputServiceEventHandlers.OnGestureRotationUpdated);
        }

        /// <inheritdoc />
        public void RaiseGestureUpdated(IController controller, InputAction action, Pose inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            poseInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(poseInputEventData, InputServiceEventHandlers.OnGesturePoseUpdated);
        }

        /// <inheritdoc />
        public void RaiseGestureCompleted(IController controller, InputAction action)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            inputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action);
            HandleEvent(inputEventData, InputServiceEventHandlers.OnGestureCompleted);
        }

        /// <inheritdoc />
        public void RaiseGestureCompleted(IController controller, InputAction action, Vector2 inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            vector2InputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(vector2InputEventData, InputServiceEventHandlers.OnGestureVector2PositionCompleted);
        }

        /// <inheritdoc />
        public void RaiseGestureCompleted(IController controller, InputAction action, Vector3 inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            positionInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(positionInputEventData, InputServiceEventHandlers.OnGesturePositionCompleted);
        }

        /// <inheritdoc />
        public void RaiseGestureCompleted(IController controller, InputAction action, Quaternion inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            rotationInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(rotationInputEventData, InputServiceEventHandlers.OnGestureRotationCompleted);
        }

        /// <inheritdoc />
        public void RaiseGestureCompleted(IController controller, InputAction action, Pose inputData)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            poseInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(poseInputEventData, InputServiceEventHandlers.OnGesturePoseCompleted);
        }

        /// <inheritdoc />
        public void RaiseGestureCanceled(IController controller, InputAction action)
        {
            Debug.Assert(detectedInputSources.Contains(controller.InputSource));
            inputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action);
            HandleEvent(inputEventData, InputServiceEventHandlers.OnGestureCanceled);
        }

        #endregion Gesture Events

        #region Speech Keyword Events

        /// <inheritdoc />
        public void RaiseSpeechCommandRecognized(IInputSource source, InputAction inputAction, RecognitionConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, string text)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            speechEventData.Initialize(source, inputAction, confidence, phraseDuration, phraseStartTime, text);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(speechEventData, InputServiceEventHandlers.OnSpeechKeywordRecognizedEventHandler);
        }

        #endregion Speech Keyword Events

        #region Dictation Events

        /// <inheritdoc />
        public void RaiseDictationHypothesis(IInputSource source, string dictationHypothesis, AudioClip dictationAudioClip = null)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            dictationEventData.Initialize(source, dictationHypothesis, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, InputServiceEventHandlers.OnDictationHypothesisEventHandler);
        }

        /// <inheritdoc />
        public void RaiseDictationResult(IInputSource source, string dictationResult, AudioClip dictationAudioClip = null)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, InputServiceEventHandlers.OnDictationResultEventHandler);
        }

        /// <inheritdoc />
        public void RaiseDictationComplete(IInputSource source, string dictationResult, AudioClip dictationAudioClip)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, InputServiceEventHandlers.OnDictationCompleteEventHandler);
        }

        /// <inheritdoc />
        public void RaiseDictationError(IInputSource source, string dictationResult, AudioClip dictationAudioClip = null)
        {
            Debug.Assert(detectedInputSources.Contains(source));

            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, InputServiceEventHandlers.OnDictationErrorEventHandler);
        }

        #endregion Dictation Events

        #endregion Input Events
    }
}
