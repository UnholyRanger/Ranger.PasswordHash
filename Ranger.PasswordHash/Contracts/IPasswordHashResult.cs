﻿namespace Ranger.PasswordHash
{
    /// <summary>
    /// An interface for working with the result of a password hash.
    /// </summary>
    public interface IPasswordHashResult
    {
        /// <summary>
        /// Gets the Id of the underlying <see cref="IPasswordHashAlgorithm"/> that was
        /// used during the hash request.
        /// </summary>
        string AlgorithmId { get; }

        /// <summary>
        /// Gets the resulting hash value.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// Gets the salt used for the hash. This will be equivalend to the <see cref="PasswordHashRequest.Salt"/> or
        /// one generated by the <see cref="IPasswordSaltGenerator"/>.
        /// </summary>
        Salt Salt { get; }

        /// <summary>
        /// Gets a value indicating whether the underlying <see cref="IPasswordHashAlgorithm"/>
        /// that was used during the hashing should be upgraded to a newer hash algorithm.
        /// </summary>
        bool IsObsolete { get; }
    }
}