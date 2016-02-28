using SteveBagnall.Trading.Indicators.Values.Contracts;
using System.Collections.Generic;


namespace SteveBagnall.Trading.Indicators.Contracts
{
    public interface IBarrierIndicator : IIndicator
	{
		List<IBarrierValue> GetBarriers(int PeriodsAgo, IIndicatorValues Ind);
	}
}
