namespace Ranger.PasswordHash.Algorithms
{
    /// <summary>
    /// A hashing function for the PBKDF V2 with 610,000 iterations.
    /// </summary>
    public class PbkdfV2610k : Pbkdfv2Base
    {
        /// <inheritdoc/>
        public override string Id => @"PBKDFv2-610k";

        /// <inheritdoc/>
        internal override int IterationCount => 610000;
    }
}
