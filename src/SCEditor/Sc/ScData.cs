using System;

namespace SCEditor.Sc
{
    /// <summary>
    /// Represents a piece of data inside of <see cref="ScFile"/>.
    /// </summary>
    public abstract class ScData
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ScData"/> class with the specified <see cref="ScFile"/>
        /// which contains the <see cref="ScData"/>.
        /// </summary>
        /// <param name="scFile"><see cref="ScFile"/> which contains the <see cref="ScData"/>.</param>
        /// <exception cref="ArgumentNullException"/>
        public ScData(ScFile scFile)
        {
            _scFile = scFile ?? throw new ArgumentNullException(nameof(scFile));
        }
        #endregion

        #region Fields & Properties
        private readonly ScFile _scFile;

        /// <summary>
        /// Gets the <see cref="ScFile"/> which contains this <see cref="ScData"/>.
        /// </summary>
        public ScFile Parent => _scFile;
        #endregion
    }
}
