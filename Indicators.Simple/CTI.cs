using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Shared;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
    /// <summary>
    /// Chande Trend Index
    /// </summary>
    public class IndicatorCTI : SimulatorOutputBase
	{
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies 
		{
			get { return _dependencies; } 
		}

		public override string ToString()
		{
			return String.Format("CTI({0})", this.IndicatorParameters);
		}

		public IndicatorCTI(int Period, int SmoothingPeriod)
			: this(new IndicatorParameters(Period, SmoothingPeriod))
		{
		}

		public IndicatorCTI(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
		}
	
		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int smoothingPeriod = (int)this.IndicatorParameters.List[1];

			if (!Ind.CTI(period, smoothingPeriod).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new CTI();
					else
						prototype = (CTI)Ind.CTI(period, smoothingPeriod)[1].Clone();

					Get(ref prototype, Ind);

					Ind.CTI(period, smoothingPeriod)[0] = (CTI)prototype;

					Ind.CTI(period, smoothingPeriod).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int smoothingPeriod = (int)this.IndicatorParameters.List[1];
			
			decimal cti = 1.0M;

			if (Ind.Bar.CurrentBar > (period + 1))
			{
				List<double> logChanges = new List<double>();

				// TODO: why is this <0 at times??
				for (int i = 0; i < period; i++)
					logChanges.Add(Math.Log((double)Ind.Bar[i].Close / (double)Ind.Bar[i + 1].Close));

				cti = (decimal)Math.Log((double)Ind.Bar[0].Close / (double)Ind.Bar[period].Close)
					/ (decimal)(Utilities.StandardDeviation(logChanges) * Math.Sqrt(period));

				if (Ind.Bar.CurrentBar > (period + 2))
				{
					decimal alpha = (2.0M / (smoothingPeriod + 1.0M));
					cti = (alpha * cti) + ((1.0M - alpha) * Ind.CTI(period, smoothingPeriod)[1].Value);
				}
			}

			(Prototype as CTI).Value = cti;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|Trendiness||Direction||", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			CTI cti = Data.CTI(IndicatorParameters)[Instant.ExposureDate];
			if (cti != null)
				return String.Format("{0}|{1}|{2}|{3}|{4}|", cti.Value, (int)cti.Trendiness, cti.Trendiness, (int)cti.Direction, cti.Direction);
			else
				return String.Format("{0}|{1}|{2}|{3}|{4}|", "", "", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|{3}|{4}|", "", "", "", "", "");
		}
	}
}
