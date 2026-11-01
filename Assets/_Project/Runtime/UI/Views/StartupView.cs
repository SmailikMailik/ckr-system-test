using System;
using Project.UI.Effects.Playback;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Views
{
    /// <summary>
    /// Показывает состояние запуска приложения и сообщает о запросе повторного запуска.
    /// </summary>
    internal sealed class StartupView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _stateLabel;
        [SerializeField] private GameObject _loadingIndicator;
        [SerializeField] private EffectPlayer _loadingEffects;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Image _heroImage;
        [SerializeField] private Sprite _loadingHero;
        [SerializeField] private Sprite _errorHero;
        [SerializeField, Min(0.05f)] private float _dotIntervalSeconds = 0.4f;

        private const string LoadingText = "Загружаем...";
        private const int MaximumDots = 3;

        private float _dotElapsedSeconds;
        private int _visibleDots = 1;

        private bool _bound;
        private bool _isLoading = true;

        /// <summary>
        /// Сообщает о запросе повторить запуск приложения.
        /// </summary>
        internal event Action RetryRequested;

        private void OnEnable()
        {
            if (_isLoading)
                ShowLoading();
        }

        private void Update()
        {
            if (_isLoading is false)
                return;

            _dotElapsedSeconds += Time.unscaledDeltaTime;

            if (_dotElapsedSeconds < _dotIntervalSeconds)
                return;

            var intervals = Mathf.FloorToInt(_dotElapsedSeconds / _dotIntervalSeconds);
            _dotElapsedSeconds %= _dotIntervalSeconds;
            _visibleDots = (_visibleDots - 1 + intervals) % MaximumDots + 1;
            _stateLabel.maxVisibleCharacters = LoadingText.Length - MaximumDots + _visibleDots;
        }

        private void OnDisable()
        {
            if (_loadingEffects != null)
                _loadingEffects.Stop();
        }

        private void OnDestroy()
        {
            Unbind();
        }

        /// <summary>
        /// Подключает обработку кнопки повторного запуска; повторный вызов сохраняет единственную подписку.
        /// </summary>
        internal void Bind()
        {
            if (_bound)
                return;

            _retryButton.onClick.AddListener(RequestRetry);
            _bound = true;
        }

        /// <summary>
        /// Отключает обработку кнопки, сохраняя возможность повторного подключения.
        /// </summary>
        internal void Unbind()
        {
            if (_bound is false)
                return;

            if (_retryButton != null)
                _retryButton.onClick.RemoveListener(RequestRetry);

            _bound = false;
        }

        /// <summary>
        /// Показывает загрузку с циклом точек, сохраняя полную строку для стабильной вёрстки.
        /// </summary>
        internal void ShowLoading()
        {
            _isLoading = true;
            _dotElapsedSeconds = 0;
            _visibleDots = 1;
            _stateLabel.text = LoadingText;
            _stateLabel.maxVisibleCharacters = LoadingText.Length - MaximumDots + _visibleDots;
            _heroImage.sprite = _loadingHero;
            _retryButton.gameObject.SetActive(false);
            _loadingIndicator.SetActive(true);
            _loadingEffects.Play();
        }

        /// <summary>
        /// Останавливает индикацию загрузки и делает повторную попытку доступной.
        /// </summary>
        internal void ShowError()
        {
            _isLoading = false;
            _stateLabel.maxVisibleCharacters = int.MaxValue;
            _stateLabel.text = "Не удалось\nзагрузить";
            _heroImage.sprite = _errorHero;
            _loadingIndicator.SetActive(false);
            _loadingEffects.Stop();
            _retryButton.gameObject.SetActive(true);
        }

        private void RequestRetry()
        {
            RetryRequested?.Invoke();
        }
    }
}