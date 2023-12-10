namespace Ranger.PasswordHash.Tests.TestAlgorithms
{
    internal class PreferredAlgorithm : IPasswordHashAlgorithm
    {
        public string Id => "Preferred-Algorithm";

        public PasswordHashResult Hash(PasswordHashRequest request)
            => new()
            {
                AlgorithmId = Id,
                Value = nameof(PreferredAlgorithm)
            };
    }
}
