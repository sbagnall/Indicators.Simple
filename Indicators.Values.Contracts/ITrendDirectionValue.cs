using SteveBagnall.Trading.Shared;

namespace SteveBagnall.Trading.Indicators.Values.Contracts
{
    /// <summary>
    /// An indicator value that measures trend direction
    /// </summary>
    public interface ITrendDirectionValue
	{
		Direction Direction { get; }
	}
}
