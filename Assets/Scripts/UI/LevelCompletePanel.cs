using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelCompletePanel : MonoBehaviour
    {
        public event Action OnReloadClick;

        [SerializeField] private GameObject _contriner;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _reloadButton;

        private void Start()
        {
            _reloadButton.onClick.AddListener(OnReloadButtonClick);
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetActivePanel()
        {
            _contriner.SetActive(true);
        }

        private void OnReloadButtonClick()
        {
            OnReloadClick?.Invoke();
        }

        private void OnDestroy()
        {
            _reloadButton.onClick.RemoveListener(OnReloadButtonClick);
        }
    }
}