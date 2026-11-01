using TMPro;
using UnityEngine;

namespace Project.UI.Effects.Typography
{
    /// <summary>
    /// Изгибает вершины TMP-текста по параболической дуге отдельно для каждой строки после вёрстки.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    [AddComponentMenu("Project/UI/Effects/Curved Text")]
    internal sealed class CurvedText : MonoBehaviour
    {
        [SerializeField, Tooltip("Высота дуги в единицах Canvas: положительное значение поднимает середину строки.")] private float _curveHeight = 24;

        private TMP_Text _label;

        private void OnValidate()
        {
            var label = GetComponent<TMP_Text>();
            label.havePropertiesChanged = true;
            label.SetVerticesDirty();
        }

        private void OnEnable()
        {
            _label = GetComponent<TMP_Text>();
            _label.OnPreRenderText += Curve;
            _label.havePropertiesChanged = true;
            _label.SetVerticesDirty();
        }

        private void OnDisable()
        {
            if (_label != null)
            {
                _label.OnPreRenderText -= Curve;
                _label.havePropertiesChanged = true;
                _label.SetVerticesDirty();
            }
        }

        private void Curve(TMP_TextInfo text)
        {
            // TMP заново строит плоские вершины перед callback: повторный layout не накапливает изгиб.
            for (var i = 0; i < text.characterCount; i++)
            {
                var character = text.characterInfo[i];

                if (character.isVisible is false)
                    continue;

                var line = text.lineInfo[character.lineNumber];
                var halfWidth = (line.lineExtents.max.x - line.lineExtents.min.x) * 0.5f;

                if (halfWidth <= Mathf.Epsilon)
                    continue;

                var vertices = text.meshInfo[character.materialReferenceIndex].vertices;
                var first = character.vertexIndex;
                var pivot = new Vector3((vertices[first].x + vertices[first + 2].x) * 0.5f, character.baseLine, 0);
                var center = (line.lineExtents.min.x + line.lineExtents.max.x) * 0.5f;
                var position = (pivot.x - center) / halfWidth;
                var offset = _curveHeight * (1 - position * position);
                var slope = -2 * _curveHeight * position / halfWidth;
                var rotation = Quaternion.Euler(0, 0, Mathf.Atan(slope) * Mathf.Rad2Deg);

                for (var j = 0; j < 4; j++)
                    vertices[first + j] = rotation * (vertices[first + j] - pivot) + pivot + new Vector3(0, offset, 0);
            }
        }
    }
}