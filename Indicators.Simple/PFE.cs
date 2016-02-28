using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorPFE : SimulatorOutputBase
	{
		
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; } 
		}


		public override string ToString()
		{
			return String.Format("PFE({0})", this.IndicatorParameters);
		}

		public IndicatorPFE(int Period, int SmoothingPeriod, decimal ThresholdPercentage)
			: this(new IndicatorParameters(Period, SmoothingPeriod, ThresholdPercentage))
		{ }
		
		public IndicatorPFE(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int smoothingPeriod = (int)this.IndicatorParameters.List[1];
			decimal thresholdPercentage = (decimal)this.IndicatorParameters.List[2];

			if (!Ind.PFE(period, smoothingPeriod, thresholdPercentage).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new PFE(thresholdPercentage);
					else
						prototype = (PFE)Ind.PFE(period, smoothingPeriod, thresholdPercentage)[1].Clone();

					Get(ref prototype, Ind);

					Ind.PFE(period, smoothingPeriod, thresholdPercentage)[0] = (PFE)prototype;

					Ind.PFE(period, smoothingPeriod, thresholdPercentage).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int smoothingPeriod = (int)this.IndicatorParameters.List[1];
			decimal thresholdPercentage = (decimal)this.IndicatorParameters.List[2];

			Decimal pfe = Decimal.MinValue;
			Decimal alpha = 2.0M / (smoothingPeriod + 1.0M);

			Decimal directPath = 0.0M;
			Decimal complexPath = 0.0M;

			if (Ind.Bar.CurrentBar > (period + 1))
			{
				for (int i = 0; i < period; i++)
					complexPath += (decimal)Math.Sqrt(Math.Pow((double)(Ind.Bar[i].Close - Ind.Bar[i + 1].Close), 2) + 1.0);

				directPath = (decimal)Math.Sqrt(Math.Pow((double)(Ind.Bar[0].Close - Ind.Bar[period].Close), 2) + Math.Pow((double)period, 2));

				Decimal tmpFE = Math.Sign(Ind.Bar[0].Close - Ind.Bar[period].Close) * Math.Round((directPath / complexPath) * 100.0M, 0);
				pfe = (alpha * tmpFE) + ((1.0M - alpha) * Ind.PFE(period, smoothingPeriod, thresholdPercentage)[1].Value);
			}
			else
			{
				pfe = 0.0M;
			}

			(Prototype as PFE).Value = pfe;
		}
			
		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|Trendiness||Direction||Oscillation||", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			PFE pfe = Data.PFE(IndicatorParameters)[Instant.ExposureDate];

			if (pfe != null)
				return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|", pfe.Value, (int)pfe.Trendiness, pfe.Trendiness, (int)pfe.Direction, pfe.Direction, (int)pfe.Oscillation, pfe.Oscillation);
			else
				return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|", "", "", "", "", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|", "", "", "", "", "", "", "");
		}
	}
}
