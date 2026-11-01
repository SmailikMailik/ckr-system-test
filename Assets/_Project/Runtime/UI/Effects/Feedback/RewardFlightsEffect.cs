using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Project.UI.Effects.Feedback
{
    /// <summary>
    /// Показывает числовую награду и разлёт UI-частиц, владея их анимациями и пулом.
    /// </summary>
    [AddComponentMenu("Project/UI/Effects/Reward Flights")]
    internal sealed class RewardFlightsEffect : MonoBehaviour
    {
        [SerializeField] private TMP_Text _rewardLabelTemplate;
        [SerializeField] private Image _coinTemplate;
        [SerializeField] private RectTransform _coinTarget;
        [Header("Летящая награда")]
        [SerializeField] private Vector2 _rewardOffset = new(0, 220);
        [SerializeField, Min(0.01f)] private float _rewardDurationSeconds = 0.8f;
        [SerializeField] private Ease _flightEase = Ease.OutQuad;
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;

        [Header("Разлёт UI-частиц")]
        [SerializeField, Range(0, 32)] private int _particleCount = 6;
        [SerializeField, Min(0)] private float _particleStartRadius = 24;
        [SerializeField, Min(0)] private float _particleDistance = 150;
        [SerializeField, Min(0.01f)] private float _particleDurationSeconds = 0.45f;
        [SerializeField, Min(0.01f)] private float _particleScale = 0.55f;

        private const int MaxVisibleFlights = 64;
        private const int InitialFlightPoolCapacity = 8;
        private const float RadiansPerRevolution = 2 * Mathf.PI;

        private readonly List<Flight> _active = new();
        private ObjectPool<Flight> _pool;

        private void OnDisable()
        {
            Stop();
        }

        private void OnDestroy()
        {
            Stop();
            _pool?.Dispose();
        }

        /// <summary>
        /// Показывает переданное значение; повторный запуск добавляет полёты в пределах общего лимита.
        /// </summary>
        public void Play(int reward)
        {
            if (isActiveAndEnabled is false)
                return;

            EnsurePool();
            AnimateCoin();
            AnimateFlight($"+{reward}", Vector2.zero, _rewardOffset, _rewardDurationSeconds, 1);

            for (var i = 0; i < _particleCount; i++)
            {
                var angle = i * RadiansPerRevolution / _particleCount;
                var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                AnimateFlight(
                    "•",
                    direction * _particleStartRadius,
                    direction * _particleDistance,
                    _particleDurationSeconds,
                    _particleScale);
            }
        }

        /// <summary>
        /// Отменяет текущие полёты и возвращает элементы в пул без выполнения завершения анимации.
        /// </summary>
        public void Stop()
        {
            if (_pool is null)
                return;

            for (var i = _active.Count - 1; i >= 0; i--)
                ReturnFlight(_active[i]);
        }

        private void EnsurePool()
        {
            if (_pool is not null)
                return;

            _rewardLabelTemplate.gameObject.SetActive(false);

            if (_coinTemplate != null)
                _coinTemplate.gameObject.SetActive(false);

            _pool = new(CreateFlight, null, null, DestroyFlight, true, InitialFlightPoolCapacity, MaxVisibleFlights);
        }

        private Flight CreateFlight()
        {
            var label = Instantiate(_rewardLabelTemplate, _rewardLabelTemplate.transform.parent);
            label.raycastTarget = false;
            var coin = (Image)null;

            if (_coinTemplate != null)
            {
                coin = Instantiate(_coinTemplate, _coinTemplate.transform.parent);
                coin.raycastTarget = false;
            }

            return new(label, coin);
        }

        private void AnimateCoin()
        {
            if (_coinTemplate == null || _coinTarget == null || _active.Count >= MaxVisibleFlights)
                return;

            var flight = _pool.Get();
            var coin = flight.Coin;
            coin.color = _coinTemplate.color;
            coin.sprite = _coinTemplate.sprite;
            coin.transform.position = _coinTemplate.transform.position;
            coin.transform.localScale = _coinTemplate.transform.localScale;
            coin.gameObject.SetActive(true);
            _active.Add(flight);
            var sequence = DOTween.Sequence();
            sequence.Append(coin.rectTransform.DOMove(_coinTarget.position, _rewardDurationSeconds).SetEase(_flightEase));
            sequence.Join(coin.rectTransform.DOScale(coin.transform.localScale * 0.45f, _rewardDurationSeconds));
            sequence.SetAutoKill(true).SetRecyclable(false).SetUpdate(true).SetLink(gameObject)
                .OnComplete(() => ReturnFlight(flight))
                .OnKill(() => flight.Motion = null);
            flight.Motion = sequence;
            sequence.Play();
        }

        private void AnimateFlight(string text, Vector2 start, Vector2 end, float duration, float scale)
        {
            if (_active.Count >= MaxVisibleFlights)
                return;

            var flight = _pool.Get();
            var label = flight.Label;
            var origin = _rewardLabelTemplate.rectTransform.anchoredPosition;
            label.text = text;
            label.color = _rewardLabelTemplate.color;
            label.rectTransform.localScale = _rewardLabelTemplate.rectTransform.localScale * scale;
            label.rectTransform.anchoredPosition = origin + start;
            label.gameObject.SetActive(true);
            _active.Add(flight);
            var sequence = DOTween.Sequence();
            sequence.Append(label.rectTransform.DOAnchorPos(origin + end, duration).SetEase(_flightEase));
            sequence.Join(DOTween.To(() => label.alpha, value => label.alpha = value, 0, duration).SetEase(_fadeEase));
            sequence.SetAutoKill(true).SetRecyclable(false).SetUpdate(true).SetLink(gameObject)
                .OnComplete(() => ReturnFlight(flight))
                .OnKill(() => flight.Motion = null);
            flight.Motion = sequence;
            sequence.Play();
        }

        private void ReturnFlight(Flight flight)
        {
            if (_active.Remove(flight) is false)
                return;

            var motion = flight.Motion;
            flight.Motion = null;
            motion?.Kill();

            if (flight.Label != null)
                flight.Label.gameObject.SetActive(false);

            if (flight.Coin != null)
                flight.Coin.gameObject.SetActive(false);

            _pool.Release(flight);
        }

        private void DestroyFlight(Flight flight)
        {
            flight.Motion?.Kill();

            if (flight.Label != null)
                Destroy(flight.Label.gameObject);

            if (flight.Coin != null)
                Destroy(flight.Coin.gameObject);
        }

        /// <summary>
        /// Связывает повторно используемые текст и монету с анимацией текущего показа.
        /// </summary>
        private sealed class Flight
        {
            internal TMP_Text Label { get; }
            internal Image Coin { get; }
            internal Tween Motion { get; set; }

            /// <summary>
            /// Объединяет созданные элементы одного полёта; изображение монеты может отсутствовать.
            /// </summary>
            internal Flight(TMP_Text label, Image coin)
            {
                Label = label;
                Coin = coin;
            }
        }
    }
}