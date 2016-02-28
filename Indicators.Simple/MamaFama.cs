using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorMamaFama : PriceRangeBarrierIndicatorBase, IMovingAveragePairIndicator
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
			return Ind.MamaFama(this.IndicatorParameters)[PeriodsAgo];
		}

		protected override PriceRangeBarrierPropertiesBase GetPriceRangeBarrierProperties(DateTime Date, IIndicatorValues Ind)
		{
			return Ind.MamaFama(this.IndicatorParameters)[Date];
		}

		#endregion


		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>(); 
		public override List<SimulatorOutputBase> Dependencies 
		{
			get { return _dependencies; } 
		}

		public override string ToString()
		{
			return String.Format("MAMA({0})", this.IndicatorParameters);
		}

		public IndicatorMamaFama(int Period)
			: this(new IndicatorParameters(Period))
		{ }

		public IndicatorMamaFama(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add(new IndicatorMESA());
			this.Dependencies.Add(new IndicatorATR(ATR_PERIOD));
		}

		public IMovingAveragePairValue GetMAPairValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return (IMovingAveragePairValue)Ind.MamaFama(this.IndicatorParameters)[PeriodsAgo];
		}

		public override decimal GetProximateDistance(IIndicatorValues Ind, object NextValue)
		{
			return Ind.ATR(ATR_PERIOD)[0].Value * ATR_PROXIMITY_MULTIPLE;

			//int period = (int)this.IndicatorParameters.List[0];
			
			//List<decimal> ranges = new List<decimal>();
			//decimal lastValue = 0.0M;

			//if (Ind.Bar.CurrentBar >= (period + 1))
			//{
			//    for (int i = 1; i < period; i++)
			//    {
			//        lastValue = Ind.MamaFama(period)[i].MAMA;
			//        ranges.Add((decimal)Math.Abs((double)lastValue - (double)(Ind.MamaFama(period)[i + 1].MAMA)));
			//    }
			//}

			//ranges.Add((decimal)Math.Abs((double)((NextValue as MamaFama).MAMA) - (double)lastValue));

			//if (lastValue == 0.0M)
			//    return 0.0M;
			//else
			//    return (ranges.Count > 0) ? ranges.Average() : 0.0M;
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			if (!Ind.MamaFama(period).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new MamaFama();
					else
						prototype = (MamaFama)Ind.MamaFama(period)[1].Clone();

					Get(ref prototype, Ind);

					Ind.MamaFama(period)[0] = (MamaFama)prototype;

					Ind.MamaFama(period).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void GetData(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			decimal alpha = 0.0M;
			
			decimal mama = 0.0M;
			decimal fama = 0.0M;

			if (Ind.Bar.CurrentBar == 1)
			{
				mama = ((Ind.Bar[0].High + Ind.Bar[0].Low) / 2.0M);
				fama = mama;
			}
			else if (Ind.Bar.CurrentBar <= 6)
			{
				alpha = (2.0M / (period + 1.0M));
				mama = (alpha * ((Ind.Bar[0].High + Ind.Bar[0].Low) / 2.0M)) + ((1.0M - alpha) * Ind.MamaFama(period)[1].MAMA);
				fama = (0.5M * alpha * mama) + ((1.0M - (0.5M * alpha)) * Ind.MamaFama(period)[1].FAMA);
			}
			if (Ind.Bar.CurrentBar > 6)
			{
				decimal fastAlpha = (2.0M / (period + 1.0M));
				decimal deltaPhase = Ind.MESA()[0].DeltaPhase;

				alpha = fastAlpha / deltaPhase;
			
				mama = (alpha * ((Ind.Bar[0].High + Ind.Bar[0].Low) / 2.0M)) + ((1.0M - alpha) * Ind.MamaFama(period)[1].MAMA);
				fama = (0.5M * alpha * mama) + ((1.0M - (0.5M * alpha)) * Ind.MamaFama(period)[1].FAMA);
			}
			
			(Prototype as MamaFama).MAMA = mama;
			(Prototype as MamaFama).FAMA = fama;
			(Prototype as MamaFama).Alpha = alpha;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|FAMA|Alpha|Direction||", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			MamaFama mama = Data.MamaFama(IndicatorParameters)[Instant.ExposureDate];

			if (mama != null)
				return String.Format("{0}|{1}|{2}|{3}|{4}|", mama.MAMA, mama.FAMA, mama.Alpha, (int)mama.Direction, mama.Direction);
			else
				return String.Format("{0}|{1}|{2}|{3}|{4}|", "", "", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|{3}|{4}|", "", "", "", "", "");
		}
	}
}
