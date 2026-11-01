using System;
using DG.Tweening;
using Project.UI.Effects.Playback;
using UnityEngine;

namespace Project.UI.Effects.Animations
{
    /// <summary>
    /// Непрерывно вращает объект вокруг локальной оси Z в настроенном направлении по сигналу проигрывателя.
    /// </summary>
    [AddComponentMenu("Project/UI/Effects/Rotator")]
    [DisallowMultipleComponent]
    internal sealed class UiRotator : TweenEffect
    {
        [SerializeField, Min(0), Tooltip("Скорость вращения в градусах в секунду; ноль отключает вращение.")] private float _degreesPerSecond = 400;
        [SerializeField] private RotationDirection _direction = RotationDirection.Clockwise;

        private const float DegreesPerRevolution = 360;
        // DOTween использует -1 для бесконечного повторения.
        private const int InfiniteLoops = -1;

        private bool _hasInitialRotation;
        private Quaternion _initialRotation;

        /// <summary>
        /// Создаёт равномерное вращение, сохраняя исходный поворот до первого запуска.
        /// </summary>
        /// <returns>Анимация вращения или null при скорости, близкой к нулю.</returns>
        protected override Tween CreateTween()
        {
            var axis = _direction switch
            {
                RotationDirection.Clockwise => Vector3.back,
                RotationDirection.CounterClockwise => Vector3.forward,
                _ => throw new ArgumentOutOfRangeException(nameof(_direction), _direction, "Неизвестное направление вращения.")
            };

            if (Mathf.Approximately(_degreesPerSecond, 0))
                return null;

            if (_hasInitialRotation is false)
            {
                _initialRotation = transform.localRotation;
                _hasInitialRotation = true;
            }

            var turn = axis * DegreesPerRevolution;
            var durationSeconds = DegreesPerRevolution / Mathf.Abs(_degreesPerSecond);
            return transform.DOLocalRotate(turn, durationSeconds, RotateMode.FastBeyond360)
                .SetRelative()
                .SetEase(Ease.Linear)
                .SetLoops(InfiniteLoops, LoopType.Restart);
        }

        /// <summary>
        /// Возвращает исходный поворот после отмены вращения.
        /// </summary>
        protected override void RestoreState()
        {
            if (_hasInitialRotation)
                transform.localRotation = _initialRotation;
        }

        /// <summary>
        /// Определяет направление вращения отдельно от его неотрицательной скорости.
        /// </summary>
        private enum RotationDirection : byte
        {
            [InspectorName("По часовой стрелке")]
            Clockwise = 0,
            [InspectorName("Против часовой стрелки")]
            CounterClockwise = 1
        }
    }
}