using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;
using System.Diagnostics;
using System.Reflection;

namespace SteveBagnall.Trading.Indicators.Values
{
    /// <summary>
    /// Synonymous with TrendLine, or PriceLevel
    /// </summary>
    [Serializable()]
	public class PriceRangeBarrierProperty : IBarrierValue
	{
		public int BarrierUniqueKey
		{
			get
			{
				int uniqueKey = 17;
				uniqueKey = uniqueKey * 23 + this.PriceRangeValue.GetType().GetHashCode();
				uniqueKey = uniqueKey * 23 + this.Property.Name.GetHashCode();
				return uniqueKey;
			}
		}

		public Type Type { get { return this.PriceRangeValue.GetType(); } }

		public BarrierDirection BarrierDirection { get { return BarrierDirection.FlatAndBouncable; } }

		
		public PriceRangeBarrierPropertiesBase PriceRangeValue { get; private set; }
		
		public PropertyInfo Property { get; private set; }


		/// <summary>
		/// Value needs to be set very time, and updated with the latest values
		/// </summary>
		public decimal ProximateDistance { get; set; }

		public decimal CongestionDistance { get; set; }


		public decimal FailureDistanceMultiple { get; private set; }


		public bool IsCongested 
		{ 
			get { return false; } 
			set { throw new ApplicationException("Cannot set congestion on Price Range property barriers."); } 
		}

		public CongestionBoundary CongestedBoundary 
		{ 
			get { return CongestionBoundary.NotSet; } 
			set { throw new ApplicationException("Cannot set congestion on Price Range property barriers."); } 
		}


		public PriceRangeBarrierProperty(
			PriceRangeBarrierPropertiesBase PriceRangeValue,
			PropertyInfo Property, 
			decimal FailureDistanceMultiple)
		{
			this.PriceRangeValue = PriceRangeValue;
			this.Property = Property;
			this.FailureDistanceMultiple = FailureDistanceMultiple;
		}

		public Decimal GetValueAt(int BarNumber)
		{
			return (decimal)this.Property.GetGetMethod(false).Invoke(this.PriceRangeValue, new object[] { });
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
		public bool Equals(PriceRangeBarrierProperty p)
		{
			if ((object)p == null)
				return false;

			return ((this.PriceRangeValue == p.PriceRangeValue)
				&& (this.Property.Name == p.Property.Name));
		}

		[DebuggerStepThrough()]
		public override bool Equals(object obj)
		{
			if (!(obj is PriceRangeBarrierProperty))
				return false;
			else
				return Equals(obj as PriceRangeBarrierProperty);
		}

		[DebuggerStepThrough()]
		public override int GetHashCode()
		{
			return this.BarrierUniqueKey;
		}

		[DebuggerStepThrough()]
		public static bool operator ==(PriceRangeBarrierProperty lhs, PriceRangeBarrierProperty rhs)
		{
			if (System.Object.ReferenceEquals(lhs, rhs))
				return true;

			if (((object)lhs == null) || ((object)rhs == null))
				return false;

			return lhs.Equals(rhs);
		}

		[DebuggerStepThrough()]
		public static bool operator !=(PriceRangeBarrierProperty lhs, PriceRangeBarrierProperty rhs)
		{
			return !(lhs == rhs);
		}
	}
}
