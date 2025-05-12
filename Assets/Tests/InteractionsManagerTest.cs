using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Common.Objects;
using System;
using ARscene;
using Managers;
using Object = UnityEngine.Object;
using Services;
using Helpers;

namespace Tests
{
    [TestFixture]
    public class InteractionsManagerTests
    {
        private InteractionsManager _manager;
        private GameObject _managerObj;
        private ARCarPlacer _mockCarPlacer;
        private ARInputHandler _mockInputHandler;
        private ARSession _mockSession;
        private GameObject _featuresUI;
        private GameManager _gameManager;

        [SetUp]
        public void Setup()
        {
            // Create manager with required components
            _managerObj = new GameObject();
            _manager = _managerObj.AddComponent<InteractionsManager>();
            var planeManager= _managerObj.AddComponent<ARPlaneManager>();
            var raycastManager=_managerObj.AddComponent<ARRaycastManager>();
            _manager._anchorManager = _managerObj.AddComponent<ARAnchorManager>();
            var pinchHandler = new PinchZoomHandler(Camera.main, new RuntimeTouchProvider());
            // Create mock dependencies
            _mockCarPlacer = Substitute.For<ARCarPlacer>(planeManager,raycastManager, _manager._anchorManager);
            _mockInputHandler = Substitute.For<ARInputHandler>(pinchHandler);

            // Setup serialized fields through internal access
            _mockSession = new GameObject().AddComponent<ARSession>();
            _featuresUI = new GameObject();
            _manager._session = _mockSession;
            _manager._featuresUI = _featuresUI;
            _manager._anchorManager = _managerObj.AddComponent<ARAnchorManager>();

            // Inject mocks
            typeof(InteractionsManager)
                .GetField("_carPlacer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_manager, _mockCarPlacer);

            typeof(InteractionsManager)
                .GetField("_inputHandler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_manager, _mockInputHandler);

            // Setup GameManager
            _gameManager = new GameObject().AddComponent<GameManager>();
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_managerObj);
            Object.DestroyImmediate(_mockSession.gameObject);
            Object.DestroyImmediate(_featuresUI);
            Object.DestroyImmediate(_gameManager.gameObject);
        }

        [Test]
        public void OnEnable_SubscribesToAllInputEvents()
        {
            // Act
            _manager.OnEnable();

            // Assert
            _mockInputHandler.Received().OnTouchDown += Arg.Any<Action<CommonTouch>>();
            _mockInputHandler.Received().OnTouchMove += Arg.Any<Action<CommonTouch>>();
            _mockInputHandler.Received().OnTouchUp += Arg.Any<Action>();
        }

        [Test]
        public void OnDisable_UnsubscribesFromAllInputEvents()
        {
            // Act
            _manager.OnDisable();

            // Assert
            _mockInputHandler.Received().OnTouchDown -= Arg.Any<Action<CommonTouch>>();
            _mockInputHandler.Received().OnTouchMove -= Arg.Any<Action<CommonTouch>>();
            _mockInputHandler.Received().OnTouchUp -= Arg.Any<Action>();
        }

        [Test]
        public void HandleTouchDown_WhenNoCarPlaced_AttemptsPlacement()
        {
            // Arrange
            _mockCarPlacer.HasPlacedCar.Returns(false);
            var testCar = new GameObject();
            GameManager.Instance.SetSelectedCar(testCar, "TestCar");
            var testTouch = new CommonTouch(new Vector2(100, 100));

            // Act
            _manager.HandleTouchDown(testTouch);

            // Assert
            _mockCarPlacer.Received().TryPlaceCar(
                Arg.Is(testTouch),
                Arg.Is(testCar),
                Arg.Any<Action<GameObject>>());
        }

        [Test]
        public void HandleTouchDown_WhenCarAlreadyPlaced_DoesNothing()
        {
            // Arrange
            _mockCarPlacer.HasPlacedCar.Returns(true);

            // Act
            _manager.HandleTouchDown(new CommonTouch(Vector2.zero));

            // Assert
            _mockCarPlacer.DidNotReceive().TryPlaceCar(
                Arg.Any<CommonTouch>(),
                Arg.Any<GameObject>(),
                Arg.Any<Action<GameObject>>());
        }

        [Test]
        public void HandleTouchMove_WhenCarPlaced_TriggersPinchZoom()
        {
            // Arrange
            _mockCarPlacer.HasPlacedCar.Returns(true);
            var testCar = new GameObject();
            GameManager.Instance.SetInstantiatedCar(testCar);

            // Act
            _manager.HandleTouchMove(new CommonTouch(Vector2.zero));

            // Assert
            _mockInputHandler.Received().HandlePinchZoom(testCar);
        }

        [Test]
        public void HandleTouchMove_WhenNoCarPlaced_DoesNothing()
        {
            // Arrange
            _mockCarPlacer.HasPlacedCar.Returns(false);

            // Act
            _manager.HandleTouchMove(new CommonTouch(Vector2.zero));

            // Assert
            _mockInputHandler.DidNotReceive().HandlePinchZoom(Arg.Any<GameObject>());
        }

        [Test]
        public void HandleTouchUp_Always_ResetsPinchZoom()
        {
            // Act
            _manager.HandleTouchUp();

            // Assert
            _mockInputHandler.Received().ResetPinchZoom();
        }

        [Test]
        public void Start_ResetsSessionAndHidesUI()
        {
            // Arrange
            _featuresUI.SetActive(true);

            // Act
            _manager.Start();

            // Assert
            Assert.IsFalse(_featuresUI.activeSelf);
        }

        [Test]
        public void PlacementCallback_ActivatesFeaturesUI()
        {
            // Arrange
            _featuresUI.SetActive(false);
            Action<GameObject> callback = null;
            _mockCarPlacer.When(x => x.TryPlaceCar(
                Arg.Any<CommonTouch>(),
                Arg.Any<GameObject>(),
                Arg.Any<Action<GameObject>>()))
                .Do(x => callback = x.Arg<Action<GameObject>>());

            // Trigger placement
            _manager.HandleTouchDown(new CommonTouch(Vector2.zero));

            // Act - Invoke callback
            callback?.Invoke(new GameObject());

            // Assert
            Assert.IsTrue(_featuresUI.activeSelf);
        }
    }
}