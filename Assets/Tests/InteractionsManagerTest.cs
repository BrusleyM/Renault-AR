//using NUnit.Framework;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.EnhancedTouch;
//using UnityEngine.TestTools;
//using System.Collections;
//using System.Reflection;
//using UnityEngine.XR.ARFoundation;
//using UnityEngine.XR.ARSubsystems;
//using System.Collections.Generic;
//using Finger = UnityEngine.InputSystem.EnhancedTouch.Finger;

//public class InteractionsManagerTests : InputTestFixture
//{
//    private GameObject testObject;
//    private InteractionsManager interactionsManager;
//    private GameObject carPrefab;
//    private GameObject cameraObject;

//    private ARRaycastManager raycastManager;
//    private ARPlaneManager planeManager;

//    private GameObject planeObj;
//    private ARPlane plane;

//    [SetUp]
//    public override void Setup()
//    {
//        base.Setup();
//        EnhancedTouchSupport.Enable();

//        // Mock Camera
//        cameraObject = new GameObject("MainCamera");
//        cameraObject.tag = "MainCamera";
//        cameraObject.AddComponent<Camera>();

//        // GameManager setup
//        GameObject gman = new GameObject("GameManager");
//        gman.AddComponent<GameManager>();
//        carPrefab = new GameObject("CarPrefab");
//        GameManager.Instance.SetSelectedCar(carPrefab, "test");

//        // Test target
//        testObject = new GameObject("InteractionsManager");
//        raycastManager = testObject.AddComponent<ARRaycastManager>();
//        planeManager = testObject.AddComponent<ARPlaneManager>();
//        interactionsManager = testObject.AddComponent<InteractionsManager>();

//        // Assign _session
//        var sessionObj = new GameObject("ARSession");
//        sessionObj.AddComponent<ARSession>();
//        typeof(InteractionsManager).GetField("_session", BindingFlags.NonPublic | BindingFlags.Instance)
//            .SetValue(interactionsManager, sessionObj.GetComponent<ARSession>());

//        // Assign _featuresUI
//        var ui = new GameObject("FeaturesUI");
//        typeof(InteractionsManager).GetField("_featuresUI", BindingFlags.NonPublic | BindingFlags.Instance)
//            .SetValue(interactionsManager, ui);

//        // Assign ARAnchorManager
//        var anchorManager = testObject.AddComponent<ARAnchorManager>();
//        typeof(InteractionsManager).GetField("_anchorManager", BindingFlags.NonPublic | BindingFlags.Instance)
//            .SetValue(interactionsManager, anchorManager);

//        // Add dummy plane
//        planeObj = new GameObject("MockPlane");
//        plane = planeObj.AddComponent<ARPlane>();
//        plane.alignment = PlaneAlignment.HorizontalUp;

//        // Add testObject to scene
//        interactionsManager.enabled = true;
//    }

//    [TearDown]
//    public override void TearDown()
//    {
//        base.TearDown();
//        EnhancedTouchSupport.Disable();
//        Object.DestroyImmediate(testObject);
//        Object.DestroyImmediate(cameraObject);
//        Object.DestroyImmediate(planeObj);
//    }

//    [UnityTest]
//    public IEnumerator PlaceCar_ShouldInstantiateCar_WhenTouchHitsPlane()
//    {
//        // Simulate raycast hit manually
//        var hitsList = new List<ARRaycastHit> { new ARRaycastHit(default, default, 0, TrackableId.invalidId) };
//        typeof(InteractionsManager).GetField("_hits", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(interactionsManager, hitsList);

//        var rayMethod = typeof(ARRaycastManager).GetMethod("Raycast", new[] { typeof(Vector2), typeof(List<ARRaycastHit>), typeof(TrackableType) });
//        var mockRaycast = new System.Func<Vector2, List<ARRaycastHit>, TrackableType, bool>((vec, list, type) =>
//        {
//            list.Add(new ARRaycastHit(default, default, 0f, plane.trackableId));
//            return true;
//        });

//        // Replace method (ugly but works)
//        // Use interface injection or a wrapper in production
//        typeof(ARRaycastManager)
//            .GetField("m_ReferencePointManager", BindingFlags.NonPublic | BindingFlags.Instance)
//            ?.SetValue(raycastManager, null); // avoid null references

//        // Simulate finger down (index 0)
//        var finger = new Finger();
//        interactionsManager.SendMessage("PlaceCar", finger); // Direct call to bypass input

//        yield return null;

//        var carField = typeof(InteractionsManager).GetField("_car", BindingFlags.NonPublic | BindingFlags.Instance);
//        var placedCar = (GameObject)carField.GetValue(interactionsManager);

//        Assert.NotNull(placedCar);
//        Assert.AreEqual("CarPrefab", placedCar.name.Replace("(Clone)", ""));
//    }

//    [UnityTest]
//    public IEnumerator PinchZoom_ScalesCarUp()
//    {
//        // Add a car
//        var car = new GameObject("Car");
//        car.transform.localScale = Vector3.one;
//        typeof(InteractionsManager).GetField("_car", BindingFlags.NonPublic | BindingFlags.Instance)
//            .SetValue(interactionsManager, car);

//        // Trigger OnEnable to register EnhancedTouch listeners
//        interactionsManager.SendMessage("OnEnable");

//        // Simulate two fingers touching and moving apart (zoom in)
//        BeginTouch(1, new Vector2(100, 100));
//        BeginTouch(2, new Vector2(200, 100));
//        yield return new WaitForEndOfFrame();

//        MoveTouch(1, new Vector2(80, 100));
//        MoveTouch(2, new Vector2(220, 100));
//        yield return new WaitForEndOfFrame();

//        Assert.Greater(car.transform.localScale.x, 1f);
//    }

//    // Helpers for EnhancedTouch simulation
//    private void BeginTouch(int id, Vector2 position)
//    {
//        Touchscreen.current.QueueStateEvent(new TouchState
//        {
//            touchId = id,
//            position = position,
//            phase = UnityEngine.InputSystem.TouchPhase.Began
//        });
//        InputSystem.Update();
//    }

//    private void MoveTouch(int id, Vector2 position)
//    {
//        Touchscreen.current.QueueStateEvent(new TouchState
//        {
//            touchId = id,
//            position = position,
//            phase = UnityEngine.InputSystem.TouchPhase.Moved
//        });
//        InputSystem.Update();
//    }
//}