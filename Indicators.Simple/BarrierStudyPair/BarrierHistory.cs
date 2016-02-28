using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SteveBagnall.Trading.Indicators.BarrierStudyPair
{
    public class BarrierHistory : ICloneable
	{
		public class EventPattern
		{
			public StateType StateType;
			public EventType EventType;
			public Direction Direction;
			public Regex Pattern;

			public EventPattern(StateType StateType, EventType EventType, Direction Direction, Regex Pattern)
			{
				this.StateType = StateType;
				this.EventType = EventType;
				this.Direction = Direction;
				//this.Pattern = new Regex(Pattern, RegexOptions.Compiled);
				this.Pattern = Pattern;
			}
		}

		public enum StateType
		{
			NotSet = 0,
			Position,
			Range,
		}

		public enum Position
		{
			Above = 2,				// A
			UpperFailRange = 1,		// U
			Proximate = 0,			// P
			LowerFailRange = -1,	// L
			Below = -2,				// B
		}

		private static List<KeyValuePair<Position, char>> AllPositionStates = new List<KeyValuePair<Position, char>>(new KeyValuePair<Position, char>[] {
			new KeyValuePair<Position,char>(Position.Above,				'A'),
			new KeyValuePair<Position,char>(Position.UpperFailRange,	'U'),
			new KeyValuePair<Position,char>(Position.Proximate,			'P'),
			new KeyValuePair<Position,char>(Position.LowerFailRange,	'L'),
			new KeyValuePair<Position,char>(Position.Below,				'B')});

		//private const string APPROACH_UP_PATTERN =			"[BL]P$";
		//private const string APPROACH_DOWN_PATTERN =		"[AU]P$";
		//private const string BREAKS_UP_PATTERN =			"[BL]P?[UA]$";
		//private const string BREAKS_DOWN_PATTERN =			"[AU]P?[LB]$";
		//private const string BOUNCES_UP_PATTERN =			"[AU]P[AU]$";
		//private const string BOUNCES_DOWN_PATTERN =			"[BL]P[BL]$";
		//private const string BOUNCE_FAILED_UP_PATTERN =		"[BL]PL[PL]*[AU]$";
		//private const string BOUNCE_FAILED_DOWN_PATTERN =	"[AU]PU[PU]*[LB]$";
		//private const string BREAK_FAILED_UP_PATTERN =		"[AU]P?L[PL]*[AU]$";
		//private const string BREAK_FAILED_DOWN_PATTERN =	"[BL]P?U[PU]*[LB]$";

		private static Regex APPROACH_UP_PATTERN = new Regex("[BL]P$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex APPROACH_DOWN_PATTERN = new Regex("[AU]P$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex BREAKS_UP_PATTERN = new Regex("[BL]P?[UA]$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex BREAKS_DOWN_PATTERN = new Regex("[AU]P?[LB]$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex BOUNCES_UP_PATTERN = new Regex("[AU]P[AU]$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex BOUNCES_DOWN_PATTERN = new Regex("[BL]P[BL]$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex BOUNCE_FAILED_UP_PATTERN = new Regex("[BL]PL[PL]*[AU]$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex BOUNCE_FAILED_DOWN_PATTERN = new Regex("[AU]PU[PU]*[LB]$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex BREAK_FAILED_UP_PATTERN = new Regex("[AU]P?L[PL]*[AU]$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex BREAK_FAILED_DOWN_PATTERN = new Regex("[BL]P?U[PU]*[LB]$", RegexOptions.Compiled | RegexOptions.RightToLeft);

		public enum Range
		{
			CloseAndLowAbove = 2,		// A
			StraddlesCloseAbove = 1,	// H
			StraddlesCloseBelow = -1,	// L
			CloseAndHighBelow = -2,		// B
		}

		private static List<KeyValuePair<Range, char>> AllRangeStates = new List<KeyValuePair<Range, char>>(new KeyValuePair<Range, char>[] {
			new KeyValuePair<Range,char>(Range.CloseAndLowAbove,		'A'),
			new KeyValuePair<Range,char>(Range.StraddlesCloseAbove,		'H'),
			new KeyValuePair<Range,char>(Range.StraddlesCloseBelow,		'L'),
			new KeyValuePair<Range,char>(Range.CloseAndHighBelow,		'B')});

		//private const string CLOSECROSSES_UP_PATTERN =		"[LB][AH]$";
		//private const string CLOSECROSSES_DOWN_PATTERN =	"[AH][LB]$";
		//private const string RANGECROSSES_UP_PATTERN =		"[HLB]A$";
		//private const string RANGECROSSES_DOWN_PATTERN =	"[AHL]B$";
		//private const string RANGESTRADDLES_UP_PATTERN =	"B[HL]$";
		//private const string RANGESTRADDLES_DOWN_PATTERN =	"A[HL]$";

		private static Regex CLOSECROSSES_UP_PATTERN = new Regex("[LB][AH]$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex CLOSECROSSES_DOWN_PATTERN = new Regex("[AH][LB]$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex RANGECROSSES_UP_PATTERN = new Regex("[HLB]A$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex RANGECROSSES_DOWN_PATTERN = new Regex("[AHL]B$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex RANGESTRADDLES_UP_PATTERN = new Regex("B[HL]$", RegexOptions.Compiled | RegexOptions.RightToLeft);
		private static Regex RANGESTRADDLES_DOWN_PATTERN = new Regex("A[HL]$", RegexOptions.Compiled | RegexOptions.RightToLeft);


		public static Dictionary<EventPatternKey, EventPattern> EventPatterns { get; private set; }

		public IBarrierValue Barrier { get; private set; }
		public string PositionStates { get; private set; }
		public string RangeStates { get; private set; }

		private Dictionary<EventType, int> LastMatchedLengths { get; set; }

		static BarrierHistory()
		{
			EventPatterns = new Dictionary<EventPatternKey, EventPattern>();
			EventPatterns.Add(new EventPatternKey(EventType.Approach, Direction.Up), new EventPattern(StateType.Position, EventType.Approach, Direction.Up, APPROACH_UP_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.Approach, Direction.Down), new EventPattern(StateType.Position, EventType.Approach, Direction.Down, APPROACH_DOWN_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.Bounce, Direction.Up), new EventPattern(StateType.Position, EventType.Bounce, Direction.Up, BOUNCES_UP_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.Bounce, Direction.Down), new EventPattern(StateType.Position, EventType.Bounce, Direction.Down, BOUNCES_DOWN_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.Break, Direction.Up), new EventPattern(StateType.Position, EventType.Break, Direction.Up, BREAKS_UP_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.Break, Direction.Down), new EventPattern(StateType.Position, EventType.Break, Direction.Down, BREAKS_DOWN_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.FailedBounce, Direction.Up), new EventPattern(StateType.Position, EventType.FailedBounce, Direction.Up, BOUNCE_FAILED_UP_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.FailedBounce, Direction.Down), new EventPattern(StateType.Position, EventType.FailedBounce, Direction.Down, BOUNCE_FAILED_DOWN_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.FailedBreak, Direction.Up), new EventPattern(StateType.Position, EventType.FailedBreak, Direction.Up, BREAK_FAILED_UP_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.FailedBreak, Direction.Down), new EventPattern(StateType.Position, EventType.FailedBreak, Direction.Down, BREAK_FAILED_DOWN_PATTERN));

			EventPatterns.Add(new EventPatternKey(EventType.StudyValueCrosses, Direction.Up), new EventPattern(StateType.Range, EventType.StudyValueCrosses, Direction.Up, CLOSECROSSES_UP_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.StudyValueCrosses, Direction.Down), new EventPattern(StateType.Range, EventType.StudyValueCrosses, Direction.Down, CLOSECROSSES_DOWN_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.StudyRangeCrosses, Direction.Up), new EventPattern(StateType.Range, EventType.StudyRangeCrosses, Direction.Up, RANGECROSSES_UP_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.StudyRangeCrosses, Direction.Down), new EventPattern(StateType.Range, EventType.StudyRangeCrosses, Direction.Down, RANGECROSSES_DOWN_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.StudyRangeStraddles, Direction.Up), new EventPattern(StateType.Range, EventType.StudyRangeStraddles, Direction.Up, RANGESTRADDLES_UP_PATTERN));
			EventPatterns.Add(new EventPatternKey(EventType.StudyRangeStraddles, Direction.Down), new EventPattern(StateType.Range, EventType.StudyRangeStraddles, Direction.Down, RANGESTRADDLES_DOWN_PATTERN));
		}

		private BarrierHistory(IBarrierValue Barrier, string PositionStates, string RangeStates, Dictionary<EventType, int> LastMatchedLengths)
		{
			this.Barrier = Barrier;
			this.PositionStates = PositionStates;
			this.RangeStates = RangeStates;
			this.LastMatchedLengths = LastMatchedLengths;
		}

		public BarrierHistory(IBarrierValue Barrier)
		{
			this.Barrier = Barrier;
			this.PositionStates = String.Empty;
			this.RangeStates = String.Empty;
			
			this.LastMatchedLengths = new Dictionary<EventType, int>();
			EventType[] eventTypes = (EventType[])Enum.GetValues(typeof(EventType));
			foreach (EventType type in eventTypes)
				if (type != EventType.NotSet) 
					this.LastMatchedLengths.Add(type, 0);
		}

		public object Clone()
		{
			// this.Barrier is a reference derived from cloned values stored in the barrier indicator and so does not need to be cloned itself
			// TODO: this assumption seems correct, but look here if problems occure - especially when attempting to access barrier history from
			// indicator values

			//return new BarrierHistory((IBarrierValue)Utilities.DeepClone(this.Barrier), this.PositionStates, this.RangeStates, GetNextMatchedLengths());


			return new BarrierHistory(this.Barrier, this.PositionStates, this.RangeStates, GetNextMatchedLengths());
		}

		private Dictionary<EventType, int> GetNextMatchedLengths()
		{
			Dictionary<EventType, int> nextMatchedLengths = new Dictionary<EventType,int>();

			int positionLength = this.PositionStates.Length;
			int rangeLength = this.RangeStates.Length;

			EventType[] eventTypes = (EventType[])Enum.GetValues(typeof(EventType));
			foreach (EventType type in eventTypes)
			{
				if (type != EventType.NotSet)
				{
					bool isPositionMatchedLength = false;
					bool isRangedMatchedLength = false;

					Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
					foreach (Direction direction in directions)
					{
						if ((direction != Direction.NotSet) && (direction != Direction.Unknown))
						{
							EventPatternKey key = new EventPatternKey(type, direction);
							StateType stateType = EventPatterns[key].StateType;

							if (IsEvent(key))
							{
								if (stateType == StateType.Position)
									isPositionMatchedLength = true;
								else
									isRangedMatchedLength = true;
							}
						}
					}

					if ((!isPositionMatchedLength) && (!isRangedMatchedLength))
						nextMatchedLengths.Add(type, this.LastMatchedLengths[type]);
					else if (isPositionMatchedLength)
						nextMatchedLengths.Add(type, positionLength);
					else if (isRangedMatchedLength)
						nextMatchedLengths.Add(type, rangeLength);
				}
			}

			return nextMatchedLengths;
		}


		public override string ToString()
		{
			return this.RangeStates + "; " + this.PositionStates;
		}

		public bool IsEvent(EventPatternKey EventPatternKey)
		{
			if (!EventPatterns.ContainsKey(EventPatternKey))
				throw new ApplicationException("Incorrect event passed.");

			EventPattern pattern = EventPatterns[EventPatternKey];

			string stateString = String.Empty;
			switch (pattern.StateType)
			{
				case StateType.Position:
					stateString = this.PositionStates;
					break;

				case StateType.Range:
					stateString = this.RangeStates;
					break;

				default:
					throw new ApplicationException("Incorrect state type.");
			}

			if (stateString.Length == this.LastMatchedLengths[pattern.EventType])
				return false;

			return pattern.Pattern.IsMatch(stateString);
		}

		public void UpdateState(IBarrierValue Barrier, Position Position, Range Range)
		{
			if (this.Barrier.BarrierUniqueKey != Barrier.BarrierUniqueKey)
				throw new ApplicationException("Wrong barrier.");

			this.Barrier = Barrier;

			char position = AllPositionStates.SingleOrDefault(s => s.Key == Position).Value;
			char range = AllRangeStates.SingleOrDefault(s => s.Key == Range).Value;

			if ((String.IsNullOrEmpty(this.PositionStates)) || (!this.PositionStates.EndsWith(Convert.ToString(position))))
				this.PositionStates = String.Concat(this.PositionStates, position);

			if ((String.IsNullOrEmpty(this.RangeStates)) || (!this.RangeStates.EndsWith(Convert.ToString(range))))
				this.RangeStates = String.Concat(this.RangeStates, range);
		}
	}
}
