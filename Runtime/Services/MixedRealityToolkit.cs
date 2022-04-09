// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RealityToolkit.ServiceFramework.Definitions;
using RealityToolkit.ServiceFramework.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions;
using XRTK.Definitions.Platforms;
using XRTK.Extensions;
using XRTK.Interfaces;
using XRTK.Utilities;
using XRTK.Utilities.Async;
using RealityToolkit.ServiceFramework.Services;
using UnityEditor.ShortcutManagement;

namespace XRTK.Services
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
        /// The public property of the Active Profile, ensuring events are raised on the change of the reference
        /// </summary>
        public ServiceManagerRootProfile ActiveProfile { get => ServiceManager.Instance.ActiveProfile; set => ServiceManager.Instance.ActiveProfile = value; }

        /// <summary>
        /// When a profile is replaced with a new one, force all services to reset and read the new values
        /// </summary>
        /// <param name="profile"></param>
        public void ResetProfile(ServiceManagerRootProfile profile)
        {
            ServiceManager.Instance.ResetProfile(profile);
        }

        private static bool isResetting = false;

        #endregion Mixed Reality Toolkit Profile properties

        #region Mixed Reality runtime service registry
        
        // ReSharper disable once InconsistentNaming
        private static readonly List<IMixedRealityPlatform> availablePlatforms = new List<IMixedRealityPlatform>();

        /// <summary>
        /// The list of active platforms detected by the <see cref="MixedRealityToolkit"/>.
        /// </summary>
        public static IReadOnlyList<IMixedRealityPlatform> AvailablePlatforms => availablePlatforms;

        // ReSharper disable once InconsistentNaming
        private static readonly List<IMixedRealityPlatform> activePlatforms = new List<IMixedRealityPlatform>();

        /// <summary>
        /// The list of active platforms detected by the <see cref="MixedRealityToolkit"/>.
        /// </summary>
        public static IReadOnlyList<IMixedRealityPlatform> ActivePlatforms => activePlatforms;

        #endregion Mixed Reality runtime service registry

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

        public static IReadOnlyDictionary<Type, IService> ActiveSystems => ServiceManager.ActiveServices;
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
                    ServiceManager.Instance.DisableAllServices();
                    ServiceManager.Instance.DestroyAllServices();
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
                    ServiceManager.Instance.InitializeServiceManager();
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

        /// <summary>
        /// Check which platforms are active and available.
        /// </summary>
        internal static void CheckPlatforms()
        {
            activePlatforms.Clear();
            availablePlatforms.Clear();

            var platformTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IMixedRealityPlatform).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .OrderBy(type => type.Name);

            var platformOverrides = new List<Type>();

            foreach (var platformType in platformTypes)
            {
                IMixedRealityPlatform platform = null;

                try
                {
                    platform = Activator.CreateInstance(platformType) as IMixedRealityPlatform;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                if (platform == null) { continue; }

                availablePlatforms.Add(platform);

                if (platform.IsAvailable
#if UNITY_EDITOR
                    || platform.IsBuildTargetAvailable &&
                    TypeExtensions.TryResolveType(UnityEditor.EditorPrefs.GetString("CurrentPlatformTarget", string.Empty), out var resolvedPlatform) &&
                    resolvedPlatform == platformType
#endif
                )
                {
                    foreach (var platformOverride in platform.PlatformOverrides)
                    {
                        platformOverrides.Add(platformOverride.GetType());
                    }
                }
            }

            foreach (var platform in availablePlatforms)
            {
                if (Application.isPlaying &&
                    platformOverrides.Contains(platform.GetType()))
                {
                    continue;
                }

                if (platform.IsAvailable
#if UNITY_EDITOR
                    || platform.IsBuildTargetAvailable
#endif
                )
                {
                    activePlatforms.Add(platform);
                }
            }
        }

        private static void EnsureMixedRealityRequirements()
        {
            CheckPlatforms();

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
        public static bool TryRegisterServiceConfigurations<T>(IMixedRealityServiceConfiguration<T>[] configurations) where T : IService
        {
            bool anyFailed = false;

            for (var i = 0; i < configurations?.Length; i++)
            {
                var configuration = configurations[i];

                if (TryCreateAndRegisterService(configuration, out var serviceInstance))
                {
                    if (configuration.ServiceConfiguration.Profile is IMixedRealityServiceProfile<IServiceDataProvider> profile &&
                        !TryRegisterDataProviderConfigurations(profile.RegisteredServiceConfigurations, serviceInstance))
                    {
                        anyFailed = true;
                    }
                }
                else
                {
                    Debug.LogError($"Failed to start {configuration.ServiceConfiguration.Name}!");
                    anyFailed = true;
                }
            }

            return !anyFailed;
        }

        /// <summary>
        /// Registers all the <see cref="IMixedRealityDataProvider"/>s defined in the provided configuration collection.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealityDataProvider"/> to be registered.</typeparam>
        /// <param name="configurations">The list of <see cref="IMixedRealityServiceConfiguration{T}"/>s.</param>
        /// <param name="serviceParent">The <see cref="IMixedRealityService"/> that the <see cref="IMixedRealityDataProvider"/> will be assigned to.</param>
        /// <returns>True, if all configurations successfully created and registered their data providers.</returns>
        public static bool TryRegisterDataProviderConfigurations<T>(IMixedRealityServiceConfiguration<T>[] configurations, IService serviceParent) where T : IServiceDataProvider
        {
            bool anyFailed = false;

            for (var i = 0; i < configurations?.Length; i++)
            {
                var configuration = configurations[i];

                if (!TryCreateAndRegisterDataProvider(configuration, serviceParent))
                {
                    Debug.LogError($"Failed to start {configuration.ServiceConfiguration.Name}!");
                    anyFailed = true;
                }
            }

            return !anyFailed;
        }

        /// <summary>
        /// Add a service instance to the Mixed Reality Toolkit active service registry.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealityService"/> to be registered.</typeparam>
        /// <param name="serviceInstance">Instance of the <see cref="IMixedRealityService"/> to register.</param>
        /// <returns>True, if the service was successfully registered.</returns>
        public static bool TryRegisterService<T>(IService serviceInstance) where T : IService
        {
            return TryRegisterServiceInternal(typeof(T), serviceInstance);
        }

        /// <summary>
        /// Creates a new instance of a service and registers it to the Mixed Reality Toolkit service registry for the specified platform.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealityService"/> to be registered.</typeparam>
        /// <param name="configuration">The <see cref="IMixedRealityServiceConfiguration{T}"/> to use to create and register the service.</param>
        /// <param name="service">If successful, then the new <see cref="IMixedRealityService"/> instance will be passed back out.</param>
        /// <returns>True, if the service was successfully created and registered.</returns>
        public static bool TryCreateAndRegisterService<T>(IMixedRealityServiceConfiguration<T> configuration, out T service) where T : IService
        {
            return TryCreateAndRegisterServiceInternal(
                configuration.ServiceConfiguration.InstancedType,
                configuration.RuntimePlatforms,
                out service,
                configuration.ServiceConfiguration.Name,
                configuration.ServiceConfiguration.Priority,
                configuration.ServiceConfiguration.Profile);
        }

        /// <summary>
        /// Creates a new instance of a data provider and registers it to the Mixed Reality Toolkit service registry for the specified platform.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealityService"/> to be registered.</typeparam>
        /// <param name="configuration">The <see cref="IMixedRealityServiceConfiguration{T}"/> to use to create and register the data provider.</param>
        /// <param name="serviceParent">The <see cref="IMixedRealityService"/> that the <see cref="IMixedRealityDataProvider"/> will be assigned to.</param>
        /// <returns>True, if the service was successfully created and registered.</returns>
        public static bool TryCreateAndRegisterDataProvider<T>(IMixedRealityServiceConfiguration<T> configuration, IService serviceParent) where T : IServiceDataProvider
        {
            return TryCreateAndRegisterServiceInternal<T>(
                configuration.ServiceConfiguration.InstancedType,
                configuration.RuntimePlatforms,
                out _,
                configuration.ServiceConfiguration.Name,
                configuration.ServiceConfiguration.Priority,
                configuration.ServiceConfiguration.Profile,
                serviceParent);
        }

        /// <summary>
        /// Creates a new instance of a service and registers it to the Mixed Reality Toolkit service registry for the specified platform.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealityService"/> to be registered.</typeparam>
        /// <param name="concreteType">The concrete class type to instantiate.</param>
        /// <param name="service">If successful, then the new <see cref="IMixedRealityService"/> instance will be passed back out.</param>
        /// <param name="args">Optional arguments used when instantiating the concrete type.</param>
        /// <returns>True, if the service was successfully created and registered.</returns>
        public static bool TryCreateAndRegisterService<T>(Type concreteType, out T service, params object[] args) where T : IService
        {
            return TryCreateAndRegisterServiceInternal(concreteType, AllPlatforms, out service, args);
        }

        private static readonly IMixedRealityPlatform[] AllPlatforms = { new AllPlatforms() };

        /// <summary>
        /// Creates a new instance of a service and registers it to the Mixed Reality Toolkit service registry for the specified platform.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealityService"/> to be registered.</typeparam>
        /// <param name="concreteType">The concrete class type to instantiate.</param>
        /// <param name="runtimePlatforms">The runtime platform to check against when registering.</param>
        /// <param name="service">If successful, then the new <see cref="IMixedRealityService"/> instance will be passed back out.</param>
        /// <param name="args">Optional arguments used when instantiating the concrete type.</param>
        /// <returns>True, if the service was successfully created and registered.</returns>
        public static bool TryCreateAndRegisterService<T>(Type concreteType, IReadOnlyList<IMixedRealityPlatform> runtimePlatforms, out T service, params object[] args) where T : IService
        {
            return TryCreateAndRegisterServiceInternal(concreteType, runtimePlatforms, out service, args);
        }

        /// <summary>
        /// Creates a new instance of a service and registers it to the Mixed Reality Toolkit service registry for the specified platform.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealityService"/> to be registered.</typeparam>
        /// <param name="concreteType">The concrete class type to instantiate.</param>
        /// <param name="runtimePlatforms">The runtime platform to check against when registering.</param>
        /// <param name="service">If successful, then the new <see cref="IMixedRealityService"/> instance will be passed back out.</param>
        /// <param name="args">Optional arguments used when instantiating the concrete type.</param>
        /// <returns>True, if the service was successfully created and registered.</returns>
        private static bool TryCreateAndRegisterServiceInternal<T>(Type concreteType, IReadOnlyList<IMixedRealityPlatform> runtimePlatforms, out T service, params object[] args) where T : IService
        {
            service = default;

            if (IsApplicationQuitting)
            {
                return false;
            }

            if (!IsSystem(typeof(T)))
            {
                var platforms = new List<IMixedRealityPlatform>();

                Debug.Assert(ActivePlatforms.Count > 0);

                for (var i = 0; i < ActivePlatforms.Count; i++)
                {
                    var activePlatform = ActivePlatforms[i].GetType();

                    for (var j = 0; j < runtimePlatforms?.Count; j++)
                    {
                        var runtimePlatform = runtimePlatforms[j].GetType();

                        if (activePlatform == runtimePlatform)
                        {
                            platforms.Add(runtimePlatforms[j]);
                            break;
                        }
                    }
                }

                if (platforms.Count == 0
#if UNITY_EDITOR
                    || !CurrentBuildTargetPlatform.IsBuildTargetActive(platforms)
#endif
                    )
                {
                    if (runtimePlatforms == null ||
                        runtimePlatforms.Count == 0)
                    {
                        Debug.LogWarning($"No runtime platforms defined for the {concreteType?.Name} service.");
                    }

                    // We return true so we don't raise en error.
                    // Even though we did not register the service,
                    // it's expected that this is the intended behavior
                    // when there isn't a valid platform to run the service on.
                    return true;
                }

                if (concreteType == null)
                {
                    Debug.LogError($"Unable to register a service with a null concrete {typeof(T).Name} type.");
                    return false;
                }
            }
            else
            {
                if (concreteType == null)
                {
                    Debug.LogError($"Unable to register a service with a null concrete {typeof(T).Name} type.");
                    return false;
                }
            }

            if (!typeof(IService).IsAssignableFrom(concreteType))
            {
                Debug.LogError($"Unable to register the {concreteType.Name} service. It does not implement {typeof(IService)}.");
                return false;
            }

            IService serviceInstance;

            try
            {
                if (IsSystem(typeof(T)))
                {
                    var profile = args[2];
                    serviceInstance = Activator.CreateInstance(concreteType, profile) as IService;
                }
                else
                {
                    serviceInstance = Activator.CreateInstance(concreteType, args) as IService;
                }
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                Debug.LogError($"Failed to register the {concreteType.Name} service: {e.InnerException?.GetType()} - {e.InnerException?.Message}\n{e.InnerException?.StackTrace}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to register the {concreteType.Name} service: {e.GetType()} - {e.Message}\n{e.StackTrace}");
                return false;
            }

            service = (T)serviceInstance;

            if (service == null ||
                serviceInstance == null)
            {
                Debug.LogError($"Failed to create a valid instance of {concreteType.Name}!");
                return false;
            }

            return TryRegisterServiceInternal(typeof(T), serviceInstance);
        }

        /// <summary>
        /// Internal service registration.
        /// </summary>
        /// <param name="interfaceType">The interface type for the <see cref="IMixedRealityService"/> to be registered.</param>
        /// <param name="serviceInstance">Instance of the <see cref="IMixedRealityService"/> to register.</param>
        /// <returns>True if registration is successful, false otherwise.</returns>
        private static bool TryRegisterServiceInternal(Type interfaceType, IService serviceInstance) => ServiceManager.TryRegisterService(interfaceType, serviceInstance);

        #endregion Registration

        #region Unregistration

        /// <summary>
        /// Remove all services from the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        public static bool TryUnregisterServicesOfType<T>() where T : IService => ServiceManager.TryUnregisterServicesOfType<T>();

        /// <summary>
        /// Removes a specific service with the provided name.
        /// </summary>
        /// <param name="serviceInstance">The instance of the <see cref="IMixedRealityService"/> to remove.</param>
        public static bool TryUnregisterService<T>(T serviceInstance) where T : IService => ServiceManager.TryUnregisterService<T>(serviceInstance);


        /// <summary>
        /// Removes a specific service with the provided name.
        /// </summary>
        /// <param name="serviceName">The name of the service to be removed. (Only for runtime services) </param>
        public static bool TryUnregisterService<T>(string serviceName) where T : IService => ServiceManager.TryUnregisterService<T>(serviceName);

        #endregion Unregistration

        #region Multiple Service Management

        /// <summary>
        /// Enable all services in the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be enabled.  E.G. InputSystem, BoundarySystem</typeparam>
        public static void EnableAllServicesOfType<T>() where T : IService
        {
            EnableAllServicesByTypeAndName(typeof(T), string.Empty);
        }

        /// <summary>
        /// Enables a single service by name.
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be enabled.  E.G. InputSystem, BoundarySystem</typeparam>
        /// <param name="serviceName"></param>
        public static void EnableService<T>(string serviceName) where T : IService
        {
            EnableAllServicesByTypeAndName(typeof(T), serviceName);
        }

        /// <summary>
        /// Enable all services in the Mixed Reality Toolkit active service registry for a given type and name
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be enabled.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">Name of the specific service</param>
        private static void EnableAllServicesByTypeAndName(Type interfaceType, string serviceName)
        {
            if (interfaceType == null)
            {
                Debug.LogError("Unable to enable null service type.");
                return;
            }

            var services = new List<IService>();
            GetAllServicesByNameInternal(interfaceType, serviceName, ref services);

            for (int i = 0; i < services?.Count; i++)
            {
                try
                {
                    services[i].Enable();
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}\n{e.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Disable all services in the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be enabled.  E.G. InputSystem, BoundarySystem</typeparam>
        public static void DisableAllServiceOfType<T>() where T : IService
        {
            DisableAllServicesByTypeAndName(typeof(T), string.Empty);
        }

        /// <summary>
        /// Disable a single service by name.
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be enabled.  E.G. InputSystem, BoundarySystem</typeparam>
        /// <param name="serviceName">Name of the specific service</param>
        public static void DisableService<T>(string serviceName) where T : IService
        {
            DisableAllServicesByTypeAndName(typeof(T), serviceName);
        }

        /// <summary>
        /// Disable all services in the Mixed Reality Toolkit active service registry for a given type and name
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be disabled.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">Name of the specific service</param>
        private static void DisableAllServicesByTypeAndName(Type interfaceType, string serviceName)
        {
            if (interfaceType == null)
            {
                Debug.LogError("Unable to disable null service type.");
                return;
            }

            var services = new List<IService>();
            GetAllServicesByNameInternal(interfaceType, serviceName, ref services);

            for (int i = 0; i < services?.Count; i++)
            {
                try
                {
                    services[i].Disable();
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}\n{e.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Retrieve all services from the Mixed Reality Toolkit active service registry for a given type and an optional name
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem.</typeparam>
        /// <returns>An array of services that meet the search criteria</returns>
        public static List<T> GetActiveServices<T>() where T : IService
        {
            return GetActiveServices<T>(typeof(T));
        }

        /// <summary>
        /// Retrieve all services from the Mixed Reality Toolkit active service registry for a given type and an optional name
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <returns>An array of services that meet the search criteria</returns>
        private static List<T> GetActiveServices<T>(Type interfaceType) where T : IService
        {
            return GetActiveServices<T>(interfaceType, string.Empty);
        }

        /// <summary>
        /// Retrieve all services from the Mixed Reality Toolkit active service registry for a given type and name
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">Name of the specific service</param>
        /// <returns>An array of services that meet the search criteria</returns>
        private static List<T> GetActiveServices<T>(Type interfaceType, string serviceName) where T : IService
        {
            return ServiceManager.GetServices<T>();
        }

        #endregion Multiple Service Management

        #region Service Utilities

        /// <summary>
        /// Query the <see cref="RegisteredMixedRealityServices"/> for the existence of a <see cref="IMixedRealityService"/>.
        /// </summary>
        /// <typeparam name="T">The interface type for the service to be retrieved.</typeparam>
        /// <returns>Returns true, if there is a <see cref="IMixedRealityService"/> registered, otherwise false.</returns>
        public static bool IsServiceRegistered<T>() where T : IService
            => GetService(typeof(T)) != null;

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="RegisteredMixedRealityServices"/>.
        /// </summary>
        /// <param name="showLogs">Should the logs show when services cannot be found?</param>
        /// <typeparam name="T">The interface type for the service to be retrieved.</typeparam>
        /// <returns>The instance of the <see cref="IMixedRealityService"/> that is registered.</returns>
        public static T GetService<T>(bool showLogs = true) where T : IService
            => (T)GetService(typeof(T), showLogs);

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="RegisteredMixedRealityServices"/>.
        /// </summary>
        /// <param name="timeout">Optional, time out in seconds to wait before giving up search.</param>
        /// <typeparam name="T">The interface type for the service to be retrieved.</typeparam>
        /// <returns>The instance of the <see cref="IMixedRealityService"/> that is registered.</returns>
        public static async Task<T> GetServiceAsync<T>(int timeout = 10) where T : IService
            => await GetService<T>().WaitUntil(system => system != null, timeout);

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="RegisteredMixedRealityServices"/>.
        /// </summary>
        /// <typeparam name="T">The interface type for the service to be retrieved.</typeparam>
        /// <param name="service">The instance of the service class that is registered.</param>
        /// <param name="showLogs">Should the logs show when services cannot be found?</param>
        /// <returns>Returns true if the <see cref="IMixedRealityService"/> was found, otherwise false.</returns>
        public static bool TryGetService<T>(out T service, bool showLogs = true) where T : IService
        {
            service = GetService<T>(showLogs);
            return service != null;
        }

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="RegisteredMixedRealityServices"/> by name.
        /// </summary>
        /// <param name="serviceName">Name of the specific service to search for.</param>
        /// <param name="showLogs">Should the logs show when services cannot be found?</param>
        /// <returns>The instance of the <see cref="IMixedRealityService"/> that is registered.</returns>
        public static T GetService<T>(string serviceName, bool showLogs = true) where T : IService
            => (T)GetService(typeof(T), serviceName, showLogs);

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="RegisteredMixedRealityServices"/> by name.
        /// </summary>
        /// <typeparam name="T">The interface type for the service to be retrieved.</typeparam>
        /// <param name="serviceName">Name of the specific service to search for.</param>
        /// <param name="service">The instance of the service class that is registered.</param>
        /// <param name="showLogs">Should the logs show when services cannot be found?</param>
        /// <returns>Returns true if the <see cref="IMixedRealityService"/> was found, otherwise false.</returns>
        public static bool TryGetService<T>(string serviceName, out T service, bool showLogs = true) where T : IService
        {
            service = (T)GetService(typeof(T), serviceName, showLogs);
            return service != null;
        }

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="RegisteredMixedRealityServices"/> by type.
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be retrieved.</param>
        /// <param name="showLogs">Should the logs show when services cannot be found?</param>
        /// <returns>The instance of the <see cref="IMixedRealityService"/> that is registered.</returns>
        private static IService GetService(Type interfaceType, bool showLogs = true)
            => GetService(interfaceType, string.Empty, showLogs);

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="RegisteredMixedRealityServices"/>.
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be retrieved.</param>
        /// <param name="serviceName">Name of the specific service.</param>
        /// <param name="showLogs">Should the logs show when services cannot be found?</param>
        /// <returns>The instance of the <see cref="IMixedRealityService"/> that is registered.</returns>
        private static IService GetService(Type interfaceType, string serviceName, bool showLogs = true)
        {
            if (!GetServiceByNameInternal(interfaceType, serviceName, out var serviceInstance) && showLogs)
            {
                Debug.LogError($"Unable to find {(string.IsNullOrWhiteSpace(serviceName) ? interfaceType.Name : serviceName)} service.");
            }

            return serviceInstance;
        }

        /// <summary>
        /// Retrieve the first <see cref="IMixedRealityService"/> from the <see cref="RegisteredMixedRealityServices"/> that meets the selected type and name.
        /// </summary>
        /// <param name="interfaceType">Interface type of the service being requested.</param>
        /// <param name="serviceName">Name of the specific service.</param>
        /// <param name="serviceInstance">return parameter of the function.</param>
        private static bool GetServiceByNameInternal(Type interfaceType, string serviceName, out IService serviceInstance)
        {
            serviceInstance = null;

            if (!CanGetService(interfaceType, serviceName)) { return false; }

            serviceInstance = ServiceManager.GetService(interfaceType, serviceName);
            
            if (serviceInstance != null)
            {
                if (CheckServiceMatch(interfaceType, serviceName, interfaceType, serviceInstance))
                {
                    return true;
                }
                
                serviceInstance = null;
            }
            
            return false;
        }

        /// <summary>
        /// Gets all <see cref="IMixedRealityService"/>s by type.
        /// </summary>
        /// <param name="interfaceType">The interface type to search for.</param>
        /// <param name="services">Memory reference value of the service list to update.</param>
        private static void GetAllServicesInternal<T>(Type interfaceType, ref List<T> services) where T : IService
        {
            GetAllServicesByNameInternal(interfaceType, string.Empty, ref services);
        }

        /// <summary>
        /// Gets all <see cref="IMixedRealityService"/>s by type and name.
        /// </summary>
        /// <param name="interfaceType">The interface type to search for.</param>
        /// <param name="serviceName">The name of the service to search for. If the string is empty than any matching <see cref="interfaceType"/> will be added to the <see cref="services"/> list.</param>
        /// <param name="services">Memory reference value of the service list to update.</param>
        private static void GetAllServicesByNameInternal<T>(Type interfaceType, string serviceName, ref List<T> services) where T : IService
        {
            if (!CanGetService(interfaceType, serviceName)) { return; }

            if (GetServiceByNameInternal(interfaceType, serviceName, out var serviceInstance) &&
                CheckServiceMatch(interfaceType, serviceName, interfaceType, serviceInstance))
            {
                services.Add((T)serviceInstance);
            }
        }

        /// <summary>
        /// Check if the interface type and name matches the registered interface type and service instance found.
        /// </summary>
        /// <param name="interfaceType">The interface type of the service to check.</param>
        /// <param name="serviceName">The name of the service to check.</param>
        /// <param name="registeredInterfaceType">The registered interface type.</param>
        /// <param name="serviceInstance">The instance of the registered service.</param>
        /// <returns>True, if the registered service contains the interface type and name.</returns>
        private static bool CheckServiceMatch(Type interfaceType, string serviceName, Type registeredInterfaceType, IService serviceInstance)
        {
            bool isNameValid = string.IsNullOrEmpty(serviceName) || serviceInstance.Name == serviceName;
            bool isInstanceValid = interfaceType == registeredInterfaceType || interfaceType.IsInstanceOfType(serviceInstance);
            return isNameValid && isInstanceValid;
        }

        private static bool CanGetService(Type interfaceType, string serviceName)
        {
            if (IsApplicationQuitting)
            {
                return false;
            }

            if (interfaceType == null)
            {
                Debug.LogError($"{serviceName} interface type is null.");
                return false;
            }

            if (!typeof(IService).IsAssignableFrom(interfaceType))
            {
                Debug.LogError($"{interfaceType.Name} does not implement {nameof(IService)}.");
                return false;
            }

            return true;
        }

        #endregion Service Utilities

        #region System Utilities

        /// <summary>
        /// Query <see cref="RegisteredMixedRealityServices"/> for the existence of a <see cref="IMixedRealitySystem"/>.
        /// </summary>
        /// <typeparam name="T">The interface type for the <see cref="IMixedRealitySystem"/> to be retrieved.</typeparam>
        /// <returns>Returns true, if there is a <see cref="IMixedRealitySystem"/> registered, otherwise false.</returns>
        public static bool IsSystemRegistered<T>() where T : IService
        {
            return ServiceManager.IsServiceRegistered<T>();
        }

        /// <summary>
        /// Is the <see cref="IMixedRealitySystem"/> enabled in the <see cref="ActiveProfile"/>?
        /// </summary>
        /// <typeparam name="T"><see cref="IMixedRealitySystem"/> to check.</typeparam>
        /// <param name="rootProfile">Optional root profile reference.</param>
        /// <returns>True, if the system is enabled in the <see cref="ActiveProfile"/>, otherwise false.</returns>
        public static bool IsSystemEnabled<T>(ServiceManagerRootProfile rootProfile = null) where T : IService
        {
            if (rootProfile.IsNull())
            {
                rootProfile = instance.ActiveProfile;
            }

            if (!rootProfile.IsNull())
            {
                // ReSharper disable once PossibleNullReferenceException
                foreach (var configuration in rootProfile.ServiceConfigurations)
                {
                    if (typeof(T).IsAssignableFrom(configuration.InstancedType.Type.FindMixedRealityServiceInterfaceType(typeof(T))) &&
                        configuration.Enabled)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Try to get the <see cref="TProfile"/> of the <see cref="TSystem"/>
        /// </summary>
        /// <typeparam name="TSystem">The <see cref="IMixedRealitySystem"/> type to get the <see cref="TProfile"/> for.</typeparam>
        /// <typeparam name="TProfile">The <see cref="BaseMixedRealityProfile"/> type to get for the <see cref="TSystem"/>.</typeparam>
        /// <param name="profile">The profile instance.</param>
        /// <param name="rootProfile">Optional root profile reference.</param>
        /// <returns>True if a <see cref="TSystem"/> type is matched and a valid <see cref="TProfile"/> is found, otherwise false.</returns>
        public static bool TryGetSystemProfile<TSystem, TProfile>(out TProfile profile, ServiceManagerRootProfile rootProfile = null)
            where TSystem : IService
            where TProfile : BaseProfile
        {
            if (rootProfile.IsNull())
            {
                rootProfile = instance.ActiveProfile;
            }

            if (!rootProfile.IsNull())
            {
                // ReSharper disable once PossibleNullReferenceException
                foreach (var configuration in rootProfile.ServiceConfigurations)
                {
                    if (typeof(TSystem).IsAssignableFrom(configuration.InstancedType.Type.FindMixedRealityServiceInterfaceType(typeof(TSystem))))
                    {
                        profile = (TProfile)configuration.Profile;
                        return profile != null;
                    }
                }
            }

            profile = null;
            return false;
        }

        private static bool IsSystem(Type concreteType)
        {
            if (concreteType == null)
            {
                Debug.LogError($"{nameof(concreteType)} is null.");
                return false;
            }

            return typeof(IMixedRealitySystem).IsAssignableFrom(concreteType);
        }

        private static readonly HashSet<Type> SearchedSystemTypes = new HashSet<Type>();
        private static readonly Dictionary<Type, IService> SystemCache = new Dictionary<Type, IService>();

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="ActiveSystems"/>.
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.</typeparam>
        /// <returns>The instance of the <see cref="IMixedRealitySystem"/> that is registered.</returns>
        public static T GetSystem<T>() where T : IService
        {
            if (!IsInitialized ||
                IsApplicationQuitting ||
                instance.ActiveProfile.IsNull())
            {
                return default;
            }

            T system = default;

            if (!SystemCache.TryGetValue(typeof(T), out var cachedSystem))
            {
                if (IsSystemRegistered<T>())
                {
                    var hasSearched = SearchedSystemTypes.Contains(typeof(T));

                    if (TryGetService(out system, !hasSearched))
                    {
                        SystemCache.Add(typeof(T), system);
                    }
                    else
                    {
                        if (!hasSearched)
                        {
                            SearchedSystemTypes.Add(typeof(T));
                        }
                    }
                }
            }
            else
            {
                system = (T)cachedSystem;
            }

            return system;
        }

        /// <summary>
        /// Retrieve a <see cref="IMixedRealityService"/> from the <see cref="ActiveSystems"/>.
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.</typeparam>
        /// <param name="timeout">Optional, time out in seconds to wait before giving up search.</param>
        /// <returns>The instance of the <see cref="IMixedRealitySystem"/> that is registered.</returns>
        public static async Task<T> GetSystemAsync<T>(int timeout = 10) where T : IService
            => await GetSystem<T>().WaitUntil(system => system != null, timeout);

        /// <summary>
        /// Retrieve a <see cref="IMixedRealitySystem"/> from the <see cref="ActiveSystems"/>.
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.</typeparam>
        /// <param name="system">The instance of the system class that is registered.</param>
        /// <returns>Returns true if the <see cref="IMixedRealitySystem"/> was found, otherwise false.</returns>
        public static bool TryGetSystem<T>(out T system) where T : IService
        {
            system = GetSystem<T>();
            return system != null;
        }

        private static void ClearSystemCache()
        {
            SystemCache.Clear();
            SearchedSystemTypes.Clear();
        }

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
