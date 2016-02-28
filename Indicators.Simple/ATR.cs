using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorATR : SimulatorOutputBase 
	{
		private const int TRUE_RANGE_PERIOD = 1;

		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; }
		}

		
		public override string ToString()
		{
			return String.Format("ATR({0})", this.IndicatorParameters);
		}
		
		public IndicatorATR(int Period)
			: this(new IndicatorParameters(Period))
		{ }

		public IndicatorATR(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add(new IndicatorTrueRange(TRUE_RANGE_PERIOD));
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			if (!Ind.ATR(period).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new ATR();
					else
						prototype = (ATR)Ind.ATR(period)[1].Clone();

					Get(ref prototype, Ind);

					Ind.ATR(period)[0] = (ATR)prototype;

					Ind.ATR(period).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			Decimal atr = 0.0M;

			if (Ind.Bar.CurrentBar > 1)
				atr = ((Ind.ATR(period)[1].Value * (period - 1)) + Ind.TrueRange(TRUE_RANGE_PERIOD)[0].Value) / period;

			(Prototype as ATR).Value = atr;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			ATR atr = Data.ATR(IndicatorParameters)[Instant.ExposureDate];
			if (atr != null)
				return String.Format("{0}|", atr.Value);
			else
				return String.Format("{0}|", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|", "");
		}
	}
}
