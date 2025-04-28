using System;
using System.Collections.Generic;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic
{
    public class ObstacleManager : MonoBehaviour
    {
        public event Action OnAllExplosionsComplete;

        [Header("References")] 
        [SerializeField]
        private Obstacle _obstaclePrefab;

        [SerializeField] private Transform _playerBall;
        [SerializeField] private Transform _endPoint;

        [Header("Settings")]
        [SerializeField] private float _playerInitialSize = GameSettingData.PlayerInitialSize;
        [SerializeField] private float _obstacleSize = GameSettingData.ObstacleSize;
        [SerializeField] private float _pathExtraMargin = 5f;
        [SerializeField] private float _visualWidthMultiplier = 2f;
        [SerializeField] private int _pathVisualPoints = 25;
        [SerializeField] private float _safeZoneStart = 5f;
        [SerializeField] private float _safeZoneEnd = 5f;

        private List<Obstacle> _allObstacles = new List<Obstacle>();
        private int _activeExplosions = 0;

        private Vector3 _startPos;
        private Vector3 _endPos;
        private float _startZ;
        private float _endZ;
        private float _pathDistance;
        private float _pathWidth;
        private float _visualWidth;

        public void Init(Vector3 startPos, Vector3 endPos)
        {
            _startPos = startPos;
            _endPos = endPos;
            _startZ = startPos.z;
            _endZ = endPos.z;
            _pathDistance = _endZ - _startZ;

            _pathWidth = _playerInitialSize * 2f + _pathExtraMargin;
            _visualWidth = _pathWidth * _visualWidthMultiplier;


            SpawnObstacles();
            SpawnVisualObstacles();
        }

        private void SpawnObstacles()
        {
            Vector3 start = _startPos;
            Vector3 end = _endPos;
            Vector3 pathDirection = (end - start).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, pathDirection).normalized;

            float usableDistance = _pathDistance - _safeZoneStart - _safeZoneEnd;
            int zoneCount = Mathf.CeilToInt(usableDistance / 0.5f); 

            for (int i = 0; i <= zoneCount; i++)
            {
                float t = (float)i / zoneCount;
                float zPos = _startZ + _safeZoneStart + t * usableDistance;
                Vector3 basePos = new Vector3(start.x, 0f, zPos);
                
                float slightCenterOffsetX =
                    Random.Range(-_playerInitialSize * 0.2f,
                        _playerInitialSize * 0.2f); 
                float slightCenterOffsetZ = Random.Range(-0.3f, 0.3f);
                Vector3 centerOffset = basePos + right * slightCenterOffsetX + pathDirection * slightCenterOffsetZ;
                CreateImportantObstacle(centerOffset);
                
                int importantExtras = Random.Range(1, 3);
                for (int j = 0; j < importantExtras; j++)
                {
                    float lateralOffset =
                        Random.Range(-_playerInitialSize * 0.5f, _playerInitialSize * 0.5f);
                    float zExtraOffset = Random.Range(-0.5f, 0.5f);
                    Vector3 sideSpawn = basePos + right * lateralOffset + pathDirection * zExtraOffset;
                    CreateImportantObstacle(sideSpawn);
                }
                
                int extraVisuals = Random.Range(3, 6);
                for (int j = 0; j < extraVisuals; j++)
                {
                    float lateralOffset = Random.Range(-_visualWidth / 2f, _visualWidth / 2f);
                    float zExtraOffset = Random.Range(-0.5f, 0.5f);
                    Vector3 spawnPos = basePos + right * lateralOffset + pathDirection * zExtraOffset;
                    CreateVisualObstacle(spawnPos);
                }
            }
        }

        private void CreateImportantObstacle(Vector3 position)
        {
            Obstacle importantObstacle = Instantiate(_obstaclePrefab, position, Quaternion.identity, transform);
            importantObstacle.OnExploded += HandleObstacleExplosion;
            importantObstacle.OnExplosionFinished += HandleObstacleExplosionFinished;
            _allObstacles.Add(importantObstacle);
        }

        private void SpawnVisualObstacles()
        {
            Vector3 start = _playerBall.position;
            Vector3 end = _endPoint.position;
            Vector3 pathDirection = (end - start).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, pathDirection).normalized;

            for (int i = 0; i <= _pathVisualPoints; i++)
            {
                float t = (float)i / (_pathVisualPoints - 1);
                Vector3 basePos = Vector3.Lerp(start, end, t);

                for (float offset = _playerInitialSize * 1.5f;
                     offset <= _visualWidth / 2f;
                     offset += _obstaclePrefab.transform.localScale.x * 1.5f)
                {
                    if (Random.value < 0.5f)
                    {
                        Vector3 spawnPosLeft = basePos - right * offset;
                        CreateVisualObstacle(spawnPosLeft);
                    }

                    if (Random.value < 0.5f)
                    {
                        Vector3 spawnPosRight = basePos + right * offset;
                        CreateVisualObstacle(spawnPosRight);
                    }
                }
            }
        }

        private void CreateVisualObstacle(Vector3 position)
        {
            Obstacle visualObstacle = Instantiate(_obstaclePrefab, position, Quaternion.identity, transform);
            visualObstacle.transform.localScale = new Vector3(_obstacleSize, 1f, _obstacleSize);
            visualObstacle.OnExploded += HandleObstacleExplosion;
            visualObstacle.OnExplosionFinished += HandleObstacleExplosionFinished;
            _allObstacles.Add(visualObstacle);
        }

        public void HandleObstacleExplosion(Vector3 position, float radius)
        {
            foreach (var obstacle in _allObstacles)
            {
                if (!obstacle.IsExploded)
                {
                    float distance = Vector3.Distance(position, obstacle.transform.position);
                    if (distance <= radius + _obstacleSize * 0.5f)
                    {
                        obstacle.StartExplosion();
                        _activeExplosions++;
                    }
                }
            }
        }

        private void HandleObstacleExplosionFinished(Obstacle finishedObstacle)
        {
            _activeExplosions--;

            if (_activeExplosions <= 0)
            {
                Debug.Log("All explosions completed!");
                OnAllExplosionsComplete?.Invoke();
            }
        }
    }
}