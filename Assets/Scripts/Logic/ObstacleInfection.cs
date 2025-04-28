using UnityEngine;
using DG.Tweening;

namespace Logic
{
    public class ObstacleInfection : MonoBehaviour
    {
        [SerializeField] private float _shakeDuration = 0.2f;
        [SerializeField] private float _shakeStrength = 0.2f;

        public void Infect()
        {
            transform.DOShakeScale(_shakeDuration, _shakeStrength);
            Invoke(nameof(Deactivate), _shakeDuration);
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}