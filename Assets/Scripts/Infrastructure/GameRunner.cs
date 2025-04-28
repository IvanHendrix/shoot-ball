using UnityEngine;

namespace Infrastructure
{
    public class GameRunner : MonoBehaviour,ICoroutineRunner
    {
        private void Awake()
        {
            Game game = new Game(LocalServices.Container, this);
            
            DontDestroyOnLoad(this);
        }
    }
}