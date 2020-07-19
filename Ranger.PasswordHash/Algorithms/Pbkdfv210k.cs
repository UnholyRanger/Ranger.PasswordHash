namespace Ranger.PasswordHash
{
	public class Pbkdfv210k : Pbkdfv2Base
	{
		public override string AlgorithmId => @"PBKDFv2-10k";

		public override bool IsObsolete { get; set; } = false;

		protected override int IterationCount => 10000;
	}
}
