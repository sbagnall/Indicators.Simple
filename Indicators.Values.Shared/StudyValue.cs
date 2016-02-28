namespace SteveBagnall.Trading.Indicators.Values.Shared
{
    public struct StudyValue
	{
		public decimal High;
		public decimal Low;
		public decimal Value;

		public StudyValue(decimal High, decimal Low, decimal Value)
		{
			this.High = High;
			this.Low = Low;
			this.Value = Value;
		}

		public StudyValue(decimal Value)
			: this(Value, Value, Value)
		{ }
	}
}
