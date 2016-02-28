using SteveBagnall.Trading.Shared;

namespace SteveBagnall.Trading.Indicators.Values.Contracts
{
    /// <summary>
    /// An indicator value that measures whether a price is trending
    /// </summary>
    public interface ITrendinessValue
	{
		Trendiness Trendiness { get; }
	}
}
