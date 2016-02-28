using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.BarrierStudyPair
{
    public class BarrierEventObject
	{
		public EventType EventType;
		public Direction Direction;
		public IBarrierValue Barrier;
		public StudyValue StudyValue;
		public int BarNumber;

		public BarrierEventObject(EventType EventType, Direction Direction, IBarrierValue Barrier, StudyValue StudyValue, int BarNumber)
		{
			this.EventType = EventType;
			this.Direction = Direction;
			this.Barrier = Barrier;
			this.StudyValue = StudyValue;
			this.BarNumber = BarNumber;
		}

		public decimal ValueActivated
		{
			get
			{
				decimal lowerPBoundary = this.Barrier.GetProximityLowerBoundaryAt(BarNumber);
				decimal upperPBoundary = this.Barrier.GetProximityUpperBoundaryAt(BarNumber);

				switch (this.EventType)
				{
					case EventType.Approach:
						return (this.Direction == Direction.Up) ? lowerPBoundary : upperPBoundary;
					case EventType.Bounce:
						return (this.Direction == Direction.Up) ? upperPBoundary : lowerPBoundary;
					case EventType.Break:
						return (this.Direction == Direction.Up) ? upperPBoundary : lowerPBoundary;
					case EventType.FailedBounce:
						return (this.Direction == Direction.Up) ? upperPBoundary : lowerPBoundary;
					case EventType.FailedBreak:
						return (this.Direction == Direction.Up) ? upperPBoundary : lowerPBoundary;

					case EventType.StudyValueCrosses:
						return this.StudyValue.Value;
					case EventType.StudyRangeCrosses:
						return this.StudyValue.Value;
					case EventType.StudyRangeStraddles:
						return this.StudyValue.Value;
				}

				throw new ApplicationException("Unknown event type.");
			}
		}
	}
}
