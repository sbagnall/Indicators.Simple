using SteveBagnall.Trading.Indicators.Values.AggregatedLines;
using SteveBagnall.Trading.Indicators.Values.Contracts;

namespace SteveBagnall.Trading.Indicators.Contracts
{
    public interface IDivergentIndicator : IIndicator
    {
        TrendLine GetPeakToPeakLine(int PeriodsAgo, IIndicatorValues Ind);

        TrendLine GetDipToDipLine(int PeriodsAgo, IIndicatorValues Ind);
    }
}
