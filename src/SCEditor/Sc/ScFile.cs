using System;
using System.IO;

namespace SCEditor.Sc
{
    /// <summary>
    /// Represents a .sc file.
    /// </summary>
    public class ScFile
    {
        #region Constants
        /// <summary>
        /// Latest version of <see cref="ScFile"/> supported. This field is constant.
        /// </summary>
        public const ScFormatVersion LatestSupportedVersion = ScFormatVersion.Version7;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ScFile"/> class.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        public ScFile(ScFormatVersion version)
        {
            if (version < ScFormatVersion.Version7 || version > ScFormatVersion.Version8)
                throw new ArgumentException();

            _version = version;
        }
        #endregion

        #region Fields & Properties
        private readonly ScFormatVersion _version;

        /// <summary>
        /// Gets the <see cref="ScFormatVersion"/> of this <see cref="ScFile"/>.
        /// </summary>
        public ScFormatVersion Version => _version;
        #endregion

        #region Methods
        /// <summary>
        /// Saves the <see cref="ScFile"/> at the specified path.
        /// </summary>
        /// <param name="path">Path at which to save the <see cref="ScFile"/>.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NotImplementedException"/>
        public void Save(string path)
        {
            //TODO: Implement.
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the .sc(<see cref="ScFile"/>) file at the specified path.
        /// </summary>
        /// <param name="path">Path pointing to the .sc file.</param>
        /// <returns>An instance of the <see cref="ScFile"/> representing the specified .sc file.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FileNotFoundException"/>
        public static ScFile Load(string path)
        {
            return Load(path, LatestSupportedVersion);
        }

        /// <summary>
        /// Loads the .sc(<see cref="ScFile"/>) file at the specified path.
        /// </summary>
        /// <param name="path">Path pointing to the .sc file.</param>
        /// <returns>An instance of the <see cref="ScFile"/> representing the specified .sc file.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FileNotFoundException"/>
        public static ScFile Load(string path, ScFormatVersion version)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path))
                throw new FileNotFoundException();

            var scFile = new ScFile(version);
            var loader = ScLoader.GetLoader(version);
            using (var fileStream = File.OpenRead(path))
                loader.Load(ref scFile, fileStream);
            
            return scFile;
        }
        #endregion
    }
}
