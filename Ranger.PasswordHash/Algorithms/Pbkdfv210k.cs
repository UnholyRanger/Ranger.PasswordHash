namespace Ranger.PasswordHash.Algorithms
{
    /// <summary>
    /// A hashing function for the PBKDF V2 with 10,000 iterations.
    /// </summary>
    public class Pbkdfv210k : Pbkdfv2Base
    {
        /// <inheritdoc/>
        public override string Id => @"PBKDFv2-10k";

        /// <inheritdoc/>
        internal override int IterationCount => 10000;
    }
}
