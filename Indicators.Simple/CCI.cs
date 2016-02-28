using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteveBagnall.Trading.Indicators
{
    /// <summary>
    /// http://stockcharts.com/school/doku.php?id=chart_school:technical_indicators:commodity_channel_index_cci
    /// </summary>
    public class IndicatorCCI : SimulatorOutputBase
	{
		private const decimal CCI_CONSTANT = 0.015M;

		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; }
		}


		public override string ToString()
		{
			return String.Format("CCI({0})", this.IndicatorParameters);
		}

		public IndicatorCCI(int Period, int SmoothingPeriod, decimal ThresholdValue)
			: this(new IndicatorParameters(Period, SmoothingPeriod, ThresholdValue))
		{ }

		public IndicatorCCI(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{

		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int smoothingPeriod = (int)this.IndicatorParameters.List[1];
			decimal thresholdValue = (decimal)this.IndicatorParameters.List[2];

			if (!Ind.CCI(period, smoothingPeriod, thresholdValue).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new CCI(thresholdValue);
					else
						prototype = (CCI)Ind.CCI(period, smoothingPeriod, thresholdValue)[1].Clone();

					Get(ref prototype, Ind);

					Ind.CCI(period, smoothingPeriod, thresholdValue)[0] = (CCI)prototype;

					Ind.CCI(period, smoothingPeriod, thresholdValue).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int smoothingPeriod = (int)this.IndicatorParameters.List[1];
			decimal thresholdValue = (decimal)this.IndicatorParameters.List[2];

			decimal cci = Decimal.MinValue;

			decimal alpha = 2.0M / (smoothingPeriod + 1.0M);

			decimal typicalPrice = (Ind.Bar[0].High + Ind.Bar[0].Low + Ind.Bar[0].Close) / 3.0M;

			decimal smaOfTypicalPrice = 0.0M;
			decimal meanDeviation = 0.0M;

			if (Ind.Bar.CurrentBar >= period)
			{
				List<decimal> typicalPrices = new List<decimal>();
				typicalPrices.Add(typicalPrice);

				for (int i = 1; i < period; i++)
					typicalPrices.Add(Ind.CCI(period, smoothingPeriod, thresholdValue)[i].TypicalPrice);

				smaOfTypicalPrice = typicalPrices.Average();

				List<decimal> typicalPriceDeviations = new List<decimal>();

				for (int i = 0; i < typicalPrices.Count; i++)
					typicalPriceDeviations.Add(Math.Abs(typicalPrices[i] - smaOfTypicalPrice));

				meanDeviation = typicalPriceDeviations.Average();

				cci = (typicalPrice - smaOfTypicalPrice) / (CCI_CONSTANT * meanDeviation);

				cci = (alpha * cci) + ((1.0M - alpha) * Ind.CCI(period, smoothingPeriod, thresholdValue)[1].Value);
			}
			else
			{
				cci = 0.0M;
			}

			(Prototype as CCI).TypicalPrice = typicalPrice;
			(Prototype as CCI).Value = cci;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|Trendiness||Direction||Oscillation||", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			CCI cci = Data.CCI(IndicatorParameters)[Instant.ExposureDate];

			if (cci != null)
				return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|", cci.Value, (int)cci.Trendiness, cci.Trendiness, (int)cci.Direction, cci.Direction, (int)cci.Oscillation, cci.Oscillation);
			else
				return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|", "", "", "", "", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|", "", "", "", "", "", "", "");
		}
	}
}
