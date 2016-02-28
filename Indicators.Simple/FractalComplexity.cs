using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;
using System.Collections.Generic;


namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorFractalComplexity : SimulatorOutputBase
	{
		
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; }
		}


		public override string ToString()
		{
			return String.Format("F.CMPLX({0})", this.IndicatorParameters);
		}

		public IndicatorFractalComplexity(int Period, int SmoothingPeriod)
			: this(new IndicatorParameters(Period, SmoothingPeriod))
		{ }

		public IndicatorFractalComplexity(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			
		}

		//public override decimal GetProximateDistance(int PeriodsAgo, IIndicatorValues Ind)
		//{
		//    int period = (int)this.IndicatorParameters.List[0];
		//    int smoothingPeriod = (int)this.IndicatorParameters.List[1];

		//    List<decimal> ranges = new List<decimal>();
		//    for (int i = PeriodsAgo; i < (PeriodsAgo + period); i++)
		//        ranges.Add((decimal)Math.Abs((double)Ind.Complexity(period, smoothingPeriod)[i].FractalComplexity - (double)Ind.Complexity(period, smoothingPeriod)[i].FractalComplexity));

		//    return ranges.Average(); ;
		//}

		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int smoothingPeriod = (int)this.IndicatorParameters.List[1];

			if (!Ind.Complexity(period, smoothingPeriod).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new Complexity();
					else
						prototype = (Complexity)Ind.Complexity(period, smoothingPeriod)[1].Clone();

					Get(ref prototype, Ind);

					Ind.Complexity(period, smoothingPeriod)[0] = (Complexity)prototype;

					Ind.Complexity(period, smoothingPeriod).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int smoothingPeriod = (int)this.IndicatorParameters.List[1];

			decimal complexPath = (decimal)period;
			decimal directPath = (decimal)period;
			Decimal alpha = 2.0M / (smoothingPeriod + 1.0M);

			Decimal tmpComplexity = 1.0M;
			if (Ind.Bar.CurrentBar > (period + 1))
			{
				complexPath = 0.0M;

				for (int i = 0; i < period; i++)
					complexPath += (decimal)Math.Sqrt(Math.Pow((double)(Ind.Bar[i].Close - Ind.Bar[i + 1].Close), 2) + Math.Pow((double)Ind.Market.PipValue, 2));

				directPath = (decimal)Math.Sqrt(Math.Pow((double)(Ind.Bar[0].Close - Ind.Bar[period].Close), 2) + Math.Pow((double)Ind.Market.PipValue * (double)period, 2));
			}

			tmpComplexity = (complexPath / directPath);

			decimal complexity = tmpComplexity;
			if (Ind.Bar.CurrentBar > 1)
				complexity = (alpha * tmpComplexity) + ((1.0M - alpha) * Ind.Complexity(period, smoothingPeriod)[1].FractalComplexity);

			(Prototype as Complexity).ComplexPath = complexPath;
			(Prototype as Complexity).DirectPath = directPath;
			(Prototype as Complexity).FractalComplexity = complexity;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			Complexity fc = Data.Complexity(IndicatorParameters)[Instant.ExposureDate];

			if (fc != null)
				return String.Format("{0}|", fc.FractalComplexity);
			else
				return String.Format("{0}|", "");
		}


		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|", "");
		}
	}
}
