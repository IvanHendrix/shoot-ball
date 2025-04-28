using System;
using Data;
using UnityEngine;

namespace Logic
{
    public class ShotBallMover : MonoBehaviour
    {
        public event Action<Vector3, float> OnHitObstacle;
        public event Action OnMissed;

        private Vector3 _direction;
        private Vector3 _startPosition;
        
        private float _speed;
        private bool _isFlying;
        private float _explosionRatio = GameSettingData.ExplosionRatio;

        private void Update()
        {
            if (_isFlying)
            {
                transform.position += _direction * (_speed * Time.deltaTime);
                if (Vector3.Distance(_startPosition, transform.position) > 15f)
                {
                    OnMissed?.Invoke();
                }
            }
        }

        public void Prepare(Vector3 startPosition)
        {
            _startPosition = startPosition;
            transform.position = _startPosition;
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            _isFlying = false;
        }
        
        public void Reset()
        {
            gameObject.SetActive(true);
            _isFlying = false;
        }

        public void Grow(float delta)
        {
            transform.localScale += Vector3.one * delta;
        }

        public void Fire(Vector3 direction, float speed)
        {
            _direction = direction;
            _speed = speed;
            _isFlying = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            OnHitObstacle?.Invoke(transform.position, transform.localScale.x * _explosionRatio);
        }
    }
}