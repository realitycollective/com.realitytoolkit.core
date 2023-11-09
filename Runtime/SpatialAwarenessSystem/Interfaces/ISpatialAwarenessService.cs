// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Interfaces;
using RealityToolkit.SpatialAwareness.Definitions;
using RealityToolkit.SpatialAwareness.Interfaces.SpatialObservers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.SpatialAwareness.Interfaces
{
    /// <summary>
    /// The interface definition for Spatial Awareness features in the Reality Toolkit.
    /// </summary>
    public interface ISpatialAwarenessService : IEventService, IRealityToolkitService
    {
        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the spatial awareness system created scene objects.
        /// </summary>
        GameObject SpatialAwarenessRootParent { get; }

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the system created <see cref="SpatialMeshObject"/>s.
        /// </summary>
        GameObject SpatialMeshesParent { get; }

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the system created mesh objects.
        /// </summary>
        GameObject SurfacesParent { get; }

        /// <summary>
        /// Gets or sets the provided mesh <see cref="SpatialMeshDisplayOptions"/> on all the <see cref="DetectedSpatialObservers"/>
        /// </summary>
        /// <remarks>
        /// Is is possible to override any previously set display options on any specific observers.
        /// </remarks>
        SpatialMeshDisplayOptions SpatialMeshVisibility { get; set; }

        #region Observers Utilities

        /// <summary>
        /// Indicates the current running state of the spatial awareness observer.
        /// </summary>
        bool IsObserverRunning(ISpatialAwarenessServiceModule observer);

        /// <summary>
        /// Generates a new unique observer id.<para/>
        /// </summary>
        /// <remarks>All <see cref="ISpatialAwarenessServiceModule"/>s are required to call this method in their initialization.</remarks>
        /// <returns>a new unique Id for the observer.</returns>
        uint GenerateNewObserverId();

        /// <summary>
        /// Starts / restarts the spatial observer.
        /// </summary>
        /// <remarks>This will cause spatial awareness events to resume.</remarks>
        void StartObserver(ISpatialAwarenessServiceModule observer);

        /// <summary>
        /// Stops / pauses the spatial observer.
        /// </summary>
        /// <remarks>This will cause spatial awareness events to be suspended until ResumeObserver is called.</remarks>
        void SuspendObserver(ISpatialAwarenessServiceModule observer);

        /// <summary>
        /// List of the spatial observers as detected by the spatial awareness system.
        /// </summary>
        HashSet<ISpatialAwarenessServiceModule> DetectedSpatialObservers { get; }

        #endregion Observer Utilities

        #region Observer Events

        /// <summary>
        /// Raise the event that a <see cref="ISpatialAwarenessServiceModule"/> has been detected.
        /// </summary>
        void RaiseSpatialAwarenessObserverDetected(ISpatialAwarenessServiceModule observer);

        /// <summary>
        /// Raise the event that a <see cref="ISpatialAwarenessServiceModule"/> has been lost.
        /// </summary>
        void RaiseSpatialAwarenessObserverLost(ISpatialAwarenessServiceModule observer);

        #endregion Observer Events

        #region Mesh Events

        /// <summary>
        /// The spatial awareness system will call the <see cref="ISpatialAwarenessMeshHandler{T}.OnMeshAdded"/> method to indicate a mesh has been added.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="meshObject">The mesh <see cref="SpatialMeshObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="ISpatialMeshObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshAdded(ISpatialMeshObserver observer, SpatialMeshObject meshObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="ISpatialAwarenessMeshHandler{T}.OnMeshUpdated"/> method to indicate an existing mesh has been updated.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="meshObject">The mesh <see cref="SpatialMeshObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="ISpatialMeshObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshUpdated(ISpatialMeshObserver observer, SpatialMeshObject meshObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="ISpatialAwarenessMeshHandler{T}.OnMeshUpdated"/> method to indicate an existing mesh has been removed.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="meshObject">The mesh <see cref="SpatialMeshObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="ISpatialMeshObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshRemoved(ISpatialMeshObserver observer, SpatialMeshObject meshObject);

        #endregion Mesh Events

        #region Surface Finding Events

        /// <summary>
        /// The spatial awareness system will call the <see cref="ISpatialAwarenessSurfaceFindingHandler{T}.OnSurfaceAdded"/> method to indicate a planar surface has been added.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <param name="surfaceObject">The surface <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="ISpatialSurfaceObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceAdded(ISpatialSurfaceObserver observer, Guid surfaceId, GameObject surfaceObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="ISpatialAwarenessSurfaceFindingHandler{T}.OnSurfaceUpdated"/> method to indicate an existing planar surface has been updated.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <param name="surfaceObject">The surface <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="ISpatialSurfaceObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceUpdated(ISpatialSurfaceObserver observer, Guid surfaceId, GameObject surfaceObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="ISpatialAwarenessSurfaceFindingHandler{T}.OnSurfaceUpdated"/> method to indicate an existing planar surface has been removed.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="ISpatialSurfaceObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceRemoved(ISpatialSurfaceObserver observer, Guid surfaceId);

        #endregion Surface Finding Events
    }
}
