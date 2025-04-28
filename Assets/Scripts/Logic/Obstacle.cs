using System;
using Data;
using UnityEngine;
using DG.Tweening;

namespace Logic
{
    public class Obstacle : MonoBehaviour
    {
        public event Action<Vector3, float> OnExploded;
        public event Action<Obstacle> OnExplosionFinished;

        public bool IsExploded;

        [SerializeField] private Renderer _renderer;
        [SerializeField] private ParticleSystem _explosionEffectPrefab;

        private float _exlosionRadius = GameSettingData.ObstacleExplosionRadius;

        public void StartExplosion()
        {
            if (IsExploded)
            {
                return;
            }

            IsExploded = true;

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOShakePosition(0.5f, 0.5f, 30))
                .Join(_renderer.material.DOColor(Color.yellow, 0.5f))
                .AppendInterval(0.2f)
                .AppendCallback(() =>
                {
                    if (_explosionEffectPrefab != null)
                    {
                        Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
                    }

                    OnExplosionFinished?.Invoke(this);

                    Destroy(gameObject);

                    OnExploded?.Invoke(transform.position, _exlosionRadius);
                });
        }
    }
}