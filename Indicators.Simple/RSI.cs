using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;
using System.Collections.Generic;
using System.Linq;



namespace SteveBagnall.Trading.Indicators
{


    public class IndicatorRSI : SimulatorOutputBase, IStudyIndicator, IRenkoStudyIndicator
	{
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; } 
		}

		#region Study

		public StudyValue GetStudyValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return new StudyValue(Ind.RSI(this.IndicatorParameters)[PeriodsAgo].Value);
		}

		public StudyValue GetStudyValue(DateTime Date, IIndicatorValues Ind)
		{
			return new StudyValue(Ind.RSI(IndicatorParameters)[Date].Value);
		}

		#endregion

		#region Renko

		public int ATRPeriod { get { return (int)this.IndicatorParameters.List[0]; } }

		// TODO: THIS IS BOLLOCKS, BUT NEEDED FOR PROXIMITY ETC IN NORMAL TRENDLINES
		public decimal ATRMultiple { get { return 1.0M; } }

		public decimal GetRange(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			decimal thresholdPercentage = (decimal)this.IndicatorParameters.List[1];

			List<decimal> ranges = new List<decimal>();

			if (Ind.Bar[0].Number < (period + 1))
				return 0.0M;

			for (int i = 0; i < period; i++)
				ranges.Add((decimal)Math.Abs((double)Ind.RSI(period, thresholdPercentage)[i].Value - (double)Ind.RSI(period, thresholdPercentage)[i + 1].Value));

			return ranges.Average();
		}

		#endregion

		public override string ToString()
		{
			return String.Format("RSI({0})", this.IndicatorParameters);
		}

		public IndicatorRSI(int Period, decimal ThresholdPercentage)
			: this(new IndicatorParameters(Period, ThresholdPercentage))
		{ }

		public IndicatorRSI(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{ }
		
		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			decimal thresholdPercentage = (decimal)this.IndicatorParameters.List[1];

			if (!Ind.RSI(period, thresholdPercentage).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new RSI(thresholdPercentage);
					else
						prototype = (RSI)Ind.RSI(period, thresholdPercentage)[1].Clone();

					Get(ref prototype, Ind);

					Ind.RSI(period, thresholdPercentage)[0] = (RSI)prototype;

					Ind.RSI(period, thresholdPercentage).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int	period = (int)this.IndicatorParameters.List[0];
			decimal thresholdPercentage = (decimal)this.IndicatorParameters.List[1];

			decimal averageGain = 0.0M;
			decimal averageLoss = 0.0M;
			decimal rs = 0.0M;
			decimal rsi = 0.0M;

			if (Ind.Bar.CurrentBar >= (period + 1))
			{
				if (Ind.Bar.CurrentBar == (period + 1))
				{
					decimal totalGains = 0.0M;
					decimal totalLosses = 0.0M;
					for (int i = 0; i < period; i++)
					{
						decimal change = Ind.Bar[i].Close - Ind.Bar[i + 1].Close;
						if (change > 0)
							totalGains += change;
						else
							totalLosses += Math.Abs(change);
					}

					averageGain = totalGains / period;
					averageLoss = totalLosses / period;
				}
				else
				{
					decimal change = Ind.Bar[0].Close - Ind.Bar[1].Close;

					averageGain = ((Ind.RSI(period, thresholdPercentage)[1].AverageGain * (period - 1)) + ((change > 0) ? change : 0.0M)) / period;
					averageLoss = ((Ind.RSI(period, thresholdPercentage)[1].AverageLoss * (period - 1)) + ((change < 0) ? Math.Abs(change) : 0.0M)) / period;
				}

				rs = (averageLoss == 0.0M) ? averageGain : (averageGain / averageLoss);
				rsi = 100.0M - (100.0M / (1.0M + rs));
			}

			(Prototype as RSI).AverageGain = averageGain;
			(Prototype as RSI).AverageLoss = averageLoss;
			(Prototype as RSI).RS = rs;
			(Prototype as RSI).Value = rsi;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|Oscillation||", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			RSI rsi = Data.RSI(IndicatorParameters)[Instant.ExposureDate];

			if (rsi != null)
				return String.Format("{0}|{1}|{2}|", rsi.Value, (int)rsi.Oscillation, rsi.Oscillation);
			else
				return String.Format("{0}|{1}|{2}|", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|", "", "", "");
		}
	}
}
