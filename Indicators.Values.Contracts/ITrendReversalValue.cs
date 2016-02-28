using SteveBagnall.Trading.Shared;

namespace SteveBagnall.Trading.Indicators.Values.Contracts
{
    /// <summary>
    /// An indicator value that measures whether a price is about to reverse
    /// </summary>
    public interface ITrendReversalValue
	{
		Direction Direction { get; }
	}
}
