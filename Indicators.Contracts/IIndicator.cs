using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators.Contracts
{
    public interface IIndicator
	{
		IndicatorParameters IndicatorParameters { get; }

		List<SimulatorOutputBase> Dependencies { get; }

		void Get(ref object Prototype, IIndicatorValues Ind);

		void GetAll(IIndicatorValues Ind);
	}
}
