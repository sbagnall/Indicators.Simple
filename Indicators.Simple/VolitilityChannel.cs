using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;
using System.Collections.Generic;


namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorVolatilityChannel : PriceRangeBarrierIndicatorBase
	{
		#region Barriers

		protected override StudyValue GetStudyValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return new StudyValue(Ind.Bar[PeriodsAgo].High, Ind.Bar[PeriodsAgo].Low, Ind.Bar[PeriodsAgo].Close);
		}

		protected override PriceRangeBarrierPropertiesBase GetPriceRangeBarrierProperties(int PeriodsAgo, IIndicatorValues Ind)
		{
			return Ind.VolatilityChannel(this.IndicatorParameters)[PeriodsAgo];
		}

		protected override PriceRangeBarrierPropertiesBase GetPriceRangeBarrierProperties(DateTime Date, IIndicatorValues Ind)
		{
			return Ind.VolatilityChannel(this.IndicatorParameters)[Date];
		}

		#endregion

		public override decimal GetProximateDistance(IIndicatorValues Ind, object NextValue)
		{
			decimal proximityMultiple = (decimal)this.IndicatorParameters.List[4];

			return (decimal)Math.Abs((double)(NextValue as VolatilityChannel).Upper - (double)(NextValue as VolatilityChannel).Lower) * proximityMultiple;
		}
	
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; }
		}

		//public override ISignal Signal
		//{
		//    get { return new BarrierStudySignal(null, this); }
		//}

		public override string ToString()
		{
			return String.Format("CH.VOL({0})", this.IndicatorParameters);
		}

		public IndicatorVolatilityChannel(int Period, int VolitalityPeriod, decimal VolatilityMultiple, decimal MarginThreshold, decimal ProximityMultiple)
			: this(new IndicatorParameters(Period, VolitalityPeriod, VolatilityMultiple, MarginThreshold, ProximityMultiple))
		{ }

		public IndicatorVolatilityChannel(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add(new IndicatorEMAPairs((int)this.IndicatorParameters.List[0]));
			this.Dependencies.Add(new IndicatorVolatility((int)this.IndicatorParameters.List[1]));
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int volatilityPeriod = (int)this.IndicatorParameters.List[1];
			decimal volitilityMultiple = (decimal)this.IndicatorParameters.List[2];
			decimal marginThreshold = (decimal)this.IndicatorParameters.List[3];
			decimal proximityMultiple = (decimal)this.IndicatorParameters.List[4];

			if (!Ind.VolatilityChannel(period, volatilityPeriod, volitilityMultiple, marginThreshold, proximityMultiple).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new VolatilityChannel(marginThreshold);
					else
						prototype = (VolatilityChannel)Ind.VolatilityChannel(period, volatilityPeriod, volitilityMultiple, marginThreshold, proximityMultiple)[1].Clone();

					Get(ref prototype, Ind);

					Ind.VolatilityChannel(period, volatilityPeriod, volitilityMultiple, marginThreshold, proximityMultiple)[0] = (VolatilityChannel)prototype;

					Ind.VolatilityChannel(period, volatilityPeriod, volitilityMultiple, marginThreshold, proximityMultiple).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void GetData(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int volatilityPeriod = (int)this.IndicatorParameters.List[1];
			decimal volitilityMultiple = (decimal)this.IndicatorParameters.List[2];
			decimal marginThreshold = (decimal)this.IndicatorParameters.List[3];
			decimal proximityMultiple = (decimal)this.IndicatorParameters.List[4];

			Decimal ema = Ind.EmaPair(period)[0].Fast;
			Decimal volitility = Ind.Volatility(volatilityPeriod)[0].Value;

			(Prototype as VolatilityChannel).Lower = ema - (volitility * volitilityMultiple);
			(Prototype as VolatilityChannel).Mid = ema;
			(Prototype as VolatilityChannel).Upper = ema + (volitility * volitilityMultiple);
			(Prototype as VolatilityChannel).Price = Ind.Bar[0].Close;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0} Low|Mid|High|Oscillation||", this.ToString());
		}

		protected override String GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			VolatilityChannel channel = Data.VolatilityChannel(IndicatorParameters)[Instant.ExposureDate];

			if (channel != null)
				return String.Format("{0}|{1}|{2}|{3}|{4}|", channel.Lower, channel.Mid, channel.Upper, (int)channel.Oscillation, channel.Oscillation);
			else
				return String.Format("{0}|{1}|{2}|{3}|{4}|", "", "", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|{3}|{4}|", "", "", "", "", "");
		}
	}
}
