using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace SteveBagnall.Trading.Indicators.Values
{
	/// <summary>
	/// An indicator value that is plotted continuously on the same scale as the price
	/// Must implement the property attribute PriceRangeValueAttribute on one or more properties whose values will be used to measure interaction with the price range
	/// Synonymous with AggregateLines (except it's abstract)
	/// </summary>
	[Serializable()]
	public abstract class PriceRangeBarrierPropertiesBase : BarrierPrototypeBase, IParametized
	{
		#region Parameters

		private static String FAILURE_DISTANCE_MULTIPLE = "PRICERANGEBARRIERPROPERTIESBASE_FAILURE_DISTANCE_MULTIPLE";

		public virtual Parameters Parameters
		{
			get
			{
				Parameters parameters = new Parameters();

				parameters.Add(new Parameter(
					
					FAILURE_DISTANCE_MULTIPLE,
					typeof(decimal),
					2.5M,
					false, // IsTesting
					1.5M,
					3.0M,
					TestSequence.Arithmatic));

				return parameters;
			}
		}

		#endregion

		//public decimal FailureDistanceMultiple { get { return Convert.ToDecimal(ParameterEngine.Instance.CurrentParameters.Get(FAILURE_DISTANCE_MULTIPLE).Value); } }

		public List<IBarrierValue> Properties { get; private set; } 

		public PriceRangeBarrierPropertiesBase()
		{
			ParameterEngine.Instance.Register(this);

			this.Properties = new List<IBarrierValue>();

			foreach (PropertyInfo property in this.GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(PriceRangeValueAttribute))))
				this.Properties.Add(new PriceRangeBarrierProperty(this, property, Convert.ToDecimal(ParameterEngine.Instance.CurrentParameters.Get(FAILURE_DISTANCE_MULTIPLE).Value)));
		}
		
		public override List<IBarrierValue> GetBarriers()
		{
			return this.Properties.Cast<IBarrierValue>().ToList();
		}
	}
}
