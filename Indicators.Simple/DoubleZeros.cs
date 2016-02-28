using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.AggregatedLines;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;
using System.Collections.Generic;


namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorDoubleZeros : AggregateLinesIndicatorBase
	{
		private const decimal CONGESTION_FACTOR = 2.0M;
		private const int MAX_LEVELS_ABOVEANDBELOW = 4;

		private const decimal MAX_VALUE = Decimal.MaxValue; // TODO: getting rid of this anyway
		private const decimal MIN_VALUE = Decimal.MinValue; // TODO: getting rid of this anyway
		
		private static readonly List<decimal> AcceptableValues = new List<decimal>(new decimal[] { 10.0M, 25.0M, 50.0M, 100.0M });

		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; }
		}

		protected override decimal MaxValue { get { return MAX_VALUE; } }

		protected override decimal MinValue { get { return MIN_VALUE; } }

		protected override decimal DeleteLinesBarsAgo { get { return 0; } }

		protected override StudyValue GetStudyValue(DateTime Date, IIndicatorValues Ind)
		{
			return GetStudyValue(Ind.Bar.CurrentBar - Ind.Bar[Date].Number, Ind);
		}

		protected override StudyValue GetStudyValue(int PeriodsAgo, IIndicatorValues Ind)
		{
			return new StudyValue(Ind.Bar[PeriodsAgo].High, Ind.Bar[PeriodsAgo].Low, Ind.Bar[PeriodsAgo].Close);
		}

		public override string ToString()
		{
			return String.Format("DZ({0})", this.IndicatorParameters);
		}

		public IndicatorDoubleZeros(int ATRPeriod, decimal ATRMultiple, decimal ProximityFraction)
			: base(new IndicatorParameters(ATRPeriod, ATRMultiple, ProximityFraction))
		{
			this.Dependencies.Add(new IndicatorATR((int)this.IndicatorParameters.List[0]));
		}

		protected override AggregateLines GetLines(DateTime Date, IIndicatorValues Ind)
		{
			return Ind.DoubleZeros(this.IndicatorParameters)[Date];
		}

		protected override AggregateLines GetLines(int PeriodsAgo, IIndicatorValues Ind)
		{
			return Ind.DoubleZeros(this.IndicatorParameters)[PeriodsAgo];
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int atrPeriod = (int)this.IndicatorParameters.List[0];
			decimal atrMultiple = (decimal)this.IndicatorParameters.List[1];
			decimal proximityFraction = (decimal)this.IndicatorParameters.List[2];

			if (!Ind.DoubleZeros(atrPeriod, atrMultiple, proximityFraction).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
			
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new DoubleZeros();
					else
						prototype = (DoubleZeros)Ind.DoubleZeros(atrPeriod, atrMultiple, proximityFraction)[1].Clone();

					Get(ref prototype, Ind);

					Ind.DoubleZeros(atrPeriod, atrMultiple, proximityFraction)[0] = (DoubleZeros)prototype;

					Ind.DoubleZeros(atrPeriod, atrMultiple, proximityFraction).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int atrPeriod = (int)this.IndicatorParameters.List[0];
			decimal atrMultiple = (decimal)this.IndicatorParameters.List[1];
			decimal proximityFraction = (decimal)this.IndicatorParameters.List[2];

			List<IAggregateLine> linesToAdd = new List<IAggregateLine>();

			decimal atr = Ind.ATR(atrPeriod)[0].Value;

			//==================
			// The ATR multiple is likely to increase the range of the lines by twice its value (TODO: a power law of sorts may work better)
			decimal approxSpacing = atr * (atrMultiple * 2.0M);
			//==================

			decimal doubleZeroIncrement = GetIncrement(approxSpacing);

			decimal proximateDistance = doubleZeroIncrement * proximityFraction;

			// TODO: need to deal with congestion differently as the proximity distance will now scale with the distance between lines
			//decimal congestionDistance = (doubleZeroIncrement * proximityFraction) * CONGESTION_FACTOR;
			decimal congestionDistance = 0.0M;

			
			if (doubleZeroIncrement > 0.0M)
			{
				decimal min = Ind.Bar[0].Close - (doubleZeroIncrement * MAX_LEVELS_ABOVEANDBELOW);
				decimal max = Ind.Bar[0].Close + (doubleZeroIncrement * MAX_LEVELS_ABOVEANDBELOW);
				decimal value = 0.0M;

				while (value < min)
					value += doubleZeroIncrement;

				for (; value <= max; value += doubleZeroIncrement)
				{
					linesToAdd.Add(new PriceLevel(
						Ind.Bar.CurrentBar,
						value,
						true,
						0,
						this.FailureDistanceMultiple));
				}
			}

			base.CleanLines(ref Prototype, Ind);

			foreach (IAggregateLine line in linesToAdd)
				(Prototype as AggregateLines).Lines.Add(line);

			// update thresholds and proximate distance / no need to update vertical threshold for double zeros
			foreach (IAggregateLine line in (Prototype as AggregateLines).Lines)
			{
				line.ProximateDistance = proximateDistance;
				line.CongestionDistance = congestionDistance;
			}

			// get congestion // TODO: does this work as expected
			AggregateLines lines = (Prototype as AggregateLines);
			GetCongestion(ref lines, Ind.Bar[0].Number);
			Prototype = lines;
		}
	
		// TODO: this seems like a roundabout way to do this
		private decimal GetIncrement(decimal ApproximateSpacing)
		{
			int roundingPosition;
			decimal scale;
			decimal round = Utilities.RoundToSigFigs(ApproximateSpacing, 1, out roundingPosition, out scale);

			decimal doubleZeroIncrement = 0.0M;

			foreach (decimal acceptableValue in AcceptableValues)
			{
				if ((Utilities.RoundToFraction(round, 1.0m * (1.0M / (decimal)Math.Pow(10.0, roundingPosition)))
					* (decimal)Math.Pow(10.0, roundingPosition + 1)) == acceptableValue)
				{
					doubleZeroIncrement = Utilities.RoundToFraction(
						round, 1.0m * (1.0M / (decimal)Math.Pow(10.0, roundingPosition)));
					break;
				}
				else if ((Utilities.RoundToFraction(round, 2.5m * (1.0M / (decimal)Math.Pow(10.0, roundingPosition)))
					* (decimal)Math.Pow(10.0, roundingPosition + 1)) == acceptableValue)
				{
					doubleZeroIncrement = Utilities.RoundToFraction(
						round, 2.5m * (1.0M / (decimal)Math.Pow(10.0, roundingPosition)));
					break;
				}
				else if ((Utilities.RoundToFraction(round, 5.0m * (1.0M / (decimal)Math.Pow(10.0, roundingPosition)))
					* (decimal)Math.Pow(10.0, roundingPosition + 1)) == acceptableValue)
				{
					doubleZeroIncrement = Utilities.RoundToFraction(
						round, 5.0m * (1.0M / (decimal)Math.Pow(10.0, roundingPosition)));
					break;
				}
				else if ((Utilities.RoundToFraction(round, 10.0m * (1.0M / (decimal)Math.Pow(10.0, roundingPosition)))
					* (decimal)Math.Pow(10.0, roundingPosition + 1) == acceptableValue))
				{
					doubleZeroIncrement = Utilities.RoundToFraction(
						round, 10.0m * (1.0M / (decimal)Math.Pow(10.0, roundingPosition)));
					break;
				}
			}

			return doubleZeroIncrement;
		}
	}
}
