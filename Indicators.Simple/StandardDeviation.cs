using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Shared;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorSD : SimulatorOutputBase
	{
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; }
		}

		public override string ToString()
		{
			return String.Format("SD({0})", this.IndicatorParameters);
		}

		public IndicatorSD(int Period)
			: this(new IndicatorParameters(Period))
		{ }

		public IndicatorSD(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
		}
	
		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			if (!Ind.SD(period).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new StandardDeviation();
					else
						prototype = (StandardDeviation)Ind.SD(period)[1].Clone();

					Get(ref prototype, Ind);

					Ind.SD(period)[0] = (StandardDeviation)prototype;

					Ind.SD(period).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			double sd = 0.0;

			if (Ind.Bar.CurrentBar > (period + 1))
			{
				List<double> prices = new List<double>();
				for (int i = 0; i < period; i++)
					prices.Add((double)Ind.Bar[i].Close);

				sd = Utilities.StandardDeviation(prices);
			}

			(Prototype as StandardDeviation).Value = (decimal)sd;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("TR({0})|", IndicatorParameters);
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			StandardDeviation sd = Data.SD(IndicatorParameters)[Instant.ExposureDate];

			if (sd != null)
				return String.Format("{0}|", sd.Value);
			else
				return String.Format("{0}|", "");
		}
		
		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|", "");
		}
	}
}
