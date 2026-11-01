using System;
using Newtonsoft.Json;

namespace Project.Data.Remote.Serialization
{
    /// <summary>
    /// Читает строковые поля внешнего ответа с проверкой типа JSON-токена.
    /// </summary>
    internal sealed class StrictStringConverter : JsonConverter<string>
    {
        public override bool CanWrite => false;

        /// <summary>
        /// Предоставляет Newtonsoft.Json конструктор конвертера из атрибута DTO.
        /// </summary>
        public StrictStringConverter() { }

        /// <summary>
        /// Читает строку или null; обязательность значения проверяет владелец DTO.
        /// </summary>
        public override string ReadJson(
            JsonReader reader,
            Type objectType,
            string existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException($"Поле {reader.Path} должно быть строкой.");

            return (string)reader.Value;
        }

        /// <summary>
        /// Отклоняет запись JSON: конвертер предназначен для чтения DTO ответа.
        /// </summary>
        /// <exception cref="NotSupportedException">Запись этим конвертером не поддерживается.</exception>
        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            throw new NotSupportedException("Конвертер предназначен только для чтения.");
        }
    }
}