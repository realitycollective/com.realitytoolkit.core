// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Devices;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.Input.Interfaces.Handlers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityToolkit.Input.Handlers
{
    internal static class InputServiceEventHandlers
    {
        #region ExecuteEvents Redirect

        internal static GameObject Execute<T>(GameObject root, BaseEventData eventData, ExecuteEvents.EventFunction<T> callbackFunction) where T : IEventSystemHandler
        {
            return ExecuteEvents.ExecuteHierarchy(root, eventData, callbackFunction);
        }

        #endregion ExecuteEvents Redirect

        #region Input Source State Events

        internal static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceDetectedEventHandler =
            delegate (ISourceStateHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceDetected(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<ISourceStateHandler> OnSourceLostEventHandler =
            delegate (ISourceStateHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceLost(casted);
            };

        #endregion Input Source State Events

        #region Input Source Pose Events

        internal static readonly ExecuteEvents.EventFunction<ISourcePoseHandler> OnSourceTrackingChangedEventHandler =
            delegate (ISourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<TrackingState>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };


        internal static readonly ExecuteEvents.EventFunction<ISourcePoseHandler> OnSourcePoseVector2ChangedEventHandler =
            delegate (ISourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<Vector2>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };


        internal static readonly ExecuteEvents.EventFunction<ISourcePoseHandler> OnSourcePositionChangedEventHandler =
            delegate (ISourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<Vector3>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };


        internal static readonly ExecuteEvents.EventFunction<ISourcePoseHandler> OnSourceRotationChangedEventHandler =
            delegate (ISourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<Quaternion>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<ISourcePoseHandler> OnSourcePoseChangedEventHandler =
            delegate (ISourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<Pose>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };

        #endregion Input Source Pose Events

        #region Focus Events

        internal static readonly ExecuteEvents.EventFunction<IFocusChangedHandler> OnPreFocusChangedHandler =
            delegate (IFocusChangedHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnBeforeFocusChange(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IFocusChangedHandler> OnFocusChangedHandler =
            delegate (IFocusChangedHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnFocusChanged(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IFocusHandler> OnFocusEnterEventHandler =
            delegate (IFocusHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnFocusEnter(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IFocusHandler> OnFocusExitEventHandler =
            delegate (IFocusHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnFocusExit(casted);
            };

        #endregion Focus Events

        #region Pointers

        internal static readonly ExecuteEvents.EventFunction<IPointerHandler> OnPointerDownEventHandler =
            delegate (IPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<EventDatum.Input.PointerEventData>(eventData);
                handler.OnPointerDown(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IPointerHandler> OnInputClickedEventHandler =
            delegate (IPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<EventDatum.Input.PointerEventData>(eventData);
                handler.OnPointerClicked(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IPointerHandler> OnPointerUpEventHandler =
            delegate (IPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<EventDatum.Input.PointerEventData>(eventData);
                handler.OnPointerUp(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IPointerScrollHandler> OnPointerScroll =
            delegate (IPointerScrollHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<PointerScrollEventData>(eventData);
                handler.OnPointerScroll(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IPointerDragHandler> OnPointerDragBegin =
            delegate (IPointerDragHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<PointerDragEventData>(eventData);
                handler.OnPointerDragBegin(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IPointerDragHandler> OnPointerDrag =
            delegate (IPointerDragHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<PointerDragEventData>(eventData);
                handler.OnPointerDrag(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IPointerDragHandler> OnPointerDragEnd =
            delegate (IPointerDragHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<PointerDragEventData>(eventData);
                handler.OnPointerDragEnd(casted);
            };
        #endregion Pointers

        #region Generic Input Events

        internal static readonly ExecuteEvents.EventFunction<IInputHandler> OnInputDownEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputDown(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IInputHandler> OnInputUpEventHandler =
            delegate (IInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputUp(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IInputHandler<float>> SingleAxisInputEventHandler =
            delegate (IInputHandler<float> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<float>>(eventData);
                handler.OnInputChanged(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IInputHandler<Vector2>> OnTwoDoFInputChanged =
            delegate (IInputHandler<Vector2> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector2>>(eventData);
                handler.OnInputChanged(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IInputHandler<Vector3>> OnPositionInputChanged =
            delegate (IInputHandler<Vector3> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnInputChanged(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IInputHandler<Quaternion>> OnRotationInputChanged =
            delegate (IInputHandler<Quaternion> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Quaternion>>(eventData);
                handler.OnInputChanged(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IInputHandler<Pose>> OnPoseInputChanged =
            delegate (IInputHandler<Pose> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Pose>>(eventData);
                handler.OnInputChanged(casted);
            };
        #endregion Generic Input Events

        #region Gesture Events

        internal static readonly ExecuteEvents.EventFunction<IGestureHandler> OnGestureStarted =
            delegate (IGestureHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnGestureStarted(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IGestureHandler> OnGestureUpdated =
            delegate (IGestureHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnGestureUpdated(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IGestureHandler<Vector2>> OnGestureVector2PositionUpdated =
            delegate (IGestureHandler<Vector2> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector2>>(eventData);
                handler.OnGestureUpdated(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IGestureHandler<Vector3>> OnGesturePositionUpdated =
            delegate (IGestureHandler<Vector3> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnGestureUpdated(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IGestureHandler<Quaternion>> OnGestureRotationUpdated =
            delegate (IGestureHandler<Quaternion> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Quaternion>>(eventData);
                handler.OnGestureUpdated(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IGestureHandler<Pose>> OnGesturePoseUpdated =
            delegate (IGestureHandler<Pose> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Pose>>(eventData);
                handler.OnGestureUpdated(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IGestureHandler> OnGestureCompleted =
            delegate (IGestureHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnGestureCompleted(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IGestureHandler<Vector2>> OnGestureVector2PositionCompleted =
            delegate (IGestureHandler<Vector2> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector2>>(eventData);
                handler.OnGestureCompleted(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IGestureHandler<Vector3>> OnGesturePositionCompleted =
            delegate (IGestureHandler<Vector3> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnGestureCompleted(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IGestureHandler<Quaternion>> OnGestureRotationCompleted =
            delegate (IGestureHandler<Quaternion> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Quaternion>>(eventData);
                handler.OnGestureCompleted(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IGestureHandler<Pose>> OnGesturePoseCompleted =
            delegate (IGestureHandler<Pose> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Pose>>(eventData);
                handler.OnGestureCompleted(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IGestureHandler> OnGestureCanceled =
            delegate (IGestureHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnGestureCanceled(casted);
            };
        #endregion Gesture Events

        #region Speech Keyword Events

        internal static readonly ExecuteEvents.EventFunction<ISpeechHandler> OnSpeechKeywordRecognizedEventHandler =
            delegate (ISpeechHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SpeechEventData>(eventData);
                handler.OnSpeechKeywordRecognized(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationHypothesisEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationHypothesis(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationResultEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationResult(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationCompleteEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationComplete(casted);
            };

        internal static readonly ExecuteEvents.EventFunction<IDictationHandler> OnDictationErrorEventHandler =
            delegate (IDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationError(casted);
            };
        #endregion Speech Keyword Events
    }
}