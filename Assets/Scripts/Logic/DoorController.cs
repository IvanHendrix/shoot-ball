using UnityEngine;
using DG.Tweening;

namespace Logic
{
    public class DoorController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform _player;
        
        [SerializeField] private float _openDistance = 5f;
        [SerializeField] private float _openHeight = 5f;
        [SerializeField] private float _openDuration = 1f;

        private bool _isOpened = false;

        private void Update()
        {
            if (_isOpened)
            {
                return;
            }
            
            if (Vector3.Distance(transform.position, _player.position) <= _openDistance)
            {
                OpenDoor();
            }
        }

        private void OpenDoor()
        {
            _isOpened = true;

            transform.DOMoveY(transform.position.y + _openHeight, _openDuration)
                .SetEase(Ease.OutCubic);
        }
    }
}