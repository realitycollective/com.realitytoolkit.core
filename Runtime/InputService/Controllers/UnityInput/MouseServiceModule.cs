// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers.UnityInput.Profiles;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Input.Interfaces;
using RealityToolkit.Utilities.Physics;
using System;
using UnityEngine;

namespace RealityToolkit.Input.Controllers.UnityInput
{
    /// <summary>
    /// The mouse service module.
    /// </summary>
    [System.Runtime.InteropServices.Guid("067CE7D4-8277-4E18-834E-3DC712074B72")]
    public class MouseServiceModule : BaseControllerServiceModule
    {
        /// <inheritdoc />
        public MouseServiceModule(string name, uint priority, MouseControllerServiceModuleProfile profile, IInputService parentService)
            : base(name, priority, profile, parentService)
        {
        }

        /// <summary>
        /// Current Mouse Controller.
        /// </summary>
        public MouseController Controller { get; private set; }

        /// <inheritdoc />
        public override void Enable()
        {
            if (MouseController.IsInGameWindow && Controller == null)
            {
                CreateController();
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();
            Controller?.Update();
        }

        /// <inheritdoc />
        public override void OnApplicationFocus(bool isFocused)
        {
            base.OnApplicationFocus(isFocused);

            if (Controller != null)
            {
                Cursor.visible = !isFocused;
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            if (Controller != null)
            {
                DestroyController();
            }
        }

        private void CreateController()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorWindow.focusedWindow != null)
            {
                UnityEditor.EditorWindow.focusedWindow.ShowNotification(new GUIContent("Press \"ESC\" to regain mouse control"));
            }
#endif

            Cursor.visible = false;
            Raycaster.DebugEnabled = true;

            try
            {
                Controller = new MouseController(this, TrackingState.NotApplicable, Handedness.Any, GetControllerMappingProfile(typeof(MouseController), Handedness.Any));
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create {nameof(MouseController)}!\n{e}");
                return;
            }

            InputSystem?.RaiseSourceDetected(Controller.InputSource, Controller);
            AddController(Controller);
        }

        private void DestroyController()
        {
            InputSystem?.RaiseSourceLost(Controller.InputSource, Controller);
            RemoveController(Controller);
        }
    }
}
