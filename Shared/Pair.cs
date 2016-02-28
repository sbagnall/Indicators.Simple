using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteveBagnall.Trading.Shared
{
	public class Pair
	{
		private string _baseCurrency;
		public string BaseCurrency
		{
			[System.Diagnostics.DebuggerStepThrough()]
			get { return _baseCurrency; }
			[System.Diagnostics.DebuggerStepThrough()]
			private set { _baseCurrency = value; }
		}

		private string _quoteCurrency;
		public string QuoteCurrency
		{
			[System.Diagnostics.DebuggerStepThrough()]
			get { return _quoteCurrency; }
			[System.Diagnostics.DebuggerStepThrough()]
			private set { _quoteCurrency = value; }
		}

		public Symbol Symbol
		{
			[System.Diagnostics.DebuggerStepThrough()]
			get { return (Symbol)Enum.Parse(typeof(Symbol), String.Format("{0}_{1}", this.BaseCurrency, this.QuoteCurrency)); }
		}

		[System.Diagnostics.DebuggerStepThrough()]
		public Pair(string Symbol)
		{
			string[] cols = Symbol.Split(new char[] { '_' });
			this.BaseCurrency = cols[0];
			this.QuoteCurrency = cols[1];
		}

		[System.Diagnostics.DebuggerStepThrough()]
		public Pair(string BaseCurrency, string QuoteCurrency)
		{
			this.BaseCurrency = BaseCurrency;
			this.QuoteCurrency = QuoteCurrency;
		}
	}
}
