using System;
using System.Collections.Generic;
using System.Linq;

namespace SteveBagnall.Trading.Shared
{
    [Serializable()]
	public class OHLCV
    {
        public DateTime DateTime;

		// Used to determin which Rate (bid/ask) quote is relevant for the Open and Close proces
		public DateTime FirstDateTime = DateTime.MaxValue;
		public DateTime LastDateTime = DateTime.MinValue;

        public Decimal Open = 0.0M;
        public Decimal High = Decimal.MinValue;
        public Decimal Low = Decimal.MaxValue;
        public Decimal Close = 0.0M;
        public int Volume = 0;

		public OHLCV(DateTime DateTime)
		{
			this.DateTime = DateTime;
		}

		public static OHLCV operator -(OHLCV lhs, Decimal rhs)
		{
			OHLCV retVal = new OHLCV(lhs.DateTime);
			retVal.Close = lhs.Close - rhs;
			retVal.High = lhs.High - rhs;
			retVal.Low = lhs.Low - rhs;
			retVal.Open = lhs.Open - rhs;
			return retVal;
		}

		public static OHLCV operator +(OHLCV lhs, Decimal rhs)
		{
			OHLCV retVal = new OHLCV(lhs.DateTime);
			retVal.Close = lhs.Close + rhs;
			retVal.High = lhs.High + rhs;
			retVal.Low = lhs.Low + rhs;
			retVal.Open = lhs.Open + rhs;
			return retVal;
		}

		public void Update(OHLCV Data)
		{
			if (Data.FirstDateTime < this.FirstDateTime)
			{
				this.FirstDateTime = Data.FirstDateTime;
				this.Open = Data.Open;
			}

			if (Data.High > this.High)
				this.High = Data.High;

			if (Data.Low < this.Low)
				this.Low = Data.Low;

			if (Data.LastDateTime > this.LastDateTime)
			{
				this.LastDateTime = Data.LastDateTime;
				this.Close = Data.Close;
			}
		}

        public static double StandardDeviation(List<OHLCV> Values)
        {
            var transformedData = new List<double>();
            for (int i = 0; i < Values.Count; i++)
                transformedData.Add((double)Values[i].Close);

            return Utilities.StandardDeviation(transformedData);
        }

        public static Decimal SquareError(OHLCV Prediction, OHLCV Actual)
        {
            return (Decimal)Math.Pow((double)(Prediction.Close - Actual.Close), 2);
        }
    }

	public class OHLCVComparer : IComparer<OHLCV>
	{
		private bool _sortDescending = false;

		public OHLCVComparer(bool descending)
		{
			_sortDescending = descending;
		}

		public int Compare(OHLCV lhs, OHLCV rhs)
		{
			if (_sortDescending)
				return rhs.DateTime.CompareTo(lhs.DateTime);
			else
				return lhs.DateTime.CompareTo(rhs.DateTime);
		}
	}

}
