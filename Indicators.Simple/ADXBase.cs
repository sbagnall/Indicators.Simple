using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
   public abstract class ADXBase : SimulatorOutputBase
    {
		private const int TRUE_RANGE_PERIOD = 1;

		protected int TrueRangePeriod { get { return TRUE_RANGE_PERIOD; } }

		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; }
		}

		public ADXBase(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add(new IndicatorTrueRange(TRUE_RANGE_PERIOD));
		}
		
		//public override abstract ISignal Signal { get; }

		public override abstract void Get(ref object Prototype, IIndicatorValues Ind);

		public override abstract void GetAll(IIndicatorValues Ind);

		protected override abstract String GetIndicatorHeader(IndicatorParameters IndicatorParameters);

		protected override abstract String GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters);

		protected override abstract String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters);
	
	}
}
