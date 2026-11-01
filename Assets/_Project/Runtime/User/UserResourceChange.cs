namespace Project.User
{
    /// <summary>
    /// Описывает изменение одного ресурса; несколько изменений составляют согласованную операцию сессии.
    /// </summary>
    public readonly struct UserResourceChange
    {
        internal IUserResource Resource { get; }
        internal int Delta { get; }

        /// <summary>
        /// Связывает ресурс с величиной его изменения для согласованной операции сессии.
        /// </summary>
        internal UserResourceChange(IUserResource resource, int delta)
        {
            Resource = resource;
            Delta = delta;
        }
    }
}