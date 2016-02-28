using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorADXSimpleRelative : SimulatorOutputBase
	{
		private const decimal UNNEEDED_ADX_THRESHOLD = 0.0M;

		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; } 
		}
		
		public override string ToString()
		{
			return String.Format("R.ADXSimple({0})", this.IndicatorParameters);
		}

		public IndicatorADXSimpleRelative(int Period)
			: this(new IndicatorParameters(Period))
		{ }

		public IndicatorADXSimpleRelative(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add(new IndicatorADXSimple((int)this.IndicatorParameters.List[0], UNNEEDED_ADX_THRESHOLD));
		}
		
		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			if (!Ind.ADXSimpleRelative(period).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new ADXRelative();
					else
						prototype = (ADXRelative)Ind.ADXSimpleRelative(period)[1].Clone();

					Get(ref prototype, Ind);

					Ind.ADXSimpleRelative(period)[0] = (ADXRelative)prototype;

					Ind.ADXSimpleRelative(period).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			decimal adxRelative = 0.0M;
			if (Ind.Bar.CurrentBar > period)
				adxRelative = (Ind.ADXSimple(period, UNNEEDED_ADX_THRESHOLD)[0].ADXValue - Ind.ADXSimple(period, UNNEEDED_ADX_THRESHOLD)[period].ADXValue);

			(Prototype as ADXRelative).Value = adxRelative;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|Trendiness||", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			ADXRelative adxRelative = Data.ADXSimpleRelative(IndicatorParameters)[Instant.ExposureDate];

			if (adxRelative != null)
				return String.Format("{0}|{1}|{2}|", adxRelative.Value, (int)adxRelative.Trendiness, adxRelative.Trendiness);
			else
				return String.Format("{0}|{1}|{2}|", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|", "", "", "");
		}
	}
}
