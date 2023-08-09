// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.ServiceFramework.Interfaces;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Definitions;
using RealityToolkit.Input.Interactions.Interactables;
using RealityToolkit.Input.Interactions.Interactors;
using RealityToolkit.Input.Interfaces.Controllers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Input.Interfaces
{
    /// <summary>
    /// Manager interface for a Input system in the Reality Toolkit
    /// All replacement systems for providing Input System functionality should derive from this interface
    /// </summary>
    public interface IInputService : IEventService
    {
        /// <summary>
        /// Event that's raised when the Input is enabled.
        /// </summary>
        event Action InputEnabled;

        /// <summary>
        /// Event that's raised when the Input is disabled.
        /// </summary>
        event Action InputDisabled;

        /// <summary>
        /// Gets or sets whether near interaction should work or not.
        /// </summary>
        bool NearInteractionEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether far interaction should work or not.
        /// </summary>
        bool FarInteractionEnabled { get; set; }

        /// <summary>
        /// List of the Interaction Input Sources as detected by the input manager like hands or motion controllers.
        /// </summary>
        IReadOnlyCollection<IInputSource> DetectedInputSources { get; }

        /// <summary>
        /// List of <see cref="IController"/>s currently detected by the input manager.
        /// </summary>
        /// <remarks>
        /// This property is similar to <see cref="DetectedInputSources"/>, as this is a subset of those <see cref="IInputSource"/>s in that list.
        /// </remarks>
        IReadOnlyCollection<IController> DetectedControllers { get; }

        /// <summary>
        /// Available <see cref="IInteractor"/>s in the scene.
        /// </summary>
        IReadOnlyList<IInteractor> Interactors { get; }

        /// <summary>
        /// Available <see cref="IInteractable"/>s in the scene.
        /// </summary>
        IReadOnlyList<IInteractable> Interactables { get; }

        /// <summary>
        /// The current Focus Provider that's been implemented by this Input System.
        /// </summary>
        IFocusProvider FocusProvider { get; }

        /// <summary>
        /// The current Gaze Provider that's been implemented by this Input System.
        /// </summary>
        IGazeProvider GazeProvider { get; }

        /// <summary>
        /// Indicates if input is currently enabled or not.
        /// </summary>
        bool IsInputEnabled { get; }

        /// <summary>
        /// Looks up the <see cref="IInputSource"/> from <see cref="DetectedInputSources"/>,
        /// if it exists.
        /// </summary>
        /// <param name="sourceId">The <see cref="IInputSource"/> identifier.</param>
        /// <param name="inputSource">The found <see cref="IInputSource"/>.</param>
        /// <returns><c>true</c>, if the source was found.</returns>
        bool TryGetInputSource(uint sourceId, out IInputSource inputSource);

        /// <summary>
        /// Push a disabled input state onto the Input System.
        /// While input is disabled no events will be sent out and the cursor displays
        /// a waiting animation.
        /// </summary>
        void PushInputDisable();

        /// <summary>
        /// Pop disabled input state. When the last disabled state is 
        /// popped off the stack input will be re-enabled.
        /// </summary>
        void PopInputDisable();

        /// <summary>
        /// Clear the input disable stack, which will immediately re-enable UnityEngine.Input.
        /// </summary>
        void ClearInputDisableStack();

        /// <summary>
        /// Push a game object into the modal input stack. Any input handlers
        /// on the game object are given priority to input events before any focused objects.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        void PushModalInputHandler(GameObject inputHandler);

        /// <summary>
        /// Remove the last game object from the modal input stack.
        /// </summary>
        void PopModalInputHandler();

        /// <summary>
        /// Clear all modal input handlers off the stack.
        /// </summary>
        void ClearModalInputStack();

        /// <summary>
        /// Push a game object into the fallback input stack. Any input handlers on
        /// the game object are given input events when no modal or focused objects consume the event.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        void PushFallbackInputHandler(GameObject inputHandler);

        /// <summary>
        /// Remove the last game object from the fallback input stack.
        /// </summary>
        void PopFallbackInputHandler();

        /// <summary>
        /// Clear all fallback input handlers off the stack.
        /// </summary>
        void ClearFallbackInputStack();

        /// <summary>
        /// Sets and updates the behaviour of the configured <see cref="IGazeProvider"/>.
        /// </summary>
        /// <param name="gazeProviderBehaviour">The new <see cref="GazeProviderBehaviour"/>.</param>
        void SetGazeProviderBehaviour(GazeProviderBehaviour gazeProviderBehaviour);

        /// <summary>
        /// Adds an <see cref="IInteractor"/> to the service's registry.
        /// </summary>
        /// <param name="interactor">The <see cref="IInteractor"/> to add.</param>
        void Add(IInteractor interactor);

        /// <summary>
        /// Removes an <see cref="IInteractor"/> from the service's registry.
        /// </summary>
        /// <param name="interactor">The <see cref="IInteractor"/> to remove.</param>
        void Remove(IInteractor interactor);

        /// <summary>
        /// Adds an <see cref="IInteractable"/> to the service's registry.
        /// </summary>
        /// <param name="interactable">The <see cref="IInteractable"/> to add.</param>
        void Add(IInteractable interactable);

        /// <summary>
        /// Removes an <see cref="IInteractable"/> from the service's registry.
        /// </summary>
        /// <param name="interactable">The <see cref="IInteractable"/> to remove.</param>
        void Remove(IInteractable interactable);

        /// <summary>
        /// Gets all known <see cref="IInteractable"/>s that have the <paramref name="label"/> provided.
        /// </summary>
        /// <param name="label">The label to look for.</param>
        /// <param name="interactables">Collection of <see cref="IInteractable"/>s with the requested label.</param>
        /// <returns><c>true, if any <see cref="IInteractable"/>s were found.</c></returns>
        bool TryGetInteractablesByLabel(string label, out IEnumerable<IInteractable> interactables);

        /// <summary>
        /// Gets the <see cref="IInteractor"/>s for the <paramref name="inputSource"/>.
        /// </summary>
        /// <param name="inputSource">The <see cref="IInputSource"/> to find the <see cref="IInteractor"/>s for.</param>
        /// <param name="interactors">The <see cref="IInteractor"/>s found.</param>
        /// <returns><c>true</c>, if <see cref="IInteractor"/>s were found.</returns>
        bool TryGetInteractors(IInputSource inputSource, out IReadOnlyList<IInteractor> interactors);

        #region IController Utilities

        /// <summary>
        /// Tried to get a <see cref="IController"/> from the <see cref="DetectedControllers"/> list.
        /// </summary>
        /// <param name="inputSource">The <see cref="IInputSource"/> you want to get a controller reference for.</param>
        /// <param name="controller">The <see cref="IController"/> that was found in the list of <see cref="DetectedControllers"/></param>
        /// <returns>True, if an <see cref="IController"/> is found.</returns>
        bool TryGetController(IInputSource inputSource, out IController controller);

        #endregion IController Utilities

        #region Input Events

        /// <summary>
        /// Raised when an input event is triggered.
        /// </summary>
        /// <remarks>
        /// WARNING: This event should not be subscribed to by MonoBehaviours!
        /// Use the InputHandler interfaces instead.
        /// </remarks>
        event Action<BaseInputEventData> OnInputEvent;

        #region Input Source Events

        /// <summary>
        /// Generates a new unique input source id.<para/>
        /// </summary>
        /// <remarks>All Input Sources are required to call this method in their constructor or initialization.</remarks>
        /// <returns>a new unique Id for the input source.</returns>
        uint GenerateNewSourceId();

        /// <summary>
        /// Request a new generic input source from the system.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pointers"></param>
        /// <returns></returns>
        IInputSource RequestNewGenericInputSource(string name, IInteractor[] pointers = null);

        /// <summary>
        /// Raise the event that the Input Source was detected.
        /// </summary>
        /// <param name="source">The detected Input Source.</param>
        /// <param name="controller"></param>
        void RaiseSourceDetected(IInputSource source, IController controller = null);

        /// <summary>
        /// Raise the event that the Input Source was lost.
        /// </summary>
        /// <param name="source">The lost Input Source.</param>
        /// <param name="controller"></param>
        void RaiseSourceLost(IInputSource source, IController controller = null);

        /// <summary>
        /// Raise the event that the Input Source's tracking state has changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="state"></param>
        void RaiseSourceTrackingStateChanged(IInputSource source, IController controller, TrackingState state);

        /// <summary>
        /// Raise the event that the Input Source position was changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="position"></param>
        void RaiseSourcePositionChanged(IInputSource source, IController controller, Vector2 position);

        /// <summary>
        /// Raise the event that the Input Source position was changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="position"></param>
        void RaiseSourcePositionChanged(IInputSource source, IController controller, Vector3 position);

        /// <summary>
        /// Raise the event that the Input Source position was changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="rotation"></param>
        void RaiseSourceRotationChanged(IInputSource source, IController controller, Quaternion rotation);

        /// <summary>
        /// Raise the event that the Input Source position was changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="position"></param>
        void RaiseSourcePoseChanged(IInputSource source, IController controller, Pose position);

        #endregion Input Source Events

        #region Focus Events

        /// <summary>
        /// Raise the pre-focus changed event.
        /// </summary>
        /// <remarks>This event is useful for doing logic before the focus changed event.</remarks>
        /// <param name="pointer">The pointer that the focus change event is raised on.</param>
        /// <param name="oldFocusedObject">The old focused object.</param>
        /// <param name="newFocusedObject">The new focused object.</param>
        void RaisePreFocusChanged(IInteractor pointer, GameObject oldFocusedObject, GameObject newFocusedObject);

        /// <summary>
        /// Raise the focus changed event.
        /// </summary>
        /// <param name="pointer">The pointer that the focus change event is raised on.</param>
        /// <param name="oldFocusedObject">The old focused object.</param>
        /// <param name="newFocusedObject">The new focused object.</param>
        void RaiseFocusChanged(IInteractor pointer, GameObject oldFocusedObject, GameObject newFocusedObject);

        /// <summary>
        /// Raise the focus enter event.
        /// </summary>
        /// <param name="pointer">The pointer that has focus.</param>
        /// <param name="focusedObject">The <see cref="GameObject"/> that the pointer has entered focus on.</param>
        void RaiseFocusEnter(IInteractor pointer, GameObject focusedObject);

        /// <summary>
        /// Raise the focus exit event.
        /// </summary>
        /// <param name="pointer">The pointer that has lost focus.</param>
        /// <param name="unfocusedObject">The <see cref="GameObject"/> that the pointer has exited focus on.</param>
        void RaiseFocusExit(IInteractor pointer, GameObject unfocusedObject);

        #endregion Focus Events

        #region Pointers

        #region Pointer Down

        /// <summary>
        /// Raise the pointer down event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="inputAction">The action associated with this event.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerDown(IInteractor pointer, InputAction inputAction, IInputSource inputSource = null);

        #endregion Pointer Down

        #region Pointer Click

        /// <summary>
        /// Raise the pointer clicked event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="inputAction">The action associated with this event.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerClicked(IInteractor pointer, InputAction inputAction, IInputSource inputSource = null);

        #endregion Pointer Click

        #region Pointer Up

        /// <summary>
        /// Raise the pointer up event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="inputAction">The action associated with this event.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerUp(IInteractor pointer, InputAction inputAction, IInputSource inputSource = null);

        #endregion Pointer Up

        /// <summary>
        /// Raise the pointer scroll event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="scrollAction">The action associated with this event.</param>
        /// <param name="scrollDelta">The distance this pointer has scrolled since the scroll event was last raised.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerScroll(IInteractor pointer, InputAction scrollAction, Vector2 scrollDelta, IInputSource inputSource = null);

        #region Pointer Dragging

        /// <summary>
        /// Raise the pointer drag begin event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="draggedAction">The action associated with this event.</param>
        /// <param name="dragDelta">The distance this pointer has been moved since the last time the dragged event was last raised.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerDragBegin(IInteractor pointer, InputAction draggedAction, Vector3 dragDelta, IInputSource inputSource = null);

        /// <summary>
        /// Raise the pointer drag event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="draggedAction">The action associated with this event.</param>
        /// <param name="dragDelta">The distance this pointer has been moved since the last time the dragged event was last raised.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerDrag(IInteractor pointer, InputAction draggedAction, Vector3 dragDelta, IInputSource inputSource = null);

        /// <summary>
        /// Raise the pointer drag end event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="draggedAction">The action associated with this event.</param>
        /// <param name="dragDelta">The distance this pointer has been moved since the last time the dragged event was last raised.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerDragEnd(IInteractor pointer, InputAction draggedAction, Vector3 dragDelta, IInputSource inputSource = null);

        #endregion Pointer Dragging

        #endregion Pointers

        #region Generic Input Events

        #region Input Down

        /// <summary>
        /// Raise the input down event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputDown(IInputSource source, InputAction inputAction);

        /// <summary>
        /// Raise the input down event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputDown(IInputSource source, Handedness handedness, InputAction inputAction);

        #endregion Input Down

        #region Input Pressed

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputPressed(IInputSource source, InputAction inputAction);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputPressed(IInputSource source, Handedness handedness, InputAction inputAction);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="pressAmount"></param>
        void RaiseOnInputPressed(IInputSource source, InputAction inputAction, float pressAmount);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="pressAmount"></param>
        void RaiseOnInputPressed(IInputSource source, Handedness handedness, InputAction inputAction, float pressAmount);

        #endregion Input Pressed

        #region Input Up

        /// <summary>
        /// Raise the input up event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputUp(IInputSource source, InputAction inputAction);

        /// <summary>
        /// Raise the input up event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputUp(IInputSource source, Handedness handedness, InputAction inputAction);

        #endregion Input Up

        #region Input Position Changed

        /// <summary>
        /// Raise the 1st degree of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IInputSource source, InputAction inputAction, float position);

        /// <summary>
        /// Raise the 1st degree of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IInputSource source, Handedness handedness, InputAction inputAction, float position);

        /// <summary>
        /// Raise the 2 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IInputSource source, InputAction inputAction, Vector2 position);

        /// <summary>
        /// Raise the 2 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IInputSource source, Handedness handedness, InputAction inputAction, Vector2 position);

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IInputSource source, InputAction inputAction, Vector3 position);

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IInputSource source, Handedness handedness, InputAction inputAction, Vector3 position);

        #endregion Input Position Changed

        #region Input Rotation Changed

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="rotation"></param>
        void RaiseRotationInputChanged(IInputSource source, InputAction inputAction, Quaternion rotation);

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="rotation"></param>
        void RaiseRotationInputChanged(IInputSource source, Handedness handedness, InputAction inputAction, Quaternion rotation);

        #endregion Input Rotation Changed

        #region Input Pose Changed

        /// <summary>
        /// Raise the 6 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="inputData"></param>
        void RaisePoseInputChanged(IInputSource source, InputAction inputAction, Pose inputData);

        /// <summary>
        /// Raise the 6 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="inputData"></param>
        void RaisePoseInputChanged(IInputSource source, Handedness handedness, InputAction inputAction, Pose inputData);

        #endregion Input Pose Changed

        #endregion Generic Input Events

        #region Generic Gesture Events

        /// <summary>
        /// Raise the Gesture Started Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        void RaiseGestureStarted(IController controller, InputAction action);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        void RaiseGestureUpdated(IController controller, InputAction action);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureUpdated(IController controller, InputAction action, Vector2 inputData);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureUpdated(IController controller, InputAction action, Vector3 inputData);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureUpdated(IController controller, InputAction action, Quaternion inputData);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureUpdated(IController controller, InputAction action, Pose inputData);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        void RaiseGestureCompleted(IController controller, InputAction action);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureCompleted(IController controller, InputAction action, Vector2 inputData);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureCompleted(IController controller, InputAction action, Vector3 inputData);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureCompleted(IController controller, InputAction action, Quaternion inputData);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureCompleted(IController controller, InputAction action, Pose inputData);

        /// <summary>
        /// Raise the Gesture Canceled Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        void RaiseGestureCanceled(IController controller, InputAction action);

        #endregion

        #region Speech Keyword Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="confidence"></param>
        /// <param name="phraseDuration"></param>
        /// <param name="phraseStartTime"></param>
        /// <param name="text"></param>
        void RaiseSpeechCommandRecognized(IInputSource source, InputAction inputAction, RecognitionConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, string text);

        #endregion Speech Keyword Events

        #region Dictation Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dictationHypothesis"></param>
        /// <param name="dictationAudioClip"></param>
        void RaiseDictationHypothesis(IInputSource source, string dictationHypothesis, AudioClip dictationAudioClip = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dictationResult"></param>
        /// <param name="dictationAudioClip"></param>
        void RaiseDictationResult(IInputSource source, string dictationResult, AudioClip dictationAudioClip = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dictationResult"></param>
        /// <param name="dictationAudioClip"></param>
        void RaiseDictationComplete(IInputSource source, string dictationResult, AudioClip dictationAudioClip);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dictationResult"></param>
        /// <param name="dictationAudioClip"></param>
        void RaiseDictationError(IInputSource source, string dictationResult, AudioClip dictationAudioClip = null);

        #endregion Dictation Events

        #endregion Input Events
    }
}