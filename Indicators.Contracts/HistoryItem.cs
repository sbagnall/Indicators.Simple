using System;
using System.Diagnostics;

namespace SteveBagnall.Trading.Indicators.Contracts
{
    public class HistoryItem<T> where T : class
    {
        public int BarNumberForPair;
        public DateTime DateTime;
        public T Data;

        [DebuggerStepThrough()]
        public HistoryItem(int BarNumber, DateTime DateTime)
            : this(BarNumber, DateTime, null)
        {
        }

        [DebuggerStepThrough()]
        public HistoryItem(int BarNumberForPair, DateTime DateTime, T Data)
        {
            this.BarNumberForPair = BarNumberForPair;
            this.DateTime = DateTime;
            this.Data = Data;
        }
    }
}
