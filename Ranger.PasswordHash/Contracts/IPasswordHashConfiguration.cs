namespace Ranger.PasswordHash
{
    /// <summary>
    /// An interfaces for configuring the <see cref="IPasswordHash"/> service.
    /// </summary>
    public interface IPasswordHashConfiguration
    {
        /// <summary>
        /// Sets the preffered algorithm to be used by the hashing service.
        /// This will be used when hashing a new password (when no <see cref="PasswordHashRequest.AlgorithmId"/>
        /// is supplied) and for determining when other hashing algorithms are considered obsolete.
        /// </summary>
        /// <typeparam name="TIHashAlgorithm"></typeparam>
        /// <returns></returns>
        IPasswordHashConfiguration SetPrefferedAlgorithm<TIHashAlgorithm>()
            where TIHashAlgorithm : class, IPasswordHashAlgorithm;

        /// <summary>
        /// Prevents the loading of any hashing algorithms bundled
        /// within this library and will only use externally configured
        /// algorithms.
        /// </summary>
        /// <returns></returns>
        IPasswordHashConfiguration UseOnlyExternalAlgorithms();

        /// <summary>
        /// Registers the algorithm for use with the hasing service.
        /// </summary>
        /// <typeparam name="TIHashAlgorithm"></typeparam>
        /// <returns></returns>
        IPasswordHashConfiguration RegisterAlgorithm<TIHashAlgorithm>()
            where TIHashAlgorithm : class, IPasswordHashAlgorithm;
    }
}
