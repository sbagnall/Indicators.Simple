using SteveBagnall.Trading.Indicators.Values.Contracts;

namespace SteveBagnall.Trading.Indicators.Contracts
{
    public interface ITrendDirectionIndicator : IIndicator
	{
		ITrendDirectionValue GetTrendDirectionValue(int PeriodsAgo, IIndicatorValues Ind);
	}
}
