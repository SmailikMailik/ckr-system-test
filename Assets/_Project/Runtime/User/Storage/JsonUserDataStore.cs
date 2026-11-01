using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Common.Validation;

namespace Project.User.Storage
{
    /// <summary>
    /// Хранит снимок пользователя в версионированном JSON с заменой целого файла после успешной записи.
    /// </summary>
    public sealed class JsonUserDataStore : IUserDataStore
    {
        private const int SchemaVersion = 1;
        private readonly string _path;

        /// <summary>
        /// Проверяет и сохраняет полный путь к файлу пользовательских данных.
        /// </summary>
        public JsonUserDataStore(string path)
        {
            ArgumentGuard.ThrowIfBlank(path, nameof(path));

            _path = Path.GetFullPath(path);
        }

        /// <summary>
        /// Возвращает проверенный снимок или null только при отсутствии файла; ошибки чтения передаются
        /// вызывающему коду.
        /// </summary>
        public UserSnapshot Load()
        {
            string json;

            try
            {
                json = File.ReadAllText(_path, Encoding.UTF8);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }

            try
            {
                var document = JObject.Parse(json, new JsonLoadSettings
                {
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error
                });
                var version = ReadNonNegativeInt(document, "schemaVersion");

                if (version != SchemaVersion)
                    throw new FormatException("Версия сохранения пользователя не поддерживается.");

                return new(ReadNonNegativeInt(document, "currency"), ReadNonNegativeInt(document, "energy"));
            }
            catch (JsonException exception)
            {
                throw new FormatException("Сохранение пользователя содержит неверный JSON.", exception);
            }
        }

        /// <summary>
        /// Записывает временный файл рядом с основным и заменяет сохранение только после полного flush.
        /// </summary>
        public void Save(UserSnapshot snapshot)
        {
            ArgumentGuard.ThrowIfNull(snapshot, nameof(snapshot));

            var directory = Path.GetDirectoryName(_path);
            Directory.CreateDirectory(directory);
            var temporaryPath = Path.Combine(directory, $".{Path.GetFileName(_path)}.{Guid.NewGuid():N}.tmp");

            try
            {
                var document = new JObject
                {
                    ["schemaVersion"] = SchemaVersion,
                    ["currency"] = snapshot.Currency,
                    ["energy"] = snapshot.Energy
                };
                var bytes = Encoding.UTF8.GetBytes(document.ToString(Formatting.None));

                using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                }

                if (File.Exists(_path))
                    File.Replace(temporaryPath, _path, null);
                else
                    File.Move(temporaryPath, _path);
            }
            finally
            {
                DeleteTemporaryFile(temporaryPath);
            }
        }

        private static int ReadNonNegativeInt(JObject document, string name)
        {
            var token = document[name];

            if (token?.Type != JTokenType.Integer
                || int.TryParse(token.ToString(Formatting.None), NumberStyles.None,
                    CultureInfo.InvariantCulture, out var value) is false)
                throw new FormatException($"Поле {name} должно содержать неотрицательное целое число int.");

            return value;
        }

        private static void DeleteTemporaryFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (IOException)
            {
                // Ошибка очистки не должна скрывать исходную ошибку сохранения.
            }
            catch (UnauthorizedAccessException)
            {
                // Ошибка очистки не должна скрывать исходную ошибку сохранения.
            }
        }
    }
}