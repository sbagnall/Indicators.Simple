using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Contracts
{
    public interface IStudyIndicator : IIndicator
	{
		StudyValue GetStudyValue(int PeriodsAgo, IIndicatorValues Ind);

		StudyValue GetStudyValue(DateTime Date, IIndicatorValues Ind);
	}
}
