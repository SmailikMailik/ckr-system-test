using TMPro;
using UnityEngine;

namespace Project.UI.Settings
{
    /// <summary>
    /// Хранит цвета, шрифты и базовые размеры компонентов интерфейса.
    /// </summary>
    /// <remarks>
    /// Размеры и отступы заданы в единицах опорного Canvas; назначение настроек показано в Inspector.
    /// </remarks>
    internal sealed class UiToolkitTheme : ScriptableObject
    {
        [Tooltip("Фон экранного интерфейса.")]
        [SerializeField] private Color _background = new Color32(247, 250, 255, 255);
        [Tooltip("Поверхность карточек и окон.")]
        [SerializeField] private Color _surface = Color.white;
        [Tooltip("Основной цвет текста.")]
        [SerializeField] private Color _text = new Color32(38, 54, 78, 255);
        [Tooltip("Цвет второстепенных подписей и неактивных иконок.")]
        [SerializeField] private Color _muted = new Color32(100, 115, 138, 255);
        [Tooltip("Светлая поверхность основного действия.")]
        [SerializeField] private Color _primary = new Color32(220, 234, 255, 255);
        [Tooltip("Контрастный текст на светлой поверхности основного действия.")]
        [SerializeField] private Color _primaryText = new Color32(38, 65, 95, 255);
        [Tooltip("Читаемый акцент небольших иконок, активной навигации и прогресса.")]
        [SerializeField] private Color _accent = new Color32(76, 111, 155, 255);
        [Tooltip("Акцент нажатого основного действия.")]
        [SerializeField] private Color _primaryPressed = new Color32(200, 218, 246, 255);
        [Tooltip("Заливка недоступного элемента.")]
        [SerializeField] private Color _disabled = new Color32(231, 236, 243, 255);
        [Tooltip("Цвет условной валюты.")]
        [SerializeField] private Color _gold = new Color32(168, 117, 37, 255);
        [Tooltip("Цвет разделителей и треков.")]
        [SerializeField] private Color _line = new Color32(225, 233, 244, 255);
        [Tooltip("Боковое поле экрана в единицах опорного Canvas.")]
        [SerializeField] private float _screenPadding = 60;
        [Tooltip("Минимальный размер области нажатия в единицах опорного Canvas.")]
        [SerializeField] private float _minimumTouchSize = 144;
        [Tooltip("Размер основного текста в единицах опорного Canvas.")]
        [SerializeField] private float _bodySize = 42;
        [Tooltip("Размер заголовка экрана в единицах опорного Canvas.")]
        [SerializeField] private float _titleSize = 78;
        [Tooltip("Шрифт обычного текста и описаний.")]
        [SerializeField] private TMP_FontAsset _bodyFont;
        [Tooltip("Начертание заголовков и важных числовых значений.")]
        [SerializeField] private TMP_FontAsset _headingFont;

        internal Color Background => _background;
        internal Color Surface => _surface;
        internal Color Text => _text;
        internal Color Muted => _muted;
        internal Color Primary => _primary;
        internal Color PrimaryText => _primaryText;
        internal Color Accent => _accent;
        internal Color PrimaryPressed => _primaryPressed;
        internal Color Disabled => _disabled;
        internal Color Gold => _gold;
        internal Color Line => _line;
        internal float ScreenPadding => _screenPadding;
        internal float MinimumTouchSize => _minimumTouchSize;
        internal float BodySize => _bodySize;
        internal float TitleSize => _titleSize;
        internal TMP_FontAsset BodyFont => _bodyFont;
        internal TMP_FontAsset HeadingFont => _headingFont;
    }
}