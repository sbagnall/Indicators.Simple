using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;
using System.Diagnostics;

namespace SteveBagnall.Trading.Indicators.Values.AggregatedLines
{
    [Serializable()]
	public class TrendLine : IAggregateLine, ITrendLine
	{
		public int BarrierUniqueKey
		{
			get
			{
				int uniqueKey = 17;
				uniqueKey = uniqueKey * 23 + this.Gradient.GetHashCode();
				uniqueKey = uniqueKey * 23 + this.Intercept.GetHashCode();
				return uniqueKey;
			}
		}

		public Type Type { get { return typeof(TrendLine); } }

		public BarrierDirection BarrierDirection
		{
			get
			{
				return (Math.Abs(this.Gradient) < this.AbsoluteFlatThreshold) ? BarrierDirection.FlatAndBouncable : ((this.Gradient > 0.0M) ? BarrierDirection.UpTrend : BarrierDirection.DownTrend);
			}
		}



		public int BarNumberIdentified { get; set; }

		public Decimal Intercept { get; private set; }

		public Decimal Gradient { get; private set; }

		public TrendType TrendType { get; private set; }

		public TrendOrientation Orientation { get; private set; }


		public int NumTests { get; set; }

		/// <summary>
		/// Value needs to be set very time, and updated with the latest values
		/// </summary>
		public decimal VerticalDistanceThreshold { get; set; }

		/// <summary>
		/// Value needs to be set very time, and updated with the latest values
		/// </summary>
		public decimal ProximateDistance { get; set; }

		/// <summary>
		/// Value needs to be set very time, and updated with the latest values
		/// </summary>
		public decimal CongestionDistance { get; set; }


		public decimal AbsoluteGradientThreshold { get; private set; }
		public decimal AbsoluteFlatThreshold { get; private set; }
		
		public decimal FailureDistanceMultiple { get; private set; }
				
		public bool IsCongested { get; set; }

		public CongestionBoundary CongestedBoundary { get; set;  }
		

		public TrendLine(
			int BarNumberIdentified,
			Decimal Intercept,
			Decimal Gradient,
			TrendType TrendType,
			TrendOrientation Orientation,
			int NumTests,
			decimal AbsoluteGradientThreshold,
			decimal AbsoluteFlatThreshold,
			decimal FailureDistanceMultiple)
		{
			this.BarNumberIdentified = BarNumberIdentified;
			this.Intercept = Intercept;
			this.Gradient = Gradient;
			this.TrendType = TrendType;
			this.Orientation = Orientation;
			this.NumTests = NumTests;
			this.AbsoluteGradientThreshold = AbsoluteGradientThreshold;
			this.AbsoluteFlatThreshold = AbsoluteFlatThreshold;
			this.FailureDistanceMultiple = FailureDistanceMultiple;
		}

		public bool IsWithinThreshold(int BarNumber, IAggregateLine CandidateLine)
		{
			decimal candidateVerticalDistanceAway = (decimal)Math.Abs((double)(this.GetValueAt(BarNumber) - CandidateLine.GetValueAt(BarNumber)));

			if ((candidateVerticalDistanceAway <= this.VerticalDistanceThreshold)
				&& ((decimal)Math.Abs((double)(this.Gradient - (CandidateLine as TrendLine).Gradient)) <= this.AbsoluteGradientThreshold))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		

		public Decimal GetValueAt(int BarNumber)
		{
			return this.Intercept + (this.Gradient * BarNumber);
		}

		public decimal GetProximityUpperBoundaryAt(int BarNumber)
		{
			return GetValueAt(BarNumber) + this.ProximateDistance;
		}

		public decimal GetProximityLowerBoundaryAt(int BarNumber)
		{
			return GetValueAt(BarNumber) - this.ProximateDistance;
		}

		public decimal GetFailureUpperBoundaryAt(int BarNumber)
		{
			return GetValueAt(BarNumber) + (this.ProximateDistance * this.FailureDistanceMultiple);
		}

		public decimal GetFailureLowerBoundaryAt(int BarNumber)
		{
			return GetValueAt(BarNumber) - (this.ProximateDistance * this.FailureDistanceMultiple);
		}

		[DebuggerStepThrough()]
		public bool Equals(TrendLine t)
		{
			if ((object)t == null)
				return false;

			return ((this.Gradient == t.Gradient) && (this.Intercept == t.Intercept));
		}

		[DebuggerStepThrough()]
		public override bool Equals(object obj)
		{
			if (!(obj is TrendLine))
				return false;
			else
				return Equals(obj as TrendLine);
		}

		[DebuggerStepThrough()]
		public override int GetHashCode()
		{
			return this.BarrierUniqueKey;
		}

		[DebuggerStepThrough()]
		public static bool operator ==(TrendLine lhs, TrendLine rhs)
		{
			if (System.Object.ReferenceEquals(lhs, rhs))
				return true;

			if (((object)lhs == null) || ((object)rhs == null))
				return false;

			return lhs.Equals(rhs);
		}

		[DebuggerStepThrough()]
		public static bool operator !=(TrendLine lhs, TrendLine rhs)
		{
			return !(lhs == rhs);
		}
	}
}
