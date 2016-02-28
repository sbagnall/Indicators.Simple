using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteveBagnall.Trading.Indicators
{


    public class IndicatorMomentumAbsolute : SimulatorOutputBase, IStudyIndicator, ITrendDirectionIndicator, IRenkoStudyIndicator
	{
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; }
		}

		public ITrendDirectionValue GetTrendDirectionValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return Ind.MomentumAbsolute(this.IndicatorParameters)[PeriodsAgo];
		}
		
		#region Study

		public StudyValue GetStudyValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return new StudyValue(Ind.MomentumAbsolute(this.IndicatorParameters)[PeriodsAgo].Value);
		}


		public StudyValue GetStudyValue(DateTime Date, IIndicatorValues Ind)
		{
			return new StudyValue(Ind.MomentumAbsolute(this.IndicatorParameters)[Date].Value);
		}

		#endregion

		#region Renko

		public decimal GetRange(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			List<decimal> ranges = new List<decimal>();

			if (Ind.Bar[0].Number < (period + 1))
				return 0.0M;

			for (int i = 0; i < period; i++)
				ranges.Add((decimal)Math.Abs((double)Ind.MomentumAbsolute(period)[i].Value - (double)Ind.MomentumAbsolute(period)[i + 1].Value));

			return ranges.Average();
		}

		#endregion

		public override string ToString()
		{
			return String.Format("A.MOM({0})", this.IndicatorParameters);
		}

		public IndicatorMomentumAbsolute(int Period)
			: this(new IndicatorParameters(Period))
		{ }

		public IndicatorMomentumAbsolute(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{ }

		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];

			if (!Ind.MomentumAbsolute(period).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new MomentumAbsolute();
					else
						prototype = (MomentumAbsolute)Ind.MomentumAbsolute(period)[1].Clone();

					Get(ref prototype, Ind);

					Ind.MomentumAbsolute(period)[0] = (MomentumAbsolute)prototype;

					Ind.MomentumAbsolute(period).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int	period = (int)this.IndicatorParameters.List[0];

			decimal momentum = 0.0M;

			if (Ind.Bar.CurrentBar > (period))
				momentum = (Ind.Bar[0].Close - Ind.Bar[period].Close);

			(Prototype as MomentumAbsolute).Value = momentum;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|Direction||", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			MomentumAbsolute mom = Data.MomentumAbsolute(IndicatorParameters)[Instant.ExposureDate];

			if (mom != null)
				return String.Format("{0}|{1}|{2}|", mom.Value, (int)mom.Direction, mom.Direction);
			else
				return String.Format("{0}|{1}|{2}|", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|", "", "", "");
		}
	}
}
