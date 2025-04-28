using Services.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure
{
    public class Game
    {
        private readonly LocalServices _services;

        public Game(LocalServices services, ICoroutineRunner coroutineRunner)
        {
            _services = services;

            RegisterServices();
            
            SceneManager.LoadSceneAsync("Main");
        }
        
        private void RegisterServices()
        {
            _services.RegisterSingle<IInputService>(InputService());
        }

        private IInputService InputService()
        {
            return Application.isEditor
                ? new MouseInputService()
                : new MobileInputService();
        }
    }
}