using SteveBagnall.Trading.Shared;

namespace SteveBagnall.Trading.Indicators.BarrierStudyPair
{
    public struct EventPatternKey
	{
		public EventType EventType;
		public Direction Direction;

		public EventPatternKey(EventType EventType, Direction Direction)
		{
			this.EventType = EventType;
			this.Direction = Direction;
		}
	}
}
