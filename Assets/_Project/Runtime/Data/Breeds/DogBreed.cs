namespace Project.Data.Breeds
{
    /// <summary>
    /// Хранит идентификатор, название и описание породы.
    /// </summary>
    public sealed class DogBreed
    {
        /// <summary>
        /// Идентификатор источника для запроса описания породы.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Название породы, полученное от источника.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Описание породы; в ответе списка может отсутствовать.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Сохраняет идентификатор и отображаемые данные породы.
        /// </summary>
        public DogBreed(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}