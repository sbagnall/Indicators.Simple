using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators.BarrierStudyPair
{
    public class BarrierHistories
	{
		private Dictionary<int, BarrierHistory> _histories = new Dictionary<int, BarrierHistory>();
		/// <summary>
		/// Histories keyed on Barrier Unique key
		/// </summary>
		private Dictionary<int, BarrierHistory> Histories
		{
			get { return _histories; }
			set { _histories = value; }
		}

		public BarrierHistories()
		{ }

		private BarrierHistories(Dictionary<int, BarrierHistory> Histories)
		{
			this.Histories = Histories;
		}

		public BarrierHistory this[IBarrierValue Barrier]
		{
			get
			{
				return this.Histories[Barrier.BarrierUniqueKey];
			}
		}

		public object Clone()
		{
			Dictionary<int, BarrierHistory> histories = new Dictionary<int, BarrierHistory>();

			foreach (int key in this.Histories.Keys)
				histories.Add(key, (BarrierHistory)this.Histories[key].Clone());

			return new BarrierHistories(histories);
		}

		public void UpdateState(IBarrierValue Barrier, BarrierHistory.Position Position, BarrierHistory.Range Range)
		{
			if (!this.Histories.ContainsKey(Barrier.BarrierUniqueKey))
				this.Histories.Add(Barrier.BarrierUniqueKey, new BarrierHistory(Barrier));

			this.Histories[Barrier.BarrierUniqueKey].UpdateState(Barrier, Position, Range);
		}

		public List<BarrierEventObject> GetAllEvents(StudyValue StudyValue, int BarNumber)
		{
			List<BarrierEventObject> events = new List<BarrierEventObject>();

			foreach (BarrierHistory history in this.Histories.Values)
			{
				EventType[] eventTypes = (EventType[])Enum.GetValues(typeof(EventType));
				foreach (EventType eventType in eventTypes)
				{
					if (eventType != EventType.NotSet)
					{
						Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
						foreach (Direction direction in directions)
						{
							EventPatternKey key = new EventPatternKey(eventType, direction);
							if ((direction != Direction.NotSet) && (direction != Direction.Unknown))
							{
								BarrierEventObject evnt = GetEvent(key, history.Barrier, StudyValue, BarNumber);
								if (evnt != null)
									events.Add(evnt);
							}
						}
					}
				}
			}

			return events;
		}

		private BarrierEventObject GetEvent(
			EventPatternKey EventPatternKey,
			IBarrierValue Barrier, 
			StudyValue StudyValue, 
			int BarNumber)
		{
			if (this.Histories[Barrier.BarrierUniqueKey].IsEvent(EventPatternKey))
				return new BarrierEventObject(EventPatternKey.EventType, EventPatternKey.Direction, Barrier, StudyValue, BarNumber);
			else
				return null;
		}
		
	}
}
