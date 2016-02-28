namespace SteveBagnall.Trading.Indicators.Values.Contracts
{
    public interface IAggregateLine : IBarrierValue
	{
		int BarNumberIdentified { get; set;  }

		int NumTests { get; set; }

		bool IsWithinThreshold(int BarNumber, IAggregateLine CandidateLine);
	}
}
