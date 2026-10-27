namespace Ranger.PasswordHash
{
    /// <summary>
    /// An interface for configuring the <see cref="IPasswordHash"/> service.
    /// </summary>
    public interface IPasswordHashConfiguration
    {
        /// <summary>
        /// Sets the preferred algorithm to be used by the hashing service.
        /// This will be used when hashing a new password (when no <see cref="PasswordHashRequest.AlgorithmId"/>
        /// is supplied) and for determining when other hashing algorithms are considered obsolete.
        /// </summary>
        /// <typeparam name="TIHashAlgorithm">The algorithm type. It must also be registered.</typeparam>
        /// <returns>The configuration, for chaining.</returns>
        IPasswordHashConfiguration SetPreferredAlgorithm<TIHashAlgorithm>()
            where TIHashAlgorithm : class, IPasswordHashAlgorithm;

        /// <summary>
        /// Misspelled alias of <see cref="SetPreferredAlgorithm{TIHashAlgorithm}"/>, kept for compatibility.
        /// </summary>
        /// <typeparam name="TIHashAlgorithm">The algorithm type. It must also be registered.</typeparam>
        /// <returns>The configuration, for chaining.</returns>
        [System.Obsolete("Use SetPreferredAlgorithm instead. This alias will be removed in a future version.")]
        IPasswordHashConfiguration SetPrefferedAlgorithm<TIHashAlgorithm>()
            where TIHashAlgorithm : class, IPasswordHashAlgorithm
            => SetPreferredAlgorithm<TIHashAlgorithm>();

        /// <summary>
        /// Prevents the loading of any hashing algorithms bundled
        /// within this library and will only use externally configured
        /// algorithms.
        /// </summary>
        /// <returns>The configuration, for chaining.</returns>
        IPasswordHashConfiguration UseOnlyExternalAlgorithms();

        /// <summary>
        /// Registers the algorithm for use with the hashing service.
        /// </summary>
        /// <typeparam name="TIHashAlgorithm">The algorithm type to register.</typeparam>
        /// <returns>The configuration, for chaining.</returns>
        IPasswordHashConfiguration RegisterAlgorithm<TIHashAlgorithm>()
            where TIHashAlgorithm : class, IPasswordHashAlgorithm;
    }
}
