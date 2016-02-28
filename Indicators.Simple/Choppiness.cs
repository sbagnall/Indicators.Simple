using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorChoppiness : SimulatorOutputBase
	{
		private const int TRUE_RANGE_PERIOD = 1;

		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies 
		{
			get { return _dependencies; } 
		}


		public override string ToString()
		{
			return String.Format("CHOP({0})", this.IndicatorParameters);
		}

		public IndicatorChoppiness(int Period, int SmoothingPeriod, decimal Threshold)
			: this(new IndicatorParameters(Period, SmoothingPeriod, Threshold))
		{ }


		public IndicatorChoppiness(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add(new IndicatorTrueRange(TRUE_RANGE_PERIOD));
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int smoothingPeriod = (int)this.IndicatorParameters.List[1];
			decimal threshold = (decimal)this.IndicatorParameters.List[2];

			if (!Ind.Chop(period, smoothingPeriod, threshold).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new Chop(threshold);
					else
						prototype = (Chop)Ind.Chop(period, smoothingPeriod, threshold)[1].Clone();

					Get(ref prototype, Ind);

					Ind.Chop(period, smoothingPeriod, threshold)[0] = (Chop)prototype;

					Ind.Chop(period, smoothingPeriod, threshold).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int smoothingPeriod = (int)this.IndicatorParameters.List[1];
			decimal threshold = (decimal)this.IndicatorParameters.List[2];

			Decimal choppiness = 0.5M;

			if (Ind.Bar.CurrentBar > (period + 1))
			{
				List<Decimal> maxHighCloses = new List<Decimal>();
				List<Decimal> minLowCloses = new List<Decimal>();
				List<Decimal> trueRanges = new List<Decimal>();

				for (int i = 0; i < period; i++)
				{
					maxHighCloses.Add(Math.Max(Ind.Bar[i].High, Ind.Bar[i + 1].Close));
					minLowCloses.Add(Math.Min(Ind.Bar[i].Low, Ind.Bar[i + 1].Close));
					trueRanges.Add(Ind.TrueRange(TRUE_RANGE_PERIOD)[i].Value);
				}

				Decimal hMax = maxHighCloses.Max();
				Decimal lMin = minLowCloses.Min();
				Decimal trSum = trueRanges.Sum();

				choppiness = (Decimal)Math.Log10((double)(trSum / (hMax - lMin))) / (Decimal)Math.Log10((double)period);

				if (Ind.Bar.CurrentBar > (period + 2))
				{
					decimal alpha = (2.0M / (smoothingPeriod + 1.0M));
					choppiness = (alpha * choppiness) + ((1.0M - alpha) * Ind.Chop(period, smoothingPeriod, threshold)[1].Value);
				}
			}

			(Prototype as Chop).Value = choppiness;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|Trendiness||", this.ToString());
		}

		protected override String GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			Chop choppiness = Data.Chop(IndicatorParameters)[Instant.ExposureDate];

			if (choppiness != null)
				return String.Format("{0}|{1}|{2}|", choppiness.Value, (int)choppiness.Trendiness, choppiness.Trendiness);
			else
				return String.Format("{0}|{1}|{2}", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}", "", "", "");
		}
	}
}
