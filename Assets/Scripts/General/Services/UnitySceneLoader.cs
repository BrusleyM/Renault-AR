using Common.interfaces;
using UnityEngine.SceneManagement;

namespace Services
{
    public class UnitySceneLoader : ISceneLoader
    {
        public void Load(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
