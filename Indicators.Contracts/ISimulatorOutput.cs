using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators.Contracts
{
    public interface ISimulatorOutput
	{
		string GetHeader();

		String GetDetail(IOutputInstant Instant, params IIndicatorValues[] Data);

		void Initialise(List<IOutputInstant> AllInstants);
	}
}
