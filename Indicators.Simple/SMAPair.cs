using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorSMAPair : PriceRangeBarrierIndicatorBase, IMovingAveragePairIndicator
	{
		private const int ATR_PERIOD = 14;
		private const decimal ATR_PROXIMITY_MULTIPLE = 1.0M;

		#region Barriers

		protected override StudyValue GetStudyValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return new StudyValue(Ind.Bar[PeriodsAgo].High, Ind.Bar[PeriodsAgo].Low, Ind.Bar[PeriodsAgo].Close);
		}

		protected override PriceRangeBarrierPropertiesBase GetPriceRangeBarrierProperties(int PeriodsAgo, IIndicatorValues Ind)
		{
			return Ind.SMAPair(this.IndicatorParameters)[PeriodsAgo];
		}

		protected override PriceRangeBarrierPropertiesBase GetPriceRangeBarrierProperties(DateTime Date, IIndicatorValues Ind)
		{
			return Ind.SMAPair(this.IndicatorParameters)[Date];
		}

		#endregion


		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>(); 
		public override List<SimulatorOutputBase> Dependencies 
		{
			get { return _dependencies; } 
		}

		public override string ToString()
		{
			return String.Format("SMA({0})", this.IndicatorParameters);
		}

		public IndicatorSMAPair(int FastPeriod, int SlowPeriod)
			: this(new IndicatorParameters(FastPeriod, SlowPeriod))
		{ }

		public IndicatorSMAPair(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add(new IndicatorATR(ATR_PERIOD));
		}

		public IMovingAveragePairValue GetMAPairValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return (IMovingAveragePairValue)Ind.SMAPair(this.IndicatorParameters)[PeriodsAgo];
		}

		public override decimal GetProximateDistance(IIndicatorValues Ind, object NextValue)
		{
			return Ind.ATR(ATR_PERIOD)[0].Value * ATR_PROXIMITY_MULTIPLE;

			//int fastPeriod = (int)this.IndicatorParameters.List[0];
			//int slowPeriod = (int)this.IndicatorParameters.List[1];

			//List<decimal> ranges = new List<decimal>();
			//decimal lastValue = 0.0M;

			//if (Ind.Bar.CurrentBar >= (fastPeriod + 1))
			//{
			//    for (int i = 1; i < fastPeriod; i++)
			//    {
			//        lastValue = Ind.SMAPair(fastPeriod, slowPeriod)[i].Fast;
			//        ranges.Add((decimal)Math.Abs((double)lastValue - (double)(Ind.SMAPair(fastPeriod, slowPeriod)[i + 1].Fast)));
			//    }
			//}

			//ranges.Add((decimal)Math.Abs((double)((NextValue as SMAPair).Fast) - (double)lastValue));

			//if (lastValue == 0.0M)
			//    return 0.0M;
			//else
			//    return (ranges.Count > 0) ? ranges.Average() : 0.0M;
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int fastPeriod = (int)this.IndicatorParameters.List[0];
			int slowPeriod = (int)this.IndicatorParameters.List[1];

			if (!Ind.SMAPair(fastPeriod, slowPeriod).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new SMAPair();
					else
						prototype = (SMAPair)Ind.SMAPair(fastPeriod, slowPeriod)[1].Clone();

					Get(ref prototype, Ind);

					Ind.SMAPair(fastPeriod, slowPeriod)[0] = (SMAPair)prototype;

					Ind.SMAPair(fastPeriod, slowPeriod).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void GetData(ref object Prototype, IIndicatorValues Ind)
		{
			int fastPeriod = (int)this.IndicatorParameters.List[0];
			int slowPeriod = (int)this.IndicatorParameters.List[1];

			decimal smaFast;
			if (Ind.Bar.CurrentBar > fastPeriod)
			{
				smaFast = 0.0M;
				for (int i = fastPeriod - 1; i >= 0; i--)
					smaFast += Ind.Bar[i].Close;
			}
			else
			{
				smaFast = 0.0M;
				for (int i = 0; i < Ind.Bar.CurrentBar; i++)
					smaFast += Ind.Bar[i].Close;

				fastPeriod = Ind.Bar.CurrentBar;
			}

			decimal smaSlow;
			if (Ind.Bar.CurrentBar > slowPeriod)
			{
				smaSlow = 0.0M;
				for (int i = slowPeriod - 1; i >= 0; i--)
					smaSlow += Ind.Bar[i].Close;
			}
			else
			{
				smaSlow = 0.0M;
				for (int i = 0; i < Ind.Bar.CurrentBar; i++)
					smaSlow += Ind.Bar[i].Close;

				slowPeriod = Ind.Bar.CurrentBar;
			}

			(Prototype as SMAPair).Fast = smaFast / (decimal)fastPeriod;
			(Prototype as SMAPair).Slow = smaSlow / (decimal)slowPeriod;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			int fastPeriod = (int)IndicatorParameters.List[0];
			int slowPeriod = (int)IndicatorParameters.List[1];

			return String.Format("{0}||Direction||", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			SMAPair sma = Data.SMAPair(IndicatorParameters)[Instant.ExposureDate];

			if (sma != null)
				return String.Format("{0}|{1}|{2}|{3}|", sma.Fast, sma.Slow, (int)sma.Direction, sma.Direction);
			else
				return String.Format("{0}|{1}|{2}|{3}|", "", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|{3}|", "", "", "", "");
		}
	}
}
