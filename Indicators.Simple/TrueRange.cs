using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorTrueRange : SimulatorOutputBase
	{
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; }
		}

		public override string ToString()
		{
			return String.Format("TR({0})", this.IndicatorParameters);
		}

		public IndicatorTrueRange(int Period)
			: this(new IndicatorParameters(Period))
		{ }

		public IndicatorTrueRange(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
		}
	
		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			if (!Ind.TrueRange(period).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new TrueRange();
					else
						prototype = (TrueRange)Ind.TrueRange(period)[1].Clone();

					Get(ref prototype, Ind);

					Ind.TrueRange(period)[0] = (TrueRange)prototype;

					Ind.TrueRange(period).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			Decimal tr = 0.0M;

			if (Ind.Bar.CurrentBar > (period + 1))
			{
				List<OHLCV> prices = new List<OHLCV>();
				for (int i = 0; i < period; i++)
					prices.Add(Ind.Bar[i].OHLCV);
			
				tr = Math.Max(prices.Max(p => p.High) - prices.Min(p => p.Low), 
					Math.Max(Math.Abs(prices.Max(p => p.High) - Ind.Bar[period + 1].Close), 
					Math.Abs(prices.Min(p => p.Low) - Ind.Bar[period + 1].Close)));
			}

			(Prototype as TrueRange).Value = tr;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("TR({0})|", IndicatorParameters);
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			TrueRange tr = Data.TrueRange(IndicatorParameters)[Instant.ExposureDate];

			if (tr != null)
				return String.Format("{0}|", tr.Value);
			else
				return String.Format("{0}|", "");
		}
		
		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|", "");
		}
	}
}
