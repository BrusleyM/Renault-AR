using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.TestTools;
using System.Collections;
using System.Reflection;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.LowLevel;

public class InteractionsManagerTests
{
    private GameObject testObject;
    private InteractionsManager interactionsManager;
    private Touchscreen touchscreen;
    private GameObject carPrefab;
    private GameObject cameraObject;

    [SetUp]
    public void Setup()
    {
        EnhancedTouchSupport.Enable();

        // Create and set up a main camera
        cameraObject = new GameObject("Main Camera");
        cameraObject.AddComponent<Camera>();
        cameraObject.tag = "MainCamera"; // Ensure it's recognized as Camera.main

        // Create the car prefab (this could be your actual prefab from Resources or another method)
        carPrefab = new GameObject("CarPrefab");

        // Set up the GameManager with the selected car
        var gman = new GameObject("GameManager");
        gman.AddComponent<GameManager>();
        GameManager.Instance.SetSelectedCar(carPrefab,"test");

        // Create a test GameObject and add the necessary AR components
        testObject = new GameObject();
        testObject.AddComponent<ARRaycastManager>();
        testObject.AddComponent<ARPlaneManager>();
        interactionsManager = testObject.AddComponent<InteractionsManager>();

        // Mock AR session for testing
        var session = new GameObject("Session");
        session.AddComponent<ARSession>();

        // Use reflection to set the private _session field
        var interactionsField = typeof(InteractionsManager).GetField("_session", BindingFlags.NonPublic | BindingFlags.Instance);
        interactionsField.SetValue(interactionsManager, session.GetComponent<ARSession>());

        // Set up touchscreen input
        touchscreen = InputSystem.AddDevice<Touchscreen>();
    }

    [TearDown]
    public void Teardown()
    {
        // Clean up after each test
        Object.DestroyImmediate(cameraObject);
        Object.DestroyImmediate(testObject);
        InputSystem.RemoveDevice(touchscreen);
    }

    //[UnityTest]
    //public IEnumerator SimulateTouch_PlacesCar()
    //{
    //    // Simulate a touch event at the center of the screen
    //    Vector2 touchPosition = new Vector2(Screen.width / 2, Screen.height / 2);

    //    var plane = new GameObject("plane");
    //    plane.AddComponent<ARPlane>();
    //    // Simulate touch press
    //    var touchState = new TouchState
    //    {
    //        touchId = 1,
    //        position = plane.transform.position,
    //        phase = UnityEngine.InputSystem.TouchPhase.Began
    //    };

    //    InputSystem.QueueStateEvent(touchscreen, touchState);
    //    InputSystem.Update();

    //    // Wait a frame for the car to be placed
    //    yield return null;

    //    // Verify car has been instantiated by checking if the _car field is set
    //    var carField = typeof(InteractionsManager).GetField("_car", BindingFlags.NonPublic | BindingFlags.Instance);
    //    GameObject placedCar = (GameObject)carField.GetValue(interactionsManager);

    //    Assert.NotNull(placedCar, "Car should have been instantiated.");
    //    Assert.AreEqual(carPrefab, placedCar, "The instantiated car should be the selected car.");
    //}

    [UnityTest]
    public IEnumerator SimulatePinch_ScalesCar()
    {
        // Create a car object to set to the _car field (use reflection)
        var carField = typeof(InteractionsManager).GetField("_car", BindingFlags.NonPublic | BindingFlags.Instance);
        GameObject carObject = new GameObject("Car");
        carField.SetValue(interactionsManager, carObject);

        // Simulate touch input for pinch gesture
        Vector2 touch1Start = new Vector2(100, 100);
        Vector2 touch2Start = new Vector2(200, 100);
        Vector2 touch1End = new Vector2(80, 100);
        Vector2 touch2End = new Vector2(220, 100); // Fingers moving apart

        var touch1 = new TouchState { touchId = 1, position = touch1Start, phase = UnityEngine.InputSystem.TouchPhase.Began };
        var touch2 = new TouchState { touchId = 2, position = touch2Start, phase = UnityEngine.InputSystem.TouchPhase.Began };
        InputSystem.QueueStateEvent(touchscreen, touch1);
        InputSystem.QueueStateEvent(touchscreen, touch2);
        InputSystem.Update();
        yield return null;

        // Move fingers apart (pinch out)
        touch1.position = touch1End;
        touch2.position = touch2End;
        touch1.phase = UnityEngine.InputSystem.TouchPhase.Moved;
        touch2.phase = UnityEngine.InputSystem.TouchPhase.Moved;
        InputSystem.QueueStateEvent(touchscreen, touch1);
        InputSystem.QueueStateEvent(touchscreen, touch2);
        InputSystem.Update();
        yield return null;

        // Get the car object from the _car field and check if it was scaled
        GameObject updatedCar = (GameObject)carField.GetValue(interactionsManager);

        // Assert that scale increased
        Assert.Greater(updatedCar.transform.localScale.x, 1.0f, "Car should be scaled up.");
    }
}