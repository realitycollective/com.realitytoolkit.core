// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.EventDatum.SpatialAwareness;
using RealityToolkit.SpatialAwareness.Definitions;
using RealityToolkit.SpatialAwareness.Interfaces;
using RealityToolkit.SpatialAwareness.Interfaces.Handlers;
using RealityToolkit.SpatialAwareness.Interfaces.SpatialObservers;
using RealityToolkit.SpatialAwareness.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityToolkit.SpatialAwareness
{
    /// <summary>
    /// Class providing the default implementation of the <see cref="ISpatialAwarenessService"/> interface.
    /// </summary>
    [RuntimePlatform(typeof(AllPlatforms))]
    [System.Runtime.InteropServices.Guid("05EF9DDC-13C2-47D4-84C5-1C9CB6CC5C1C")]
    public class SpatialAwarenessService : BaseEventService, ISpatialAwarenessService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The service display name.</param>
        /// <param name="priority">The service initialization priority.</param>
        /// <param name="profile">The service configuration profile.</param>
        public SpatialAwarenessService(string name, uint priority, SpatialAwarenessSystemProfile profile)
            : base(name, priority, profile)
        {
            spatialMeshVisibility = profile.MeshDisplayOption;
        }

        private GameObject spatialAwarenessParent = null;

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the spatial awareness system created scene objects.
        /// </summary>
        public GameObject SpatialAwarenessRootParent => spatialAwarenessParent != null ? spatialAwarenessParent : (spatialAwarenessParent = CreateSpatialAwarenessParent);

        /// <summary>
        /// Creates the parent for spatial awareness objects so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/> to which spatial awareness created objects will be parented.
        /// </returns>
        private GameObject CreateSpatialAwarenessParent
        {
            get
            {
                var spatialAwarenessSystemObject = new GameObject("Spatial Awareness System");
                var rigTransform = Camera.main.transform.parent;
                spatialAwarenessSystemObject.transform.SetParent(rigTransform, false);
                return spatialAwarenessSystemObject;
            }
        }

        private GameObject meshParent = null;

        /// <inheritdoc />
        public GameObject SpatialMeshesParent => meshParent != null ? meshParent : (meshParent = CreateSecondGenerationParent("Meshes"));

        private GameObject surfaceParent = null;

        /// <inheritdoc />
        public GameObject SurfacesParent => surfaceParent != null ? surfaceParent : (surfaceParent = CreateSecondGenerationParent("Surfaces"));

        /// <inheritdoc />
        public SpatialMeshDisplayOptions SpatialMeshVisibility
        {
            get => spatialMeshVisibility;
            set
            {
                spatialMeshVisibility = value;

                foreach (var observer in DetectedSpatialObservers)
                {
                    if (observer is BaseSpatialMeshObserver meshObserver)
                    {
                        meshObserver.MeshDisplayOption = spatialMeshVisibility;
                    }
                }
            }
        }

        private SpatialMeshDisplayOptions spatialMeshVisibility;

        /// <summary>
        /// Creates a parent that is a child of the Spatial Awareness System parent so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/> to which spatial awareness objects will be parented.
        /// </returns>
        private GameObject CreateSecondGenerationParent(string name)
        {
            var secondGeneration = new GameObject(name);
            secondGeneration.transform.SetParent(SpatialAwarenessRootParent.transform, false);
            return secondGeneration;
        }

        #region ISpatialAwarenessSystem Implementation

        /// <inheritdoc />
        public HashSet<ISpatialAwarenessServiceModule> DetectedSpatialObservers { get; } = new HashSet<ISpatialAwarenessServiceModule>();

        /// <inheritdoc />
        public bool IsObserverRunning(ISpatialAwarenessServiceModule observer)
        {
            foreach (var detectedObserver in DetectedSpatialObservers)
            {
                if (detectedObserver.SourceId == observer.SourceId)
                {
                    return observer.IsRunning;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public uint GenerateNewObserverId()
        {
            var newId = (uint)UnityEngine.Random.Range(1, int.MaxValue);

            foreach (var observer in DetectedSpatialObservers)
            {
                if (observer.SourceId == newId)
                {
                    return GenerateNewObserverId();
                }
            }

            return newId;
        }

        /// <inheritdoc />
        public void StartObserver(ISpatialAwarenessServiceModule observer)
        {
            foreach (var spatialObserver in DetectedSpatialObservers)
            {
                if (spatialObserver.SourceId == observer.SourceId)
                {
                    spatialObserver.StartObserving();
                    break;
                }
            }
        }

        /// <inheritdoc />
        public void SuspendObserver(ISpatialAwarenessServiceModule observer)
        {
            foreach (var spatialObserver in DetectedSpatialObservers)
            {
                if (spatialObserver.SourceId == observer.SourceId)
                {
                    spatialObserver.StopObserving();
                    break;
                }
            }
        }

        /// <inheritdoc />
        public void RaiseSpatialAwarenessObserverDetected(ISpatialAwarenessServiceModule observer)
        {
            DetectedSpatialObservers.Add(observer);
        }

        /// <inheritdoc />
        public void RaiseSpatialAwarenessObserverLost(ISpatialAwarenessServiceModule observer)
        {
            DetectedSpatialObservers.Remove(observer);
        }

        #endregion ISpatialAwarenessSystem Implementation

        #region IService Implementation

        private SpatialAwarenessEventData<SpatialMeshObject> meshEventData = null;
        private SpatialAwarenessEventData<GameObject> surfaceFindingEventData = null;

        /// <inheritdoc/>
        public override void Initialize()
        {
            base.Initialize();

            if (Application.isPlaying)
            {
                var eventSystem = EventSystem.current;
                meshEventData = new SpatialAwarenessEventData<SpatialMeshObject>(eventSystem);
                surfaceFindingEventData = new SpatialAwarenessEventData<GameObject>(eventSystem);
            }
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            base.Destroy();

            if (!Application.isPlaying) { return; }

            if (spatialAwarenessParent != null)
            {
                spatialAwarenessParent.transform.DetachChildren();
                spatialAwarenessParent.Destroy();
            }

            // Detach the mesh objects (they are to be cleaned up by the observer) and cleanup the parent
            if (meshParent != null)
            {
                meshParent.transform.DetachChildren();
                meshParent.Destroy();
            }

            // Detach the surface objects (they are to be cleaned up by the observer) and cleanup the parent
            if (surfaceParent != null)
            {
                surfaceParent.transform.DetachChildren();
                surfaceParent.Destroy();
            }
        }

        #region Mesh Events

        /// <inheritdoc />
        public void RaiseMeshAdded(ISpatialMeshObserver observer, SpatialMeshObject spatialMeshObject)
        {
            // Parent the mesh object
            spatialMeshObject.GameObject.transform.parent = SpatialMeshesParent.transform;

            meshEventData.Initialize(observer, spatialMeshObject.Id, spatialMeshObject);
            HandleEvent(meshEventData, OnMeshAdded);
        }

        /// <summary>
        /// Event sent whenever a mesh is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<ISpatialAwarenessMeshHandler<SpatialMeshObject>> OnMeshAdded =
            delegate (ISpatialAwarenessMeshHandler<SpatialMeshObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<SpatialAwarenessEventData<SpatialMeshObject>>(eventData);
                handler.OnMeshAdded(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseMeshUpdated(ISpatialMeshObserver observer, SpatialMeshObject spatialMeshObject)
        {
            // Parent the mesh object
            spatialMeshObject.GameObject.transform.parent = SpatialMeshesParent.transform;

            meshEventData.Initialize(observer, spatialMeshObject.Id, spatialMeshObject);
            HandleEvent(meshEventData, OnMeshUpdated);
        }

        /// <summary>
        /// Event sent whenever a mesh is updated.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<ISpatialAwarenessMeshHandler<SpatialMeshObject>> OnMeshUpdated =
            delegate (ISpatialAwarenessMeshHandler<SpatialMeshObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<SpatialAwarenessEventData<SpatialMeshObject>>(eventData);
                handler.OnMeshUpdated(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseMeshRemoved(ISpatialMeshObserver observer, SpatialMeshObject spatialMeshObject)
        {
            meshEventData.Initialize(observer, spatialMeshObject.Id, spatialMeshObject);
            HandleEvent(meshEventData, OnMeshRemoved);
        }

        /// <summary>
        /// Event sent whenever a mesh is discarded.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<ISpatialAwarenessMeshHandler<SpatialMeshObject>> OnMeshRemoved =
            delegate (ISpatialAwarenessMeshHandler<SpatialMeshObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<SpatialAwarenessEventData<SpatialMeshObject>>(eventData);
                handler.OnMeshRemoved(spatialEventData);
            };

        #endregion Mesh Events

        #region Surface Finding Events

        /// <inheritdoc />
        public void RaiseSurfaceAdded(ISpatialSurfaceObserver observer, Guid surfaceId, GameObject surfaceObject)
        {
            surfaceFindingEventData.Initialize(observer, surfaceId, surfaceObject);
            HandleEvent(surfaceFindingEventData, OnSurfaceAdded);
        }

        /// <summary>
        /// Event sent whenever a planar surface is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<ISpatialAwarenessSurfaceFindingHandler<GameObject>> OnSurfaceAdded =
            delegate (ISpatialAwarenessSurfaceFindingHandler<GameObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<SpatialAwarenessEventData<GameObject>>(eventData);
                handler.OnSurfaceAdded(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseSurfaceUpdated(ISpatialSurfaceObserver observer, Guid surfaceId, GameObject surfaceObject)
        {
            surfaceFindingEventData.Initialize(observer, surfaceId, surfaceObject);
            HandleEvent(surfaceFindingEventData, OnSurfaceUpdated);
        }

        /// <summary>
        /// Event sent whenever a planar surface is updated.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<ISpatialAwarenessSurfaceFindingHandler<GameObject>> OnSurfaceUpdated =
            delegate (ISpatialAwarenessSurfaceFindingHandler<GameObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<SpatialAwarenessEventData<GameObject>>(eventData);
                handler.OnSurfaceUpdated(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseSurfaceRemoved(ISpatialSurfaceObserver observer, Guid surfaceId)
        {
            surfaceFindingEventData.Initialize(observer, surfaceId, null);
            HandleEvent(surfaceFindingEventData, OnSurfaceRemoved);
        }

        /// <summary>
        /// Event sent whenever a planar surface is discarded.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<ISpatialAwarenessSurfaceFindingHandler<GameObject>> OnSurfaceRemoved =
            delegate (ISpatialAwarenessSurfaceFindingHandler<GameObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<SpatialAwarenessEventData<GameObject>>(eventData);
                handler.OnSurfaceRemoved(spatialEventData);
            };

        #endregion Surface Finding Events

        #endregion IService Implementation
    }
}
