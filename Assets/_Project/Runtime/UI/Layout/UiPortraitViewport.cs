using UnityEngine;

namespace Project.UI.Layout
{
    /// <summary>
    /// Ограничивает ширину интерфейса портретными пропорциями, используя всю высоту родителя.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    internal sealed class UiPortraitViewport : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float _maximumAspectRatio = 9f / 16f;

        private void OnValidate()
        {
            ApplyWidth();
        }

        private void OnEnable()
        {
            ApplyWidth();
        }

        private void OnRectTransformDimensionsChange()
        {
            ApplyWidth();
        }

        private void ApplyWidth()
        {
            var rect = (RectTransform)transform;
            var parent = (RectTransform)rect.parent;

            if (parent == null)
                return;

            var width = Mathf.Min(parent.rect.width, parent.rect.height * _maximumAspectRatio);

            if (Mathf.Approximately(rect.sizeDelta.x, width) is false)
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }
    }
}