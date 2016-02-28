using SteveBagnall.Trading.Indicators.Values.Contracts;

namespace SteveBagnall.Trading.Indicators.Contracts
{
    public interface IMovingAveragePairIndicator : IIndicator
	{
		IMovingAveragePairValue GetMAPairValue(int PeriodsAgo, IIndicatorValues Ind);
	}
}
