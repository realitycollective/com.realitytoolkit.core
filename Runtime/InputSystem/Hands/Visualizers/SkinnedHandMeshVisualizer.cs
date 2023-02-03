// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.EventDatum.Input;
using RealityToolkit.InputSystem.Interfaces.Handlers;
using RealityToolkit.Services.InputSystem.Utilities;
using UnityEngine;

namespace RealityToolkit.InputSystem.Hands.Visualizers
{
    /// <summary>
    /// This <see cref="IMixedRealityControllerVisualizer"/> will visualize the <see cref="InputSystem.Interfaces.Controllers.IMixedRealityController"/>
    /// it represnts using a <see cref="SkinnedMeshRenderer"/>. For <see cref="IMixedRealityHandController"/>s
    /// the visualizer is driven using <see cref="TrackedHandJoint"/> information. For regular <see cref="InputSystem.Interfaces.Controllers.IMixedRealityController"/>s
    /// the <see cref="SkinnedMeshRenderer"/> is transformed using pre-recorded hand poses depending on the controller's input state.
    /// </summary>
    [System.Runtime.InteropServices.Guid("4d1e8487-db18-4c13-ba62-eef9b7754985")]
    public class SkinnedHandMeshVisualizer : ControllerPoseSynchronizer, IMixedRealityControllerVisualizer
    {
        private SkinnedMeshRenderer skinnedMeshRenderer;
        private HandSkeleton skeleton;

        /// <inheritdoc />
        public GameObject GameObject => gameObject;

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();

            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>(true);
            Debug.Assert(skinnedMeshRenderer.IsNotNull(), $"{nameof(SkinnedHandMeshVisualizer)} can only work with a {nameof(SkinnedMeshRenderer)} in the object hierarchy.");

            skeleton = gameObject.EnsureComponent<HandSkeleton>();
            if (!skeleton.IsSetUp)
            {
                skeleton.TryAutoSetup();
            }

            Debug.Assert(skeleton.IsSetUp, $"{nameof(SkinnedHandMeshVisualizer)} failed to set up {nameof(HandSkeleton)}. Did you follow naming conventions on the hand mesh?");
        }

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            base.OnInputChanged(eventData);

            if (eventData.SourceId != Controller?.InputSource.SourceId)
            {
                return;
            }

            if (Controller is IHandController handController)
            {
                TrackingState = handController.TrackingState;

                for (int i = 0; i < HandData.JointCount; i++)
                {
                    var handJoint = (TrackedHandJoint)i;
                    if (handController.TryGetJointPose(handJoint, out var jointPose))
                    {
                        skeleton.Set(handJoint, jointPose);
                    }
                }
            }
        }
    }
}