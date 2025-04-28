using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class ObstacleRegistry : MonoBehaviour
    {
        public static ObstacleRegistry Instance { get; private set; }

        public readonly List<Transform> Obstacles = new();

        private void Awake()
        {
            Instance = this;
        }

        public void Register(Transform obstacle)
        {
            if (!Obstacles.Contains(obstacle))
                Obstacles.Add(obstacle);
        }

        public void Unregister(Transform obstacle)
        {
            Obstacles.Remove(obstacle);
        }
    }
}