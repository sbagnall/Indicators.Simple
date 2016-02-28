using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values.AggregatedLines;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteveBagnall.Trading.Indicators
{
    public abstract class AggregateLinesIndicatorBase : SimulatorOutputBase, IBarrierIndicator, IParametized
	{
		#region Parameters

		private static String FAILURE_DISTANCE_MULTIPLE = "AGGREGATELINESINDICATORBASE_FAILURE_DISTANCE_MULTIPLE";

		public virtual Parameters Parameters
		{
			get
			{
				Parameters parameters = new Parameters();

				parameters.Add(new Parameter(
					
					FAILURE_DISTANCE_MULTIPLE,
					typeof(decimal),
					2.5M,
					false, // IsTesting
					1.5M,
					3.0M,
					TestSequence.Arithmatic));

				return parameters;
			}
		}

		#endregion

		public decimal FailureDistanceMultiple { get { return Convert.ToDecimal(ParameterEngine.Instance.CurrentParameters.Get(FAILURE_DISTANCE_MULTIPLE).Value); } }

		private const int NUM_LEVELS_ABOVEANDBELOW = 2;

		public AggregateLinesIndicatorBase(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			ParameterEngine.Instance.Register(this);
		}

		protected abstract decimal MaxValue { get; }

		protected abstract decimal MinValue { get; }

		protected abstract decimal DeleteLinesBarsAgo { get; }

		protected abstract StudyValue GetStudyValue(int PeriodsAgo, IIndicatorValues Ind);

		protected abstract StudyValue GetStudyValue(DateTime Date, IIndicatorValues Ind);

		protected abstract AggregateLines GetLines(int PeriodsAgo, IIndicatorValues Ind);

		protected abstract AggregateLines GetLines(DateTime Date, IIndicatorValues Ind);

		protected void CleanLines(ref object Prototype, IIndicatorValues Ind)
		{
			List<IAggregateLine> currentLines = (Prototype as AggregateLines).Lines;

			(Prototype as AggregateLines).Lines = new List<IAggregateLine>();

			foreach (IAggregateLine line in currentLines)
			{
				if ((line.GetValueAt(Ind.Bar[0].Number) < this.MaxValue)
					&& (line.GetValueAt(Ind.Bar[0].Number) > this.MinValue)
					&& (line.BarNumberIdentified > (Ind.Bar[0].Number - this.DeleteLinesBarsAgo)))
				{
					(Prototype as AggregateLines).Lines.Add(line);
				}
			}
		}

		public List<IBarrierValue> GetBarriers(int PeriodsAgo, IIndicatorValues Ind)
		{
			return GetLines(PeriodsAgo, Ind).Lines.Cast<IBarrierValue>().ToList();
		}

		private Dictionary<BarrierDirection, List<IAggregateLine>> GetLinesOfSameDirection(AggregateLines Lines)
		{
			Dictionary<BarrierDirection, List<IAggregateLine>> linesOfSameDirection = new Dictionary<BarrierDirection, List<IAggregateLine>>();

			foreach (IAggregateLine line in Lines.Lines)
			{
				if (!linesOfSameDirection.ContainsKey(line.BarrierDirection))
					linesOfSameDirection.Add(line.BarrierDirection, new List<IAggregateLine>());

				linesOfSameDirection[line.BarrierDirection].Add(line);
			}

			return linesOfSameDirection;
		}

		public Dictionary<BarrierDirection, decimal> GetSpacingAroundTarget(AggregateLines Lines, int BarNumber, decimal TargetValue)
		{
			Dictionary<BarrierDirection, decimal> directionSpacing = new Dictionary<BarrierDirection, decimal>();

			Dictionary<BarrierDirection, List<IAggregateLine>> linesOfSameDirection = GetLinesOfSameDirection(Lines);

			foreach (BarrierDirection key in linesOfSameDirection.Keys)
			{
				decimal? previousLineValue = null;
				decimal? spacing = null;
				bool isTargetReached = false;

				foreach (IAggregateLine line in linesOfSameDirection[key].OrderBy(b => b.GetValueAt(BarNumber)))
				{
					decimal lineCurrentValue = line.GetValueAt(BarNumber);
					if (previousLineValue == null)
					{
						if (lineCurrentValue > TargetValue)
						{
							isTargetReached = true;
							break;
						}

						previousLineValue = lineCurrentValue;
					}
					else
					{
						spacing = lineCurrentValue - previousLineValue;

						if (lineCurrentValue > TargetValue)
						{
							isTargetReached = true;
							break;
						}

						previousLineValue = lineCurrentValue;
					}
				}

				if ((spacing != null) && isTargetReached)
					directionSpacing.Add(key, spacing ?? 0.0M);
			}

			return directionSpacing;
		}

		/// <summary>
		/// Sets IsCongested and CongestedBoundary on the passed lines
		/// </summary>
		/// <param name="Lines"></param>
		/// <param name="BarNumber"></param>
		public void GetCongestion(ref AggregateLines Lines, int BarNumber)
		{
			Dictionary<BarrierDirection, List<IAggregateLine>> linesOfSameDirection = GetLinesOfSameDirection(Lines);

			foreach (BarrierDirection key in linesOfSameDirection.Keys)
			{
				decimal currentLimit = Decimal.MinValue;
				IAggregateLine previousLine = null;

				bool isCongested = false;
				bool isPreviousSetToLower = false;
				bool isPreviousSetToUpper = false;

				foreach (IAggregateLine line in linesOfSameDirection[key].OrderBy(b => b.GetValueAt(BarNumber)))
				{
					isPreviousSetToLower = false;
					isPreviousSetToUpper = false;
					decimal lineCurrentValue = line.GetValueAt(BarNumber);

					//if ((lineCurrentValue - line.ProximateDistance) <= currentLimit)
					if ((lineCurrentValue - line.CongestionDistance) <= currentLimit)
					{
						if (!isCongested)
							isPreviousSetToLower = true;

						isCongested = true;
					}
					else
					{
						if (isCongested)
						{
							isPreviousSetToUpper = true;
							isCongested = false;
						}
					}

					line.IsCongested = isCongested;
					line.CongestedBoundary = CongestionBoundary.NotSet;

					if (isPreviousSetToLower)
					{
						previousLine.CongestedBoundary = CongestionBoundary.Lower;
						previousLine.IsCongested = true;
					}

					if (isPreviousSetToUpper)
					{
						previousLine.CongestedBoundary = CongestionBoundary.Upper;
						previousLine.IsCongested = true;
					}

					//currentLimit = lineCurrentValue + line.ProximateDistance;
					currentLimit = lineCurrentValue + line.CongestionDistance;
					previousLine = line;
				}

				// if the last line was congested then it becomes a boundary
				if (isCongested)
				{
					previousLine.CongestedBoundary = CongestionBoundary.Upper;
					previousLine.IsCongested = true;
				}

			}
		}
	
		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(this.ToString());

			for (int i = 0; i < NUM_LEVELS_ABOVEANDBELOW; i++)
				sb.AppendFormat("A({0})|Proximity|Tests|", i + 1);

			for (int i = 0; i < NUM_LEVELS_ABOVEANDBELOW; i++)
				sb.AppendFormat("B({0})|Proximity|Tests|", i + 1);

			sb.AppendFormat("Bnd A U|Bnd A L|Bnd B U|Bnd B L|");

			return sb.ToString();
		}

		protected override String GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			StringBuilder sb = new StringBuilder();
			int index = 0;

			foreach (IBarrierValue barrier in this.GetLines(Instant.ExposureDate, (Data as IIndicatorValues)).GetClosestBarrierAbove(
				GetStudyValue(Instant.ExposureDate, (Data as IIndicatorValues)),
				(Data as IIndicatorValues).Bar[Instant.ExposureDate].Number,
				false,
				TrendType.NotSet,
				false, 
				CongestionBoundary.NotSet,
				0))
			{
				decimal value = barrier.GetValueAt((Data as IIndicatorValues).Bar.CurrentBar);

				if (index < NUM_LEVELS_ABOVEANDBELOW)
					sb.AppendFormat("{0}|{1}|{2}|", value, value - (barrier as IAggregateLine).ProximateDistance, (barrier as IAggregateLine).NumTests);
				else
					break;

				index++;
			}

			while (index < NUM_LEVELS_ABOVEANDBELOW)
			{
				sb.AppendFormat("{0}|{1}|{2}|", "", "", "");
				index++;
			}

			index = 0;
			foreach (IBarrierValue barrier in this.GetLines(Instant.ExposureDate, (Data as IIndicatorValues)).GetClosestBarrierBelow(
				GetStudyValue(Instant.ExposureDate, (Data as IIndicatorValues)),
				(Data as IIndicatorValues).Bar[Instant.ExposureDate].Number,
				false,
				TrendType.NotSet,
				false, 
				CongestionBoundary.NotSet,
				0))
			{
				decimal value = barrier.GetValueAt((Data as IIndicatorValues).Bar.CurrentBar);

				if (index < NUM_LEVELS_ABOVEANDBELOW)
					sb.AppendFormat("{0}|{1}|{2}|", value, value + (barrier as IAggregateLine).ProximateDistance, (barrier as IAggregateLine).NumTests);
				else
					break;

				index++;
			}

			while (index < NUM_LEVELS_ABOVEANDBELOW)
			{
				sb.AppendFormat("{0}|{1}|{2}|", "", "", "");
				index++;
			}

			index = 0;
			foreach (IBarrierValue barrier in this.GetLines(Instant.ExposureDate, (Data as IIndicatorValues)).GetClosestBarrierAbove(
				GetStudyValue(Instant.ExposureDate, (Data as IIndicatorValues)),
				(Data as IIndicatorValues).Bar[Instant.ExposureDate].Number,
				false,
				TrendType.NotSet,
				true,
				CongestionBoundary.Upper,
				0))
			{
				decimal value = barrier.GetValueAt((Data as IIndicatorValues).Bar.CurrentBar);

				if (index < 1)
					sb.AppendFormat("{0}|", value);
				else
					break;

				index++;
			}

			while (index < 1)
			{
				sb.AppendFormat("{0}|", "");
				index++;
			}

			index = 0;
			foreach (IBarrierValue barrier in this.GetLines(Instant.ExposureDate, (Data as IIndicatorValues)).GetClosestBarrierAbove(
				GetStudyValue(Instant.ExposureDate, (Data as IIndicatorValues)),
				(Data as IIndicatorValues).Bar[Instant.ExposureDate].Number,
				false,
				TrendType.NotSet,
				true,
				CongestionBoundary.Lower,
				0))
			{
				decimal value = barrier.GetValueAt((Data as IIndicatorValues).Bar.CurrentBar);

				if (index < 1)
					sb.AppendFormat("{0}|", value);
				else
					break;

				index++;
			}

			while (index < 1)
			{
				sb.AppendFormat("{0}|", "");
				index++;
			}

			index = 0;
			foreach (IBarrierValue barrier in this.GetLines(Instant.ExposureDate, (Data as IIndicatorValues)).GetClosestBarrierBelow(
				GetStudyValue(Instant.ExposureDate, (Data as IIndicatorValues)),
				(Data as IIndicatorValues).Bar[Instant.ExposureDate].Number,
				false,
				TrendType.NotSet,
				true,
				CongestionBoundary.Upper,
				0))
			{
				decimal value = barrier.GetValueAt((Data as IIndicatorValues).Bar.CurrentBar);

				if (index < 1)
					sb.AppendFormat("{0}|", value);
				else
					break;

				index++;
			}

			while (index < 1)
			{
				sb.AppendFormat("{0}|", "");
				index++;
			}

			index = 0;
			foreach (IBarrierValue barrier in this.GetLines(Instant.ExposureDate, (Data as IIndicatorValues)).GetClosestBarrierBelow(
				GetStudyValue(Instant.ExposureDate, (Data as IIndicatorValues)),
				(Data as IIndicatorValues).Bar[Instant.ExposureDate].Number,
				false,
				TrendType.NotSet,
				true,
				CongestionBoundary.Lower,
				0))
			{
				decimal value = barrier.GetValueAt((Data as IIndicatorValues).Bar.CurrentBar);

				if (index < 1)
					sb.AppendFormat("{0}|", value);
				else
					break;

				index++;
			}

			while (index < 1)
			{
				sb.AppendFormat("{0}|", "");
				index++;
			}

			return sb.ToString();
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < NUM_LEVELS_ABOVEANDBELOW; i++)
				sb.AppendFormat("{0}|{1}|{2}|", "", "", "");

			sb.AppendFormat("{0}|{1}|{2}|{3}|", "", "", "", "");

			return sb.ToString();
		}
	}
}
