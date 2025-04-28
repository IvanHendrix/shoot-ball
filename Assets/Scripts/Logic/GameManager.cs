using Data;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Logic
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ObstacleManager _obstacleManager;

        [SerializeField] private LevelCompletePanel _levelCompletePanel;

        [SerializeField] private PlayerShoot _playerShoot;
        [SerializeField] private ShotBallMover _shotBallMover;

        [SerializeField] private GameObject _explosionEffect;
        [SerializeField] private Transform _target;

        [SerializeField] private LineRenderer _pathLine;

        private float _shotSpeed = GameSettingData.ShotSpeed;

        private Vector3 _direction;
        private Vector3 _spawnPosition;

        private void Start()
        {
            _obstacleManager.Init(_playerShoot.transform.position, _target.position);
            _obstacleManager.OnAllExplosionsComplete += AllExplosionsComplete;

            _levelCompletePanel.OnReloadClick += ReloadLevel;

            _playerShoot.OnStartChargingEvent += StartCharging;
            _playerShoot.OnChargeEvent += BallCharge;
            _playerShoot.OnFireEvent += FireShot;
            _playerShoot.OnCriticalSizeEvent += Lose;

            _shotBallMover.OnHitObstacle += HandleObstacleHit;
            _shotBallMover.OnMissed += OnShotMissed;

            _direction = (_target.position - _playerShoot.transform.position).normalized;

            SetupPathLine();

            _playerShoot.SetCanFireState();

            CalculateShotPosition();
            _shotBallMover.Prepare(_spawnPosition);
        }

        private void OnShotMissed()
        {
            _playerShoot.SetCanFireState();
            _shotBallMover.Prepare(_spawnPosition);

            CheckWinCondition();
        }

        private void CalculateShotPosition()
        {
            Vector3 directionToTarget = (_target.position - _playerShoot.transform.position).normalized;
            float spawnOffset = 1.5f;
            _spawnPosition = _playerShoot.transform.position + directionToTarget * spawnOffset;
        }

        private void ReloadLevel()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

        private void AllExplosionsComplete()
        {
            CheckWinCondition();

            _playerShoot.SetCanFireState();
        }

        private void FireShot()
        {
            _shotBallMover.Fire(_direction, _shotSpeed);

            UpdatePathWidth();
        }

        private void BallCharge(float delta)
        {
            _shotBallMover.Grow(delta);
        }

        private void StartCharging()
        {
            _shotBallMover.Prepare(_spawnPosition);
        }

        private void HandleObstacleHit(Vector3 hitPosition, float explosionRadius)
        {
            Instantiate(_explosionEffect, hitPosition, Quaternion.identity);

            _shotBallMover.Reset();
            _shotBallMover.gameObject.SetActive(false);
            _obstacleManager.HandleObstacleExplosion(hitPosition, explosionRadius);
        }

        private void SetupPathLine()
        {
            _pathLine.positionCount = 2;
            _pathLine.SetPosition(0,
                new Vector3(_playerShoot.transform.position.x, -1,
                    _playerShoot.transform.position.z));
            _pathLine.SetPosition(1, new Vector3(_target.position.x, -1, _target.position.z));

            UpdatePathWidth();
        }

        private void UpdatePathWidth()
        {
            float width = _playerShoot.transform.localScale.x;
            _pathLine.startWidth = width;
            _pathLine.endWidth = width;
        }

        private void CheckWinCondition()
        {
            Vector3 direction = (_target.position - _playerShoot.transform.position).normalized;
            float distance = Vector3.Distance(_playerShoot.transform.position, _target.position);
            float radius = _playerShoot.transform.localScale.x;

            if (Physics.SphereCast(_playerShoot.transform.position, radius, direction, out RaycastHit hit, distance))
            {
                if (hit.collider.CompareTag("Obstacle"))
                {
                    return;
                }
            }
            
            Win();
        }

        private void Win()
        {
            _playerShoot.StartMoving(_target.position);
            _levelCompletePanel.SetText("Win");
            _levelCompletePanel.SetActivePanel();
            Debug.Log("Win");
        }

        private void Lose()
        {
            _shotBallMover.Prepare(_spawnPosition);
            _levelCompletePanel.SetText("Lose");
            _levelCompletePanel.SetActivePanel();
            Debug.Log("Lose");
        }

        private void OnDestroy()
        {
            _obstacleManager.OnAllExplosionsComplete -= AllExplosionsComplete;

            _playerShoot.OnStartChargingEvent -= StartCharging;
            _playerShoot.OnChargeEvent -= BallCharge;
            _playerShoot.OnFireEvent -= FireShot;
            _playerShoot.OnCriticalSizeEvent -= Lose;

            _shotBallMover.OnHitObstacle -= HandleObstacleHit;
        }
    }
}