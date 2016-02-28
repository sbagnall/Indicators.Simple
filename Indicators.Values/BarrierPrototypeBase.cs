using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SteveBagnall.Trading.Indicators.Values
{
    [Serializable()]
	public abstract class BarrierPrototypeBase : ICloneable, IBarrierPrototype
    {
		public BarrierPrototypeBase()
		{ }

		public abstract object Clone();

		public abstract List<IBarrierValue> GetBarriers();

		public virtual List<IBarrierValue> GetClosestBarrierAbove(
			StudyValue StudyValue, 
			int BarNumber, 
			bool IsTrendTypeTest,
			TrendType TrendType,
			bool IsBoundaryTest, 
			CongestionBoundary CongestionBoundary)
		{
			List<IBarrierValue> barriers = new List<IBarrierValue>();

			foreach (IBarrierValue barrier in
				GetBarrierDistances(StudyValue, BarNumber, IsTrendTypeTest, TrendType, IsBoundaryTest, CongestionBoundary).Where(t => t.Value >= 0.0M).OrderBy(t => t.Value).Select(t => t.Key))
			{
				barriers.Add(barrier);
			}

			return barriers;
		}

		public virtual List<IBarrierValue> GetClosestBarrierBelow(
			StudyValue StudyValue, 
			int BarNumber,
			bool IsTrendTypeTest,
			TrendType TrendType,
			bool IsBoundaryTest, 
			CongestionBoundary CongestionBoundary)
		{
			List<IBarrierValue> barriers = new List<IBarrierValue>();

			foreach (IBarrierValue barrier in
				GetBarrierDistances(StudyValue, BarNumber, IsTrendTypeTest, TrendType, IsBoundaryTest, CongestionBoundary).Where(t => t.Value < 0.0M).OrderBy(t => t.Value).Reverse().Select(t => t.Key))
			{
				barriers.Add(barrier);
			}

			return barriers;
		}

		private Dictionary<IBarrierValue, Decimal> GetBarrierDistances(
			StudyValue StudyValue, 
			int BarNumber,
			bool IsTrendTypeTest,
			TrendType TrendType,
			bool IsBoundaryTest, 
			CongestionBoundary CongestionBoundary)
		{
			Dictionary<IBarrierValue, Decimal> distances = new Dictionary<IBarrierValue, Decimal>();
			foreach (IBarrierValue barrier in this.GetBarriers())
			{
				if ((!IsTrendTypeTest) || ((barrier is ITrendLine) && ((barrier as ITrendLine).TrendType == TrendType)))
				{
					if ((!IsBoundaryTest) || (CongestionBoundary == barrier.CongestedBoundary))
					{
						// TODO: should not have to check this, but some double zero lines are coming through twice
						if (!distances.ContainsKey(barrier))
							distances.Add(barrier, barrier.GetValueAt(BarNumber) - StudyValue.Value);
					}
				}
			}

			return distances;
		}
	}
}
