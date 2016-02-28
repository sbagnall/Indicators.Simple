using SteveBagnall.Trading.Indicators.Values.Shared;

namespace SteveBagnall.Trading.Indicators.Values.Contracts
{
    /// <summary>
    /// An indicator value that measures position in a range
    /// </summary>
    public interface IOscillatingValue
	{
		Oscillation Oscillation { get; }
	}
}
