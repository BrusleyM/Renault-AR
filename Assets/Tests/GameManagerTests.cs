using NUnit.Framework;
using UnityEngine;
using Common.Objects;
using Managers;
using Common.interfaces;
using NSubstitute;

namespace Tests
{
    public class GameManagerTests
    {
        private GameManager _gameManager;
        private GameObject _gmObject;

        [SetUp]
        public void SetUp()
        {
            var type = typeof(GameManager);
            var field = type.GetField("_instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            field.SetValue(null, null);

            _gmObject = new GameObject("GameManager");
            _gameManager = _gmObject.AddComponent<GameManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gmObject);
        }

        [Test]
        public void SetSelectedCar_SetsCarAndName()
        {
            var carPrefab = new GameObject("Car");
            string name = "Lamborghini";

            _gameManager.SetSelectedCar(carPrefab, name);

            Assert.AreEqual(carPrefab, _gameManager.SelectedCar);
            Assert.AreEqual(name, _gameManager.CarName);
        }

        [Test]
        public void SetInstantiatedCar_SetsCar()
        {
            var instantiatedCar = new GameObject("InstantiatedCar");

            _gameManager.SetInstantiatedCar(instantiatedCar);

            Assert.AreEqual(instantiatedCar, _gameManager.InstantiatedCar);
        }

        [Test]
        public void SetUserInfo_SetsPersonData()
        {
            var person = new Person
            {
                Name = "Phashe",
                Surname = "Masemola",
                ID = "123456789"
            };

            _gameManager.SetUserInfo(person);

            Assert.AreEqual("Phashe", _gameManager.UserInfo.Name);
            Assert.AreEqual("Masemola", _gameManager.UserInfo.Surname);
            Assert.AreEqual("123456789", _gameManager.UserInfo.ID);
        }

        [Test]
        public void LoadScene_UsesSceneLoader()
        {
            var mockLoader = Substitute.For<ISceneLoader>();
            _gameManager.SetSceneLoader(mockLoader);

            _gameManager.LoadScene("TestScene");

            mockLoader.Received(1).Load("TestScene");
        }

        [Test]
        public void LoadScene_DoesNotThrow_IfLoaderIsNull()
        {
            _gameManager.SetSceneLoader(null);

            Assert.DoesNotThrow(() => _gameManager.LoadScene("SomeScene"));
        }

        [Test]
        public void GameManager_IsSingleton()
        {
            Assert.AreEqual(_gameManager,GameManager.Instance);
        }

        [Test]
        public void GameManager_DestroysDuplicateInstances()
        {
            // Arrange
            var duplicateGO = new GameObject("DuplicateGM");
            duplicateGO.AddComponent<GameManager>();

            // Act
            Object.DestroyImmediate(duplicateGO);

            // Assert
            Assert.AreEqual(_gameManager, GameManager.Instance);
        }
    }
}