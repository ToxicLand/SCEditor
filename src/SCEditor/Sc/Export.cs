namespace SCEditor.Sc
{
    /// <summary>
    /// Represents an export.
    /// </summary>
    public class Export : ScData
    {
        #region Constructors
        public Export(ScFile scFile) : base(scFile)
        {
            // Space
        }
        #endregion

        #region Fields & Properties
        internal int _id;
        internal string _name;

        /// <summary>
        /// Gets the ID of the <see cref="Export"/>.
        /// </summary>
        public int Id => _id;

        /// <summary>
        /// Gets the name of the <see cref="Export"/>.
        /// </summary>
        public string Name => _name;
        #endregion
    }
}
