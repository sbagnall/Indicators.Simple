﻿using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorStochasticSlow : StochasticBase
	{
		public IndicatorStochasticSlow(int Period, int FirstSmoothingPeriod, int? SecondSmoothingPeriod, decimal ThresholdPercentage)
			: this(new IndicatorParameters(Period, FirstSmoothingPeriod, SecondSmoothingPeriod, ThresholdPercentage))
		{ }

		public IndicatorStochasticSlow(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int firstSmoothingPeriod = (int)this.IndicatorParameters.List[1];
			int secondSmoothingPeriod = (int)this.IndicatorParameters.List[2];
			decimal thresholdPercentage = (decimal)this.IndicatorParameters.List[3];

			if (!Ind.SlowStochastic(period, firstSmoothingPeriod, secondSmoothingPeriod, thresholdPercentage).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new Stochastic(thresholdPercentage);
					else
						prototype = (Stochastic)Ind.SlowStochastic(period, firstSmoothingPeriod, secondSmoothingPeriod, thresholdPercentage)[1].Clone();

					Get(ref prototype, Ind);

					Ind.SlowStochastic(period, firstSmoothingPeriod, secondSmoothingPeriod, thresholdPercentage)[0] = (Stochastic)prototype;

					Ind.SlowStochastic(period, firstSmoothingPeriod, secondSmoothingPeriod, thresholdPercentage).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			int firstSmoothingPeriod = (int)this.IndicatorParameters.List[1];
			int secondSmoothingPeriod = (int)this.IndicatorParameters.List[2];
			decimal thresholdPercentage = (decimal)this.IndicatorParameters.List[3];
	
			decimal fastPercK = 50.0M;
			decimal percK = 50.0M;
			decimal percD = 50.0M;

			if (Ind.Bar.CurrentBar > (period + 1))
			{
				decimal lowestLow = Decimal.MaxValue;
				decimal highestHigh = Decimal.MinValue;

				for (int i = 0; i < period; i++)
				{
					lowestLow = (Ind.Bar[i].Low < lowestLow) ? Ind.Bar[i].Low : lowestLow;
					highestHigh = (Ind.Bar[i].High > highestHigh) ? Ind.Bar[i].High : highestHigh;
				}

				fastPercK = ((Ind.Bar[0].Close - lowestLow) / (highestHigh - lowestLow)) * 100.0M;
			}

			decimal totalFastPercK = fastPercK;
			if (Ind.Bar.CurrentBar > (period + 1 + firstSmoothingPeriod))
			{

				for (int i = 1; i < firstSmoothingPeriod; i++)
					totalFastPercK += Ind.SlowStochastic(period, firstSmoothingPeriod, secondSmoothingPeriod, thresholdPercentage)[i].FastK;

				percK = totalFastPercK / (decimal)firstSmoothingPeriod;
			}
			else
			{
				percK = fastPercK;
			}

			decimal totalPercK = percK;
			if (Ind.Bar.CurrentBar > (period + 1 + secondSmoothingPeriod))
			{

				for (int i = 1; i < secondSmoothingPeriod; i++)
					totalPercK += Ind.SlowStochastic(period, firstSmoothingPeriod, secondSmoothingPeriod, thresholdPercentage)[i].PercentageK;

				percD = totalPercK / (decimal)secondSmoothingPeriod;
			}
			else
			{
				percD = percK;
			}

			(Prototype as Stochastic).FastK = fastPercK;
			(Prototype as Stochastic).PercentageK = percK;
			(Prototype as Stochastic).PercentageD = percD;
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			Stochastic stochastic = Data.SlowStochastic(IndicatorParameters)[Instant.ExposureDate];
			if (stochastic != null)
				return String.Format("{0}|{1}|{2}|{3}|", stochastic.PercentageK, stochastic.PercentageD, (int)stochastic.Oscillation, stochastic.Oscillation);
			else
				return String.Format("{0}|{1}|{2}|{3}|", "", "", "", "");
		}
	}
}
