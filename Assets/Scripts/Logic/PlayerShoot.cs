using System;
using Data;
using DG.Tweening;
using Infrastructure;
using Services.Input;
using UnityEngine;

namespace Logic
{
    public class PlayerShoot : MonoBehaviour
    {
        public event Action OnStartChargingEvent;
        public event Action<float> OnChargeEvent;
        public event Action OnFireEvent;
        public event Action OnCriticalSizeEvent;

        [Header("Settings")] 
        [SerializeField] private float _jumpPower = 2f;
        [SerializeField] private float _jumpDuration = 0.5f;
        [SerializeField] private float _jumpDistance = 2f;
        [SerializeField] private int _jumpCount = 1;

        private float _chargeSpeed = GameSettingData.ChargeSpeed;
        private float _minPlayerSize = GameSettingData.MinPlayerSize;

        private bool _isCharging;
        private bool _canFire;

        private IInputService _input => LocalServices.Container.Single<IInputService>();

        private bool _isMoving;
        private Vector3 _direction;
        private Vector3 _endPosition;
        private Sequence _jumpSequence;

        private void Start()
        {
            transform.localScale = new Vector3(GameSettingData.PlayerInitialSize, GameSettingData.PlayerInitialSize,
                GameSettingData.PlayerInitialSize);
        }

        private void Update()
        {
            if (_input.IsTouchStarted() && !_isCharging && CanShoot())
            {
                StartCharging();
                return;
            }

            if (_isCharging && _input.IsTouching())
            {
                Charge();
                return;
            }

            if (_input.IsTouchEnded())
            {
                Fire();
            }
        }

        public void SetCanFireState()
        {
            _canFire = true;
        }

        public void StartMoving(Vector3 endPosition)
        {
            _endPosition = endPosition;
            _direction = (endPosition - transform.position).normalized;
            _isMoving = true;
            StartJumping();
        }

        public void StopMoving()
        {
            _isMoving = false;

            if (_jumpSequence != null)
            {
                _jumpSequence.Kill();
            }
        }

        private bool CanShoot()
        {
            return _canFire;
        }

        private void StartCharging()
        {
            _isCharging = true;
            _canFire = true;

            OnStartChargingEvent?.Invoke();
        }

        private void Charge()
        {
            float delta = _chargeSpeed * Time.deltaTime;

            if (!CanShoot())
            {
                _isCharging = false;
                return;
            }

            if (transform.localScale.x <= GameSettingData.PlayerCriticalSize)
            {
                OnCriticalSizeEvent?.Invoke();
                return;
            }
            
            transform.localScale -= Vector3.one * delta;
            OnChargeEvent?.Invoke(delta);
        }

        private void Fire()
        {
            _isCharging = false;
            _canFire = false;
            OnFireEvent?.Invoke();
        }

        private void StartJumping()
        {
            if (_jumpSequence != null && _jumpSequence.IsActive())
            {
                _jumpSequence.Kill();
            }
            
            _jumpSequence = DOTween.Sequence();

            JumpForward();
        }

        private void JumpForward()
        {
            if (!_isMoving || Vector3.Distance(transform.position, _endPosition) < 0.1f)
            {
                return;
            }

            Vector3 targetPosition = transform.position + _direction * _jumpDistance;

            _jumpSequence.Append(transform.DOJump(targetPosition, _jumpPower, _jumpCount, _jumpDuration)
                .SetEase(Ease.Linear)
                .OnComplete(JumpForward));
        }
    }
}