using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;
using System.Collections.Generic;


namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorKeltner : PriceRangeBarrierIndicatorBase
	{
		#region Barriers

		protected override StudyValue GetStudyValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return new StudyValue(Ind.Bar[PeriodsAgo].High, Ind.Bar[PeriodsAgo].Low, Ind.Bar[PeriodsAgo].Close);
		}

		protected override PriceRangeBarrierPropertiesBase GetPriceRangeBarrierProperties(int PeriodsAgo, IIndicatorValues Ind)
		{
			return Ind.KeltnerChannel(this.IndicatorParameters)[PeriodsAgo];
		}

		protected override PriceRangeBarrierPropertiesBase GetPriceRangeBarrierProperties(DateTime Date, IIndicatorValues Ind)
		{
			return Ind.KeltnerChannel(this.IndicatorParameters)[Date];
		}

		#endregion


		public override decimal GetProximateDistance(IIndicatorValues Ind, object NextValue)
		{
			decimal proximityMultiple = (decimal)this.IndicatorParameters.List[4];

			return (decimal)Math.Abs((double)(NextValue as KeltnerChannel).Upper - (double)(NextValue as KeltnerChannel).Lower) * proximityMultiple;
		}
	
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies 
		{
			get { return _dependencies; }
		}

		public override string ToString()
		{
			return String.Format("KELT({0})", this.IndicatorParameters);
		}

		public IndicatorKeltner(int Period, int ATRPeriod, decimal ATRMultiple, decimal MarginThreshold, decimal ProximityMultiple)
			: this(new IndicatorParameters(Period, ATRPeriod, ATRMultiple, MarginThreshold, ProximityMultiple))
		{ }

		public IndicatorKeltner(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add(new IndicatorEMAPairs((int)this.IndicatorParameters.List[0]));
			this.Dependencies.Add(new IndicatorATR((int)this.IndicatorParameters.List[1]));
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int atrPeriod = (int)this.IndicatorParameters.List[1];
			decimal atrMultiple = (decimal)this.IndicatorParameters.List[2];
			decimal marginThreshold = (decimal)this.IndicatorParameters.List[3];
			decimal proximityMultiple = (decimal)this.IndicatorParameters.List[4];


			if (!Ind.KeltnerChannel(period, atrPeriod, atrMultiple, marginThreshold, proximityMultiple).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new KeltnerChannel(marginThreshold);
					else
						prototype = (KeltnerChannel)Ind.KeltnerChannel(period, atrPeriod, atrMultiple, marginThreshold, proximityMultiple)[1].Clone();

					Get(ref prototype, Ind);

					Ind.KeltnerChannel(period, atrPeriod, atrMultiple, marginThreshold, proximityMultiple)[0] = (KeltnerChannel)prototype;

					Ind.KeltnerChannel(period, atrPeriod, atrMultiple, marginThreshold, proximityMultiple).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void GetData(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int atrPeriod = (int)this.IndicatorParameters.List[1];
			decimal atrMultiple = (decimal)this.IndicatorParameters.List[2];
			decimal MarginThreshold = (decimal)this.IndicatorParameters.List[3];
			decimal proximityMultiple = (decimal)this.IndicatorParameters.List[4];

			Decimal ema = Ind.EmaPair(period)[0].Fast;
			Decimal atr = Ind.ATR(atrPeriod)[0].Value;

			(Prototype as KeltnerChannel).Lower = ema - (atr * atrMultiple);
			(Prototype as KeltnerChannel).Mid = ema;
			(Prototype as KeltnerChannel).Upper = ema + (atr * atrMultiple);
			(Prototype as KeltnerChannel).Price = Ind.Bar[0].Close;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0} Low|Mid|High|Oscillation||", this.ToString());
		}

		protected override String GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			KeltnerChannel keltner = Data.KeltnerChannel(IndicatorParameters)[Instant.ExposureDate];

			if (keltner != null)
				return String.Format("{0}|{1}|{2}|{3}|{4}|", keltner.Lower, keltner.Mid, keltner.Upper, (int)keltner.Oscillation, keltner.Oscillation);
			else
				return String.Format("{0}|{1}|{2}|{3}|{4}|", "", "", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|{3}|{4}|", "", "", "", "", "");
		}
	}
}
