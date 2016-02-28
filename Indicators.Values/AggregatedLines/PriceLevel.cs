using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;
using System.Diagnostics;

namespace SteveBagnall.Trading.Indicators.Values.AggregatedLines
{
    [Serializable()]
	public class PriceLevel : IAggregateLine
	{
		public int BarrierUniqueKey
		{
			get
			{
				int uniqueKey = 17;
				uniqueKey = uniqueKey * 23 + this.Price.GetHashCode();
				uniqueKey = uniqueKey * 23 + this.IsDoubleZero.GetHashCode();
				return uniqueKey;
			}
		}

		public Type Type { get { return typeof(PriceLevel); } }

		public BarrierDirection BarrierDirection { get { return BarrierDirection.FlatAndBouncable; } }


		public int BarNumberIdentified { get; set; }

		public Decimal Price { get; private set; }

		public bool IsDoubleZero { get; private set; }
				
		
		public int NumTests { get; set; }

		/// <summary>
		/// Value needs to be set very time, and updated with the latest values
		/// </summary>
		public decimal VerticalThreshold { get; set; }

		/// <summary>
		/// Value needs to be set very time, and updated with the latest values
		/// </summary>
		public decimal ProximateDistance { get; set; }

		public decimal CongestionDistance { get; set; }


		public decimal FailureDistanceMultiple { get; private set; }

		
		public bool IsCongested { get; set; }

		public CongestionBoundary CongestedBoundary { get; set; }


		public PriceLevel(
			int BarNumberIdentified,
			decimal Price, 
			bool IsDoubleZero, 
			int NumTests, 
			decimal FailureDistanceMultiple)
		{
			this.BarNumberIdentified = BarNumberIdentified;
			this.Price = Price;
			this.IsDoubleZero = IsDoubleZero;
			this.NumTests = NumTests;
			this.FailureDistanceMultiple = FailureDistanceMultiple;
		}

		public bool IsWithinThreshold(int BarNumber, IAggregateLine CandidateLine)
		{
			return ((Decimal)Math.Abs(this.GetValueAt(BarNumber) - CandidateLine.GetValueAt(BarNumber)) <= this.VerticalThreshold);
		}
				
		public Decimal GetValueAt(int BarNumber)
		{
			return this.Price;
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
		public bool Equals(PriceLevel p)
		{
			if ((object)p == null)
				return false;

			return ((this.Price == p.Price));
		}

		[DebuggerStepThrough()]
		public override bool Equals(object obj)
		{
			if (!(obj is PriceLevel))
				return false;
			else
				return Equals(obj as PriceLevel);
		}

		[DebuggerStepThrough()]
		public override int GetHashCode()
		{
			return this.BarrierUniqueKey;
		}

		[DebuggerStepThrough()]
		public static bool operator ==(PriceLevel lhs, PriceLevel rhs)
		{
			if (System.Object.ReferenceEquals(lhs, rhs))
				return true;

			if (((object)lhs == null) || ((object)rhs == null))
				return false;

			return lhs.Equals(rhs);
		}

		[DebuggerStepThrough()]
		public static bool operator !=(PriceLevel lhs, PriceLevel rhs)
		{
			return !(lhs == rhs);
		}
	}
}
