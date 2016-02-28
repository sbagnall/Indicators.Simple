using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorMomentumRelative : SimulatorOutputBase, ITrendDirectionIndicator
	{
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; } 
		}

		public ITrendDirectionValue GetTrendDirectionValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return Ind.MomentumRelative(this.IndicatorParameters)[PeriodsAgo];
		}

		public override string ToString()
		{
			return String.Format("R.MOM({0})", this.IndicatorParameters);
		}

		public IndicatorMomentumRelative(int Period)
			: this(new IndicatorParameters(Period))
		{ }

		public IndicatorMomentumRelative(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add(new IndicatorMomentumAbsolute((int)this.IndicatorParameters.List[0]));
		}
		
		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			if (!Ind.MomentumRelative(period).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new MomentumRelative();
					else
						prototype = (MomentumRelative)Ind.MomentumRelative(period)[1].Clone();

					Get(ref prototype, Ind);

					Ind.MomentumRelative(period)[0] = (MomentumRelative)prototype;

					Ind.MomentumRelative(period).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int	period = (int)this.IndicatorParameters.List[0];

			decimal relativeMomentum = 0.0M;
			if (Ind.Bar.CurrentBar > period)
				relativeMomentum = (Ind.MomentumAbsolute(period)[0].Value - Ind.MomentumAbsolute(period)[period].Value);

			(Prototype as MomentumRelative).Value = relativeMomentum;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|Direction||", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			MomentumRelative momRelative = Data.MomentumRelative(IndicatorParameters)[Instant.ExposureDate];

			if (momRelative != null)
				return String.Format("{0}|{1}|{2}|", momRelative.Value, (int)momRelative.Direction, momRelative.Direction);
			else
				return String.Format("{0}|{1}|{2}|", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|", "", "", "");
		}
	}
}
