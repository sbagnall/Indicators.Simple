using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorVolatility : SimulatorOutputBase
	{
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; }
		}

		public override string ToString()
		{
			return String.Format("VOL({0})", this.IndicatorParameters);
		}

		public IndicatorVolatility(int Period)
			: this(new IndicatorParameters(Period))
		{ }

		public IndicatorVolatility(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add(new IndicatorTrueRange((int)this.IndicatorParameters.List[0]));
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			if (!Ind.Volatility(period).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new Volatility();
					else
						prototype = (Volatility)Ind.Volatility(period)[1].Clone();

					Get(ref prototype, Ind);

					Ind.Volatility(period)[0] = (Volatility)prototype;

					Ind.Volatility(period).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int	period = (int)this.IndicatorParameters.List[0];

			decimal volatility = 0.0M;

			if (Ind.Bar.CurrentBar > 1)
				volatility = ((Ind.Volatility(period)[1].Value * (period - 1)) + Ind.TrueRange(period)[0].Value) / (decimal)period;

			(Prototype as Volatility).Value = volatility;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			Volatility vol = Data.Volatility(IndicatorParameters)[Instant.ExposureDate];

			if (vol != null)
				return String.Format("{0}|", vol.Value);
			else
				return String.Format("{0}|", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|", "");
		}
	}
}
