using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorTrends : SimulatorOutputBase, ITrendDirectionIndicator
	{
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; } 
		}

		public ITrendDirectionValue GetTrendDirectionValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return Ind.Trends(this.IndicatorParameters)[PeriodsAgo];
		}

		public override string ToString()
		{
			return String.Format("TRENDS({0})", this.IndicatorParameters);
		}

		public IndicatorTrends(IDivergentIndicator DivergentIndicator)
			: this(new IndicatorParameters(DivergentIndicator))
		{ }

		public IndicatorTrends(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add((SimulatorOutputBase)IndicatorParameters.List[0]);
		}
		
		public override void GetAll(IIndicatorValues Ind)
		{
			IDivergentIndicator divergentIndicator = (IDivergentIndicator)this.IndicatorParameters.List[0];

			if (!Ind.Trends(divergentIndicator).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new Trends();
					else
						prototype = (Trends)Ind.Trends(divergentIndicator)[1].Clone();

					Get(ref prototype, Ind);

					Ind.Trends(divergentIndicator)[0] = (Trends)prototype;

					Ind.Trends(divergentIndicator).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			IDivergentIndicator divergentIndicator = (IDivergentIndicator)this.IndicatorParameters.List[0];

			(Prototype as Trends).PeakToPeak = divergentIndicator.GetPeakToPeakLine(0, Ind);
			(Prototype as Trends).DipToDip = divergentIndicator.GetDipToDipLine(0, Ind);
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}||", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			Trends trends = Data.Trends(IndicatorParameters)[Instant.ExposureDate];

			if (trends != null)
				return String.Format("{0}|{1}|", (int)trends.Direction, trends.Direction);
			else
				return String.Format("{0}|{1}|", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|", "", "");
		}
	}
}
