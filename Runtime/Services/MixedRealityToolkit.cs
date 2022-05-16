// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Extensions;
using RealityToolkit.Interfaces;
using RealityToolkit.Interfaces.Events;
using RealityToolkit.ServiceFramework.Definitions;
using RealityToolkit.ServiceFramework.Interfaces;
using RealityToolkit.ServiceFramework.Services;
using RealityToolkit.Utilities;
using RealityToolkit.Utilities.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RealityToolkit.Services
{
    /// <summary>
    /// This class is responsible for coordinating the operation of the Mixed Reality Toolkit. It is the only Singleton in the entire project.
    /// It provides a service registry for all active services that are used within a project as well as providing the active profile for the project.
    /// The <see cref="ActiveProfile"/> can be swapped out at any time to meet the needs of your project.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public sealed class MixedRealityToolkit : MonoBehaviour, IDisposable
    {
        private static Type[] serviceInterfaceTypes = new[] { typeof(IMixedRealityEventSystem), typeof(IMixedRealitySystem) };

        private static ServiceManager serviceManagerInstance;

        internal static ServiceManager ServiceManagerInstance
        {
            get
            {
                if (serviceManagerInstance == null)
                {
                    serviceManagerInstance = new ServiceManager(additionalBaseServiceTypes: serviceInterfaceTypes);
                    serviceManagerInstance.Initialize(Instance.gameObject);
                }
                return serviceManagerInstance;
            }
        }

        #region Mixed Reality Toolkit Profile properties

        /// <summary>
        /// Checks if there is a valid instance of the MixedRealityToolkit, then checks if there is there a valid Active Profile.
        /// </summary>
        public static bool HasActiveProfile
        {
            get
            {
                if (!IsInitialized)
                {
                    return false;
                }

                if (!ConfirmInitialized())
                {
                    return false;
                }

                return Instance.ActiveProfile != null;
            }
        }

        public static string DefaultXRCameraRigName = "XRCameraRig";

        /// <summary>
        /// The active profile of the Mixed Reality Toolkit which controls which services are active and their initial settings.
        /// *Note a profile is used on project initialization or replacement, changes to properties while it is running has no effect.
        /// </summary>
        [SerializeField]
        [Tooltip("The current active settings for the Mixed Reality project")]
        private ServiceProvidersProfile activeProfile;

        /// <summary>
        /// The public property of the Active Profile, ensuring events are raised on the change of the reference
        /// </summary>
        public ServiceProvidersProfile ActiveProfile { get => ServiceManagerInstance.ActiveProfile; set => ServiceManagerInstance.ActiveProfile = value; }

        /// <summary>
        /// The list of active platforms detected by the <see cref="MixedRealityToolkit"/>.
        /// </summary>
        public static IReadOnlyList<IPlatform> AvailablePlatforms => ServiceManager.AvailablePlatforms;

        /// <summary>
        /// The list of active platforms detected by the <see cref="MixedRealityToolkit"/>.
        /// </summary>
        public static IReadOnlyList<IPlatform> ActivePlatforms => ServiceManager.AvailablePlatforms;

        /// <summary>
        /// Check which platforms are active and available.
        /// </summary>
        internal static void CheckPlatforms() => ServiceManager.CheckPlatforms();

        /// <summary>
        /// When a profile is replaced with a new one, force all services to reset and read the new values
        /// </summary>
        /// <param name="profile"></param>
        public void ResetProfile(ServiceProvidersProfile profile)
        {
            ServiceManagerInstance.ResetProfile(profile, instance.gameObject);
        }

        private static bool isResetting = false;

        #endregion Mixed Reality Toolkit Profile properties

        #region Instance Management

        private static bool isGettingInstance = false;

        /// <summary>
        /// Returns the Singleton instance of the classes type.
        /// If no instance is found, then we search for an instance in the scene.
        /// If more than one instance is found, we log an error and no instance is returned.
        /// </summary>
        public static MixedRealityToolkit Instance
        {
            get
            {
                if (IsInitialized)
                {
                    return instance;
                }

                if (isGettingInstance ||
                   (Application.isPlaying && !searchForInstance))
                {
                    return null;
                }

                isGettingInstance = true;

                var objects = FindObjectsOfType<MixedRealityToolkit>();
                searchForInstance = false;
                MixedRealityToolkit newInstance;

                switch (objects.Length)
                {
                    case 0:
                        newInstance = new GameObject(nameof(MixedRealityToolkit)).AddComponent<MixedRealityToolkit>();
                        break;
                    case 1:
                        newInstance = objects[0];
                        break;
                    default:
                        Debug.LogError($"Expected exactly 1 {nameof(MixedRealityToolkit)} but found {objects.Length}.");
                        isGettingInstance = false;
                        return null;
                }

                if (newInstance == null)
                {
                    Debug.LogError("Failed to get instance!");
                    isGettingInstance = false;
                    return null;
                }

                if (!IsApplicationQuitting)
                {
                    // Setup any additional things the instance needs.
                    newInstance.InitializeInstance();
                }
                else
                {
                    // Don't do any additional setup because the app is quitting.
                    instance = newInstance;
                }

                if (instance == null)
                {
                    Debug.LogError("Failed to get instance!");
                    isGettingInstance = false;
                    return null;
                }

                isGettingInstance = false;
                return instance;
            }
        }

        public static IReadOnlyDictionary<Type, IService> ActiveSystems => ServiceManagerInstance.ActiveServices;
        private static MixedRealityToolkit instance;

        /// <summary>
        /// Lock property for the Mixed Reality Toolkit to prevent reinitialization
        /// </summary>
        private static readonly object InitializedLock = new object();

        private void InitializeInstance()
        {
            lock (InitializedLock)
            {
                if (IsInitialized) { return; }

                instance = this;

                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(instance.transform.root);
                }

                Application.quitting += () =>
                {
                    ServiceManagerInstance.DisableAllServices();
                    ServiceManagerInstance.DestroyAllServices();
                    IsApplicationQuitting = true;
                };

#if UNITY_EDITOR
                UnityEditor.EditorApplication.hierarchyChanged += OnHierarchyChanged;
                UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

                void OnHierarchyChanged()
                {
                    if (instance != null)
                    {
                        Debug.Assert(instance.transform.parent == null, $"The {nameof(MixedRealityToolkit)} should not be parented under any other GameObject!");
                        Debug.Assert(instance.transform.childCount == 0, $"The {nameof(MixedRealityToolkit)} should not have GameObject children!");
                    }
                }

                void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange playModeState)
                {
                    switch (playModeState)
                    {
                        case UnityEditor.PlayModeStateChange.EnteredEditMode:
                            IsApplicationQuitting = false;
                            break;
                        case UnityEditor.PlayModeStateChange.ExitingEditMode:
                            if (ActiveProfile.IsNull())
                            {
                                Debug.LogError($"{nameof(MixedRealityToolkit)} has no active profile! Exiting playmode...");
                                UnityEditor.EditorApplication.isPlaying = false;
                                UnityEditor.Selection.activeObject = Instance;
                                UnityEditor.EditorApplication.delayCall += () =>
                                    UnityEditor.EditorGUIUtility.PingObject(Instance);
                            }
                            break;
                        case UnityEditor.PlayModeStateChange.EnteredPlayMode:
                        case UnityEditor.PlayModeStateChange.ExitingPlayMode:
                            // Nothing for now.
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(playModeState), playModeState, null);
                    }

                }
#endif // UNITY_EDITOR

                // if the Toolkit has a profile, validate toolkit requirements and initialize services
                if (HasActiveProfile)
                {
                    EnsureMixedRealityRequirements();
                    ServiceManagerInstance.InitializeServiceManager();
                }
            }
        }

        /// <summary>
        /// Flag to search for instance the first time Instance property is called.
        /// Subsequent attempts will generally switch this flag false, unless the instance was destroyed.
        /// </summary>
        private static bool searchForInstance = true;

        private static bool isInitializing = false;

        /// <summary>
        /// Flag stating if the application is currently attempting to quit.
        /// </summary>
        public static bool IsApplicationQuitting { get; private set; } = false;

        /// <summary>
        /// Expose an assertion whether the MixedRealityToolkit class is initialized.
        /// </summary>
        public static void AssertIsInitialized()
        {
            Debug.Assert(IsInitialized, $"The {nameof(MixedRealityToolkit)} has not been initialized.");
        }

        /// <summary>
        /// Returns whether the instance has been initialized or not.
        /// </summary>
        public static bool IsInitialized => instance != null;

        /// <summary>
        /// Static function to determine if the <see cref="MixedRealityToolkit"/> class has been initialized or not.
        /// </summary>
        internal static bool ConfirmInitialized()
        {
            var access = Instance;
            Debug.Assert(IsInitialized.Equals(access != null));
            return IsInitialized;
        }

        private static void EnsureMixedRealityRequirements()
        {
            // There's lots of documented cases that if the camera doesn't start at 0,0,0, things break with the WMR SDK specifically.
            // We'll enforce that here, then tracking can update it to the appropriate position later.
            CameraCache.Main.transform.position = Vector3.zero;

            // Validate the CameraRig is setup with the main camera as a child of the rig
            EnsureCameraRig();

            // We need at least one instance of the event system to be active.
            EnsureEventSystemSetup();
        }

        private static void EnsureCameraRig()
        {
            if (CameraCache.Main.transform.parent.IsNull())
            {
                var rigTransform = new GameObject(MixedRealityToolkit.DefaultXRCameraRigName).transform;
                CameraCache.Main.transform.SetParent(rigTransform);
                Debug.Log($"There was no {MixedRealityToolkit.DefaultXRCameraRigName} in the scene. The {nameof(MixedRealityToolkit)} requires one and added it, as well as making the main camera a child of the rig.");
            }
        }

        private static void EnsureEventSystemSetup()
        {
            var eventSystems = FindObjectsOfType<EventSystem>();
            if (eventSystems.Length == 0)
            {
                CameraCache.Main.gameObject.EnsureComponent<EventSystem>();
                Debug.Log($"There was no {nameof(EventSystem)} in the scene. The {nameof(MixedRealityToolkit)} requires one and added it to the main camera.");
            }
            else if (eventSystems.Length > 1)
            {
                Debug.LogError($"There is more than one {nameof(EventSystem)} active in the scene. Please make sure only one instance of it exists as it may cause errors.");
            }
        }

        #endregion Instance Management

        #region Service Container Management

        #region Registration

        /// <summary>
        /// Registers all the <see cref="IMixedRealityService"/>s defined in the provided configuration collection.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealityService"/> to be registered.</typeparam>
        /// <param name="configurations">The list of <see cref="IMixedRealityServiceConfiguration{T}"/>s.</param>
        /// <returns>True, if all configurations successfully created and registered their services.</returns>
        public static bool TryRegisterServiceConfigurations<T>(IServiceConfiguration<T>[] configurations) where T : IService
            => ServiceManagerInstance.TryRegisterServiceConfigurations<T>(configurations);

        /// <summary>
        /// Registers all the <see cref="IServiceDataProvider"/>s defined in the provided configuration collection.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IServiceDataProvider"/> to be registered.</typeparam>
        /// <param name="configurations">The list of <see cref="IMixedRealityServiceConfiguration{T}"/>s.</param>
        /// <param name="serviceParent">The <see cref="IMixedRealityService"/> that the <see cref="IServiceDataProvider"/> will be assigned to.</param>
        /// <returns>True, if all configurations successfully created and registered their data providers.</returns>
        public static bool TryRegisterDataProviderConfigurations<T>(IServiceConfiguration<T>[] configurations, IService serviceParent) where T : IServiceDataProvider
            => ServiceManagerInstance.TryRegisterDataProviderConfigurations<T>(configurations, serviceParent);

        /// <summary>
        /// Add a service instance to the Mixed Reality Toolkit active service registry.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealityService"/> to be registered.</typeparam>
        /// <param name="serviceInstance">Instance of the <see cref="IMixedRealityService"/> to register.</param>
        /// <returns>True, if the service was successfully registered.</returns>
        public static bool TryRegisterService<T>(IService serviceInstance) where T : IService
            => ServiceManagerInstance.TryRegisterService<T>(serviceInstance);

        /// <summary>
        /// Creates a new instance of a service and registers it to the Mixed Reality Toolkit service registry for the specified platform.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealityService"/> to be registered.</typeparam>
        /// <param name="configuration">The <see cref="IMixedRealityServiceConfiguration{T}"/> to use to create and register the service.</param>
        /// <param name="service">If successful, then the new <see cref="IMixedRealityService"/> instance will be passed back out.</param>
        /// <returns>True, if the service was successfully created and registered.</returns>
        public static bool TryCreateAndRegisterService<T>(IServiceConfiguration<T> configuration, out T service) where T : IService
            => ServiceManagerInstance.TryCreateAndRegisterService<T>(configuration, out service);

        /// <summary>
        /// Creates a new instance of a data provider and registers it to the Mixed Reality Toolkit service registry for the specified platform.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealityService"/> to be registered.</typeparam>
        /// <param name="configuration">The <see cref="IMixedRealityServiceConfiguration{T}"/> to use to create and register the data provider.</param>
        /// <param name="serviceParent">The <see cref="IMixedRealityService"/> that the <see cref="IServiceDataProvider"/> will be assigned to.</param>
        /// <returns>True, if the service was successfully created and registered.</returns>
        public static bool TryCreateAndRegisterDataProvider<T>(IServiceConfiguration<T> configuration, IService serviceParent) where T : IServiceDataProvider
            => ServiceManagerInstance.TryCreateAndRegisterDataProvider<T>(configuration, serviceParent);


        /// <summary>
        /// Creates a new instance of a service and registers it to the Mixed Reality Toolkit service registry for the specified platform.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealityService"/> to be registered.</typeparam>
        /// <param name="concreteType">The concrete class type to instantiate.</param>
        /// <param name="service">If successful, then the new <see cref="IMixedRealityService"/> instance will be passed back out.</param>
        /// <param name="args">Optional arguments used when instantiating the concrete type.</param>
        /// <returns>True, if the service was successfully created and registered.</returns>
        public static bool TryCreateAndRegisterService<T>(Type concreteType, out T service, params object[] args) where T : IService
            => ServiceManagerInstance.TryCreateAndRegisterService<T>(concreteType, out service, args);

        /// <summary>
        /// Creates a new instance of a service and registers it to the Mixed Reality Toolkit service registry for the specified platform.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealityService"/> to be registered.</typeparam>
        /// <param name="concreteType">The concrete class type to instantiate.</param>
        /// <param name="runtimePlatforms">The runtime platform to check against when registering.</param>
        /// <param name="service">If successful, then the new <see cref="IMixedRealityService"/> instance will be passed back out.</param>
        /// <param name="args">Optional arguments used when instantiating the concrete type.</param>
        /// <returns>True, if the service was successfully created and registered.</returns>
        public static bool TryCreateAndRegisterService<T>(Type concreteType, IReadOnlyList<IPlatform> runtimePlatforms, out T service, params object[] args) where T : IService
            => ServiceManagerInstance.TryCreateAndRegisterService<T>(concreteType, runtimePlatforms, out service, args);

        #endregion Registration

        #region Unregistration

        /// <summary>
        /// Remove all services from the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        public static bool TryUnregisterServicesOfType<T>() where T : IService
            => ServiceManagerInstance.TryUnregisterServicesOfType<T>();

        /// <summary>
        /// Removes a specific service with the provided name.
        /// </summary>
        /// <param name="serviceInstance">The instance of the <see cref="IMixedRealityService"/> to remove.</param>
        public static bool TryUnregisterService<T>(T serviceInstance) where T : IService
            => ServiceManagerInstance.TryUnregisterService<T>(serviceInstance);

        #endregion Unregistration

        #region Multiple Service Management

        /// <summary>
        /// Enable all services in the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be enabled.  E.G. InputSystem, BoundarySystem</typeparam>
        public static void EnableAllServicesOfType<T>() where T : IService
            => ServiceManagerInstance.EnableService<T>();

        /// <summary>
        /// Enables a single service by name.
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be enabled.  E.G. InputSystem, BoundarySystem</typeparam>
        /// <param name="serviceName"></param>
        public static void EnableService<T>(string serviceName) where T : IService
            => ServiceManagerInstance.EnableService<T>(serviceName);

        /// <summary>
        /// Disable all services in the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be enabled.  E.G. InputSystem, BoundarySystem</typeparam>
        public static void DisableAllServiceOfType<T>() where T : IService
            => ServiceManagerInstance.DisableService<T>();

        /// <summary>
        /// Disable a single service by name.
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be enabled.  E.G. InputSystem, BoundarySystem</typeparam>
        /// <param name="serviceName">Name of the specific service</param>
        public static void DisableService<T>(string serviceName) where T : IService
            => ServiceManagerInstance.DisableService<T>(serviceName);

        /// <summary>
        /// Retrieve all services from the Mixed Reality Toolkit active service registry for a given type and an optional name
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem.</typeparam>
        /// <returns>An array of services that meet the search criteria</returns>
        public static List<T> GetActiveServices<T>() where T : IService => ServiceManagerInstance.GetServices<T>();

        #endregion Multiple Service Management

        #region Service Utilities

        /// <summary>
        /// Query the <see cref="RegisteredMixedRealityServices"/> for the existence of a <see cref="IMixedRealityService"/>.
        /// </summary>
        /// <typeparam name="T">The interface type for the service to be retrieved.</typeparam>
        /// <returns>Returns true, if there is a <see cref="IMixedRealityService"/> registered, otherwise false.</returns>
        public static bool IsServiceRegistered<T>() where T : IService
            => ServiceManagerInstance.IsServiceRegistered<T>();

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="RegisteredMixedRealityServices"/>.
        /// </summary>
        /// <param name="showLogs">Should the logs show when services cannot be found?</param>
        /// <typeparam name="T">The interface type for the service to be retrieved.</typeparam>
        /// <returns>The instance of the <see cref="IMixedRealityService"/> that is registered.</returns>
        public static T GetService<T>(bool showLogs = true) where T : IService
            => ServiceManagerInstance.GetService<T>(showLogs);

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="RegisteredMixedRealityServices"/>.
        /// </summary>
        /// <param name="timeout">Optional, time out in seconds to wait before giving up search.</param>
        /// <typeparam name="T">The interface type for the service to be retrieved.</typeparam>
        /// <returns>The instance of the <see cref="IMixedRealityService"/> that is registered.</returns>
        public static async Task<T> GetServiceAsync<T>(int timeout = 10) where T : IService
            => await ServiceManagerInstance.GetService<T>().WaitUntil(system => system != null, timeout);

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="RegisteredMixedRealityServices"/>.
        /// </summary>
        /// <typeparam name="T">The interface type for the service to be retrieved.</typeparam>
        /// <param name="service">The instance of the service class that is registered.</param>
        /// <param name="showLogs">Should the logs show when services cannot be found?</param>
        /// <returns>Returns true if the <see cref="IMixedRealityService"/> was found, otherwise false.</returns>
        public static bool TryGetService<T>(out T service, bool showLogs = true) where T : IService
            => ServiceManagerInstance.TryGetService<T>(out service, showLogs);

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="RegisteredMixedRealityServices"/> by name.
        /// </summary>
        /// <param name="serviceName">Name of the specific service to search for.</param>
        /// <param name="showLogs">Should the logs show when services cannot be found?</param>
        /// <returns>The instance of the <see cref="IMixedRealityService"/> that is registered.</returns>
        public static T GetService<T>(string serviceName, bool showLogs = true) where T : IService
            => (T)ServiceManagerInstance.GetServiceByName<T>(serviceName, showLogs);

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="RegisteredMixedRealityServices"/> by name.
        /// </summary>
        /// <typeparam name="T">The interface type for the service to be retrieved.</typeparam>
        /// <param name="serviceName">Name of the specific service to search for.</param>
        /// <param name="service">The instance of the service class that is registered.</param>
        /// <param name="showLogs">Should the logs show when services cannot be found?</param>
        /// <returns>Returns true if the <see cref="IMixedRealityService"/> was found, otherwise false.</returns>
        public static bool TryGetService<T>(string serviceName, out T service, bool showLogs = true) where T : IService
            => ServiceManagerInstance.TryGetServiceByName<T>(serviceName, out service, showLogs);

        #endregion Service Utilities

        #region System Utilities

        /// <summary>
        /// Query <see cref="RegisteredMixedRealityServices"/> for the existence of a <see cref="IMixedRealitySystem"/>.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealitySystem"/> to be retrieved.</typeparam>
        /// <returns>Returns true, if there is a <see cref="IMixedRealitySystem"/> registered, otherwise false.</returns>
        public static bool IsSystemRegistered<T>() where T : IService
            => ServiceManagerInstance.IsServiceRegistered<T>();

        /// <summary>
        /// Is the <see cref="IMixedRealitySystem"/> enabled in the <see cref="ActiveProfile"/>?
        /// </summary>
        /// <typeparam name="T"><see cref="IMixedRealitySystem"/> to check.</typeparam>
        /// <param name="rootProfile">Optional root profile reference.</param>
        /// <returns>True, if the system is enabled in the <see cref="ActiveProfile"/>, otherwise false.</returns>
        public static bool IsSystemEnabled<T>(ServiceProvidersProfile rootProfile = null) where T : IService
            => ServiceManagerInstance.IsServiceEnabledInProfile<T>(rootProfile);

        /// <summary>
        /// Try to get the <see cref="TProfile"/> of the <see cref="TSystem"/>
        /// </summary>
        /// <typeparam name="TSystem">The <see cref="IMixedRealitySystem"/> type to get the <see cref="TProfile"/> for.</typeparam>
        /// <typeparam name="TProfile">The <see cref="BaseProfile"/> type to get for the <see cref="TSystem"/>.</typeparam>
        /// <param name="profile">The profile instance.</param>
        /// <param name="rootProfile">Optional root profile reference.</param>
        /// <returns>True if a <see cref="TSystem"/> type is matched and a valid <see cref="TProfile"/> is found, otherwise false.</returns>
        public static bool TryGetSystemProfile<TSystem, TProfile>(out TProfile profile, ServiceProvidersProfile rootProfile = null)
            where TSystem : IService
            where TProfile : BaseProfile
                => ServiceManagerInstance.TryGetSystemProfile<TSystem, TProfile>(out profile, rootProfile);

        private static bool IsSystem(Type concreteType)
        {
            if (concreteType == null)
            {
                Debug.LogError($"{nameof(concreteType)} is null.");
                return false;
            }

            return typeof(IMixedRealitySystem).IsAssignableFrom(concreteType);
        }

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="ActiveSystems"/>.
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.</typeparam>
        /// <returns>The instance of the <see cref="IMixedRealitySystem"/> that is registered.</returns>
        public static T GetSystemCached<T>() where T : IService
            => ServiceManagerInstance.GetServiceCached<T>();

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="ActiveSystems"/>.
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.</typeparam>
        /// <param name="timeout">Optional, time out in seconds to wait before giving up search.</param>
        /// <returns>The instance of the <see cref="IMixedRealitySystem"/> that is registered.</returns>
        public static async Task<T> GetSystemAsync<T>(int timeout = 10) where T : IService
            => await ServiceManagerInstance.GetServiceCached<T>().WaitUntil(system => system != null, timeout);

        /// <summary>
        /// Retrieve a <see cref="IMixedRealitySystem"/> from the <see cref="ActiveSystems"/>.
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.</typeparam>
        /// <param name="system">The instance of the system class that is registered.</param>
        /// <returns>Returns true if the <see cref="IMixedRealitySystem"/> was found, otherwise false.</returns>
        public static bool TryGetSystem<T>(out T system) where T : IService
            => ServiceManagerInstance.TryGetServiceCached<T>(out system);

        #endregion System Utilities

        #endregion Service Container Management

        #region IDisposable Implementation

        private bool disposed;

        ~MixedRealityToolkit()
        {
            OnDispose(true);
        }

        /// <summary>
        /// Dispose the <see cref="MixedRealityToolkit"/> object.
        /// </summary>
        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;
            GC.SuppressFinalize(this);
            OnDispose(false);
        }

        private void OnDispose(bool finalizing)
        {
            if (instance == this)
            {
                instance = null;
                searchForInstance = true;
            }
        }

        #endregion IDisposable Implementation

        #region MonoBehaviour Implementation
