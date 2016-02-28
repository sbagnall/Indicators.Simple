using SteveBagnall.Trading.Indicators.BarrierStudyPair;
using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteveBagnall.Trading.Indicators
{
    public abstract class PriceRangeBarrierIndicatorBase : SimulatorOutputBase, IBarrierIndicator/*, IParametized*/
	{
		//#region Parameters

		//public static String PROXIMITY_DISTANCE_FRACTION_OF_RANGE = "PRICERANGEBARRIERINDICATORBASE_PROXIMITY_DISTANCE_FRACTION_OF_RANGE";

		//public virtual Parameters Parameters
		//{
		//    get
		//    {
		//        Parameters parameters = new Parameters();

		//        parameters.Add(new Parameter(
		//            PROXIMITY_DISTANCE_FRACTION_OF_RANGE,
		//            typeof(decimal),
		//            0.15M,
		//            false, // IsTesting
		//            0.0M,
		//            0.50M,
		//            TestSequence.Arithmatic));


		//        return parameters;
		//    }
		//}

		//#endregion

		//public decimal ProximityDistanceFractionOfRange { get { return Convert.ToDecimal(ParameterEngine.Instance.CurrentParameters.Get(PROXIMITY_DISTANCE_FRACTION_OF_RANGE).Value); } }

		public PriceRangeBarrierIndicatorBase(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			//ParameterEngine.Instance.Register(this);
		}

		protected abstract StudyValue GetStudyValue(int PeriodsAgo, IIndicatorValues Ind);

		protected abstract PriceRangeBarrierPropertiesBase GetPriceRangeBarrierProperties(int PeriodsAgo, IIndicatorValues Ind);

		protected abstract PriceRangeBarrierPropertiesBase GetPriceRangeBarrierProperties(DateTime Date, IIndicatorValues Ind);

		public abstract void GetData(ref object Prototype, IIndicatorValues Ind);

		public abstract decimal GetProximateDistance(IIndicatorValues Ind, object NextValue);

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			GetData(ref Prototype, Ind);

			decimal proximateDistance = GetProximateDistance(Ind, Prototype);

			foreach (PriceRangeBarrierProperty barrierProperty in (Prototype as PriceRangeBarrierPropertiesBase).Properties)
				barrierProperty.ProximateDistance = proximateDistance;
		}

		public List<IBarrierValue> GetBarriers(int PeriodsAgo, IIndicatorValues Ind)
		{
			return GetPriceRangeBarrierProperties(PeriodsAgo, Ind).Properties.Cast<IBarrierValue>().ToList();
		}
	}
}
