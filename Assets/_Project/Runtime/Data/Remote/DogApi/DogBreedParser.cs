using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Project.Data.Breeds;
using Project.Data.Remote.Serialization;

namespace Project.Data.Remote.DogApi
{
    /// <summary>
    /// Проверяет ответы JSON:API Dog API и преобразует ресурсы сервера в модели пород.
    /// </summary>
    internal static class DogBreedParser
    {
        private static readonly JsonSerializerSettings _settings = new()
        {
            DateParseHandling = DateParseHandling.None,
            TypeNameHandling = TypeNameHandling.None,
            CheckAdditionalContent = true
        };

        /// <summary>
        /// Проверяет и преобразует все записи страницы, сохраняя серверный порядок.
        /// </summary>
        internal static IReadOnlyList<DogBreed> ParseList(string json)
        {
            var response = JsonConvert.DeserializeObject<Response<Resource[]>>(json, _settings);

            if (response?.Data is null)
                throw new FormatException("Ответ не содержит списка пород.");

            var breeds = new List<DogBreed>(response.Data.Length);

            foreach (var resource in response.Data)
                breeds.Add(Convert(resource, false));

            return breeds.AsReadOnly();
        }

        /// <summary>
        /// Проверяет наличие описания в ответе JSON:API для отдельной породы.
        /// </summary>
        internal static DogBreed ParseDetails(string json)
        {
            return Convert(JsonConvert.DeserializeObject<Response<Resource>>(json, _settings)?.Data, true);
        }

        private static DogBreed Convert(Resource resource, bool requireDescription)
        {
            if (string.IsNullOrWhiteSpace(resource?.Id) || string.IsNullOrWhiteSpace(resource.Attributes?.Name)
                || (requireDescription && string.IsNullOrWhiteSpace(resource.Attributes.Description)))
                throw new FormatException("Ответ содержит неполные данные породы.");

            return new(resource.Id, resource.Attributes.Name, resource.Attributes.Description ?? string.Empty);
        }

        /// <summary>
        /// Описывает контейнер JSON:API со списком или отдельным ресурсом.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        private sealed class Response<T>
        {
            [JsonProperty("data")]
            internal T Data { get; private set; }
        }

        /// <summary>
        /// Содержит серверный идентификатор и атрибуты породы.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        private sealed class Resource
        {
            [JsonProperty("id"), JsonConverter(typeof(StrictStringConverter))]
            internal string Id { get; private set; }

            [JsonProperty("attributes")]
            internal Attributes Attributes { get; private set; }
        }

        /// <summary>
        /// Описывает отображаемые атрибуты Dog API.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        private sealed class Attributes
        {
            [JsonProperty("name"), JsonConverter(typeof(StrictStringConverter))]
            internal string Name { get; private set; }

            [JsonProperty("description"), JsonConverter(typeof(StrictStringConverter))]
            internal string Description { get; private set; }
        }
    }
}