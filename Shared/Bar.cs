using System;
using System.Diagnostics;

namespace SteveBagnall.Trading.Shared
{
    public class Bar
	{
		private OHLCV _ohlcv;
		public OHLCV OHLCV
		{
			[DebuggerStepThrough()]
			get { return _ohlcv; }
			[DebuggerStepThrough()]
			private set { _ohlcv = value; }
		}

		public DateTime DateTime
		{
			[DebuggerStepThrough()]
			get { return _ohlcv.DateTime; }
		}

		public decimal Open
		{
			[DebuggerStepThrough()]
			get { return _ohlcv.Open; } 
		}

		public decimal High
		{
			[DebuggerStepThrough()]
			get { return _ohlcv.High; }
		}

		public decimal Low
		{
			[DebuggerStepThrough()]
			get { return _ohlcv.Low; }
		}

		public decimal Close
		{
			[DebuggerStepThrough()]
			get { return _ohlcv.Close; }
		}

		public int Volume
		{
			[DebuggerStepThrough()]
			get { return _ohlcv.Volume; }
		}

		public int Number;

		[DebuggerStepThrough()]
		public Bar(OHLCV Price, int BarNumber)
		{
			_ohlcv = Price;
			this.Number = BarNumber;
		}
	}

	
}
