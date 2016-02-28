using SteveBagnall.Trading.Indicators.Contracts;

namespace SteveBagnall.Trading.Indicators.Contracts
{
    public interface IRenkoStudyIndicator : IStudyIndicator
    {
        decimal GetRange(IIndicatorValues Ind);
    }
}
