using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
    public abstract class StochasticBase : SimulatorOutputBase
	{
		public override string ToString()
		{
			return String.Format("STOCH({0})", this.IndicatorParameters);
		}

		public StochasticBase(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
		}

		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; }
		}


		public override abstract void Get(ref object Prototype, IIndicatorValues Ind);

		public override abstract void GetAll(IIndicatorValues Ind);

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			if (IndicatorParameters.List.Length == 3)
				return String.Format("Fast STO %K({0})|%D({1})|Oscillation||", (int)IndicatorParameters.List[0], (int)IndicatorParameters.List[1]);
			else
				return String.Format("Slow STO %K({0},{1})|%D({2})|Oscillation||", (int)IndicatorParameters.List[0], (int)IndicatorParameters.List[1], (int)IndicatorParameters.List[2]);
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|{3}|", "", "", "", "");
		}
	}
}