#if UNITY_EDITOR
        private void OnValidate() => ServiceManagerInstance?.OnValidate();
#endif // UNITY_EDITOR

        private void Awake() => ServiceManagerInstance?.Awake();

        private void OnEnable() => ServiceManagerInstance?.OnEnable();

        private void Start() => ServiceManagerInstance?.Start();

        private void Update() => ServiceManagerInstance?.Update();

        private void LateUpdate() => ServiceManagerInstance?.LateUpdate();

        private void FixedUpdate() => ServiceManagerInstance?.FixedUpdate();

        private void OnDisable() => ServiceManagerInstance?.OnDisable();

        internal void OnDestroy() => ServiceManagerInstance?.OnDestroy();

        private void OnApplicationFocus(bool focus) => ServiceManagerInstance?.OnApplicationFocus(focus);

        private void OnApplicationPause(bool pause) => ServiceManagerInstance?.OnApplicationPause(pause);

        #endregion MonoBehaviour Implementation

        private static List<Tuple<string, Version>> modules = null;

        /// <summary>
        /// The list of active xrtk modules/packages currently loaded into runtime.
        /// </summary>
        public static List<Tuple<string, Version>> Modules
        {
            get
            {
                return modules ?? (modules = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(assembly =>
                    {
                        var titleAttribute = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
                        return titleAttribute != null && titleAttribute.Title.Contains("xrtk");
                    })
                    .Select(assembly => new Tuple<string, Version>(assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title, assembly.GetName().Version))
                    .ToList());
            }
        }
    }
}