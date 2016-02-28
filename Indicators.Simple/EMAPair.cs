using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorEMAPairs : PriceRangeBarrierIndicatorBase, IMovingAveragePairIndicator
	{
		private const int ATR_PERIOD = 14;
		private const decimal ATR_PROXIMITY_MULTIPLE = 1.0M;

		private const Decimal GOLDEN_RATIO = 1.618M;
		private static readonly Decimal RATIO = (Decimal)Math.Pow((double)GOLDEN_RATIO, 2.0); // equivalent to the next but one fabinacci number

		#region Barriers

		protected override StudyValue GetStudyValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return new StudyValue(Ind.Bar[PeriodsAgo].High, Ind.Bar[PeriodsAgo].Low, Ind.Bar[PeriodsAgo].Close);
		}

		protected override PriceRangeBarrierPropertiesBase GetPriceRangeBarrierProperties(int PeriodsAgo, IIndicatorValues Ind)
		{
			return Ind.EmaPair(this.IndicatorParameters)[PeriodsAgo];
		}

		protected override PriceRangeBarrierPropertiesBase GetPriceRangeBarrierProperties(DateTime Date, IIndicatorValues Ind)
		{
			return Ind.EmaPair(this.IndicatorParameters)[Date];
		}

		#endregion

		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; } 
		}
	
		public override string ToString()
		{
			return String.Format("EMA({0})", this.IndicatorParameters);
		}

		public IndicatorEMAPairs(int FastPeriod)
			: this(FastPeriod, (int)Math.Round(FastPeriod * RATIO, 0))
		{
			
		}

		public IndicatorEMAPairs(int FastPeriod, int SlowPeriod)
			: this(new IndicatorParameters(FastPeriod, SlowPeriod))
		{ }

		public IndicatorEMAPairs(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add(new IndicatorATR(ATR_PERIOD));
		}

		public IMovingAveragePairValue GetMAPairValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return Ind.EmaPair(this.IndicatorParameters)[PeriodsAgo];
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
			//        lastValue = Ind.EmaPair(fastPeriod, slowPeriod)[i].Fast; 
			//        ranges.Add((decimal)Math.Abs((double)lastValue - (double)Ind.EmaPair(fastPeriod, slowPeriod)[i + 1].Fast));
			//    }
			//}

			//ranges.Add((decimal)Math.Abs((NextValue as EMAPair).Fast - lastValue));

			//if (lastValue == 0.0M)
			//    return 0.0M;
			//else
			//    return (ranges.Count > 0) ? ranges.Average() : 0.0M;
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int fastPeriod = (int)this.IndicatorParameters.List[0];
			int slowPeriod = (int)this.IndicatorParameters.List[1];

			if (!Ind.EmaPair(fastPeriod, slowPeriod).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new EMAPair();
					else
						prototype = (EMAPair)Ind.EmaPair(fastPeriod, slowPeriod)[1].Clone();

					Get(ref prototype, Ind);

					Ind.EmaPair(fastPeriod, slowPeriod)[0] = (EMAPair)prototype;

					Ind.EmaPair(fastPeriod, slowPeriod).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void GetData(ref object Prototype, IIndicatorValues Ind)
		{
			int fastPeriod = (int)this.IndicatorParameters.List[0];
			int slowPeriod = (int)this.IndicatorParameters.List[1];

			decimal fast = 0.0M;
			decimal slow = 0.0M;

			decimal fastEmaAlpha = (2.0M / (Decimal)(fastPeriod + 1.0M));
			//Decimal slowEmaAlpha = (2.0M / (Decimal)((int)Math.Round(fastPeriod * RATIO, 0) + 1.0M));
			Decimal slowEmaAlpha = (2.0M / (Decimal)(slowPeriod + 1.0M));

			if (Ind.Bar.CurrentBar > 1)
			{
				fast = (fastEmaAlpha * Ind.Bar[0].Close) + ((1.0M - fastEmaAlpha) * Ind.EmaPair(fastPeriod, slowPeriod)[1].Fast);
				slow = (slowEmaAlpha * Ind.Bar[0].Close) + ((1.0M - slowEmaAlpha) * Ind.EmaPair(fastPeriod, slowPeriod)[1].Slow);
			}
			else
			{
				fast = Ind.Bar[0].Close;
				slow = Ind.Bar[0].Close;
			}

			(Prototype as EMAPair).Fast = fast;
			(Prototype as EMAPair).Slow = slow;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			int fastPeriod = (int)IndicatorParameters.List[0];
			int slowPeriod = (int)IndicatorParameters.List[1];
			
			return String.Format("{0}|EMA({1})|Direction||", this.ToString(), slowPeriod);
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			int fastPeriod = (int)IndicatorParameters.List[0];
			int slowPeriod = (int)IndicatorParameters.List[1];

			EMAPair emaPair = Data.EmaPair(fastPeriod, slowPeriod)[Instant.ExposureDate];

			return String.Format("{0}|{1}|{2}|{3}|", 
				(emaPair == null) ? "" : Convert.ToString(emaPair.Fast), 
				(emaPair == null) ? "" : Convert.ToString(emaPair.Slow),
				(int)emaPair.Direction,
				emaPair.Direction);
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format(String.Format("{0}|{1}|{2}|{3}|", "", "", "", ""));
		}
	}
}
