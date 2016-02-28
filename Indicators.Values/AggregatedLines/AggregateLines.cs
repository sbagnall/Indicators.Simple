using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System.Collections.Generic;
using System.Linq;

namespace SteveBagnall.Trading.Indicators.Values.AggregatedLines
{
    public class AggregateLines : BarrierPrototypeBase
	{
		public List<IAggregateLine> Lines { get; set; }

		public AggregateLines()
			: this(new List<IAggregateLine>())
		{ }

		public AggregateLines(List<IAggregateLine> Lines)
		{
			this.Lines = Lines;
		}

		public override object Clone()
		{
			return new AggregateLines((List<IAggregateLine>)Utilities.DeepClone(this.Lines));
		}

		public override List<IBarrierValue> GetBarriers()
		{
			return this.Lines.Cast<IBarrierValue>().ToList();
		}

		public List<IBarrierValue> GetClosestBarrierAbove(
			StudyValue StudyValue, 
			int BarNumber,
			bool IsTrendTypeTest,
			TrendType TrendType,
			bool IsBoundaryTest, 
			CongestionBoundary CongestionBoundary,
			int MinimumTests)
		{
			List<IBarrierValue> barriers = new List<IBarrierValue>();

			foreach (IBarrierValue barrier in base.GetClosestBarrierAbove(StudyValue, BarNumber, IsTrendTypeTest, TrendType, IsBoundaryTest, CongestionBoundary))
				if ((barrier as IAggregateLine).NumTests >= MinimumTests)
					barriers.Add(barrier);

			return barriers;
		}

		public List<IBarrierValue> GetClosestBarrierBelow(
			StudyValue StudyValue, 
			int BarNumber,
			bool IsTrendTypeTest,
			TrendType TrendType,
			bool IsBoundaryTest, 
			CongestionBoundary CongestionBoundary,
			int MinimumTests)
		{
			List<IBarrierValue> barriers = new List<IBarrierValue>();

			foreach (IBarrierValue barrier in base.GetClosestBarrierBelow(StudyValue, BarNumber, IsTrendTypeTest, TrendType, IsBoundaryTest, CongestionBoundary))
				if ((barrier as IAggregateLine).NumTests >= MinimumTests)
					barriers.Add(barrier);

			return barriers;
		}
	}
}
