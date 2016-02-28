namespace SteveBagnall.Trading.Indicators.BarrierStudyPair
{
    public enum EventType
	{
		NotSet = 0,
		Break,
		Bounce,
		Approach,
		FailedBounce,
		FailedBreak,
		StudyRangeCrosses,
		StudyRangeStraddles,
		StudyValueCrosses,
	}
}
