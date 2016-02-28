using SteveBagnall.Trading.Indicators.Values.Shared;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators.Values.Contracts
{
    public interface IBarrierPrototype
    {
        List<IBarrierValue> GetBarriers();
        List<IBarrierValue> GetClosestBarrierAbove(StudyValue StudyValue, int BarNumber, bool IsTrendTypeTest, TrendType TrendType, bool IsBoundaryTest, CongestionBoundary CongestionBoundary);
        List<IBarrierValue> GetClosestBarrierBelow(StudyValue StudyValue, int BarNumber, bool IsTrendTypeTest, TrendType TrendType, bool IsBoundaryTest, CongestionBoundary CongestionBoundary);
    }
}