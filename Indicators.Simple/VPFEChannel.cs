using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;
using System.Collections.Generic;


namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorVPFEChannel : PriceRangeBarrierIndicatorBase
	{
		#region Barriers

		protected override StudyValue GetStudyValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return new StudyValue(Ind.Bar[PeriodsAgo].High, Ind.Bar[PeriodsAgo].Low, Ind.Bar[PeriodsAgo].Close);
		}

		protected override PriceRangeBarrierPropertiesBase GetPriceRangeBarrierProperties(int PeriodsAgo, IIndicatorValues Ind)
		{
			return Ind.VPFEChannel(this.IndicatorParameters)[PeriodsAgo];
		}

		protected override PriceRangeBarrierPropertiesBase GetPriceRangeBarrierProperties(DateTime Date, IIndicatorValues Ind)
		{
			return Ind.VPFEChannel(this.IndicatorParameters)[Date];
		}

		#endregion


		public override decimal GetProximateDistance(IIndicatorValues Ind, object NextValue)
		{
			decimal proximityMultiple = (decimal)this.IndicatorParameters.List[4];

			return (decimal)Math.Abs((double)(NextValue as VPFEChannel).Upper - (double)(NextValue as VPFEChannel).Lower) * proximityMultiple;
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
			return String.Format("CH.VPFE({0})", this.IndicatorParameters);
		}

		public IndicatorVPFEChannel(int Period, int VolatalityPeriod, decimal VolatilityMultiple, int PFEPeriod, int PFESmoothingPeriod, decimal PFEThresholdPercentage, decimal PFEMultiple, decimal MarginThreshold, decimal ProximityMultiple)
			: this(new IndicatorParameters(Period, VolatalityPeriod, VolatilityMultiple, PFEPeriod, PFESmoothingPeriod, PFEThresholdPercentage, PFEMultiple, MarginThreshold, ProximityMultiple))
		{ }

		public IndicatorVPFEChannel(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add(new IndicatorEMAPairs((int)this.IndicatorParameters.List[0]));
			this.Dependencies.Add(new IndicatorVolatility((int)this.IndicatorParameters.List[1]));
			this.Dependencies.Add(new IndicatorPFE((int)this.IndicatorParameters.List[3], (int)this.IndicatorParameters.List[4], (decimal)this.IndicatorParameters.List[5]));
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int volatilityPeriod = (int)this.IndicatorParameters.List[1];
			Decimal volatilityMultiple = (decimal)this.IndicatorParameters.List[2];
			int pfePeriod = (int)this.IndicatorParameters.List[3];
			int pfeSmoothingPeriod = (int)this.IndicatorParameters.List[4];
			decimal pfeThresholdPercentage = (decimal)this.IndicatorParameters.List[5];
			decimal pfeMultiple = (decimal)this.IndicatorParameters.List[6];
			decimal marginThreshold = (decimal)this.IndicatorParameters.List[7];
			decimal proximityMultiple = (decimal)this.IndicatorParameters.List[8];
			
			if (!Ind.VPFEChannel(period, volatilityPeriod, volatilityMultiple, pfePeriod, pfeSmoothingPeriod, pfeThresholdPercentage, pfeMultiple, marginThreshold, proximityMultiple).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new VPFEChannel(marginThreshold);
					else
						prototype = (VPFEChannel)Ind.VPFEChannel(period, volatilityPeriod, volatilityMultiple, pfePeriod, pfeSmoothingPeriod, pfeThresholdPercentage, pfeMultiple, marginThreshold, proximityMultiple)[1].Clone();

					Get(ref prototype, Ind);

					Ind.VPFEChannel(period, volatilityPeriod, volatilityMultiple, pfePeriod, pfeSmoothingPeriod, pfeThresholdPercentage, pfeMultiple, marginThreshold, proximityMultiple)[0] = (VPFEChannel)prototype;

					Ind.VPFEChannel(period, volatilityPeriod, volatilityMultiple, pfePeriod, pfeSmoothingPeriod, pfeThresholdPercentage, pfeMultiple, marginThreshold, proximityMultiple).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void GetData(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int volatilityPeriod = (int)this.IndicatorParameters.List[1];
			Decimal volitilityMultiple = (decimal)this.IndicatorParameters.List[2];
			int pfePeriod = (int)this.IndicatorParameters.List[3];
			int pfeSmoothingPeriod = (int)this.IndicatorParameters.List[4];
			decimal pfeThresholdPercentage = (decimal)this.IndicatorParameters.List[5];
			decimal pfeMultiple = (decimal)this.IndicatorParameters.List[6];
			decimal marginThreshold = (decimal)this.IndicatorParameters.List[7];
			decimal proximityMultiple = (decimal)this.IndicatorParameters.List[8];

			Decimal ema = Ind.EmaPair(period)[0].Fast;
			Decimal volitility = Ind.Volatility(volatilityPeriod)[0].Value;

			decimal pfe = Math.Abs(Ind.PFE(pfePeriod, pfeSmoothingPeriod, pfeThresholdPercentage)[0].Value);

			(Prototype as VPFEChannel).Lower = ema - ((1.0M - ((pfe / 100M) * pfeMultiple)) * (volitility * volitilityMultiple));
			(Prototype as VPFEChannel).Mid = ema;
			(Prototype as VPFEChannel).Upper = ema + ((1.0M - ((pfe / 100M) * pfeMultiple)) * (volitility * volitilityMultiple));
			(Prototype as VPFEChannel).Price = Ind.Bar[0].Close;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0} Low|Mid|High|Oscillation||", this.ToString());
		}

		protected override String GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			VPFEChannel channel = Data.VPFEChannel(IndicatorParameters)[Instant.ExposureDate];

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
