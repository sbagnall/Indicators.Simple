using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators.Contracts
{
    public interface IIndicatorHistory<T> where T : class
    {
        T this[int PeriodsAgo] { get; set; }
        T this[DateTime DateTime] { get; set; }

        Dictionary<int, DateTime> BarNumberDates { get; }
        int CurrentBar { get; set; }
        HistoryItem<T>[] Data { get; }
        Dictionary<DateTime, int> DateBarNumbers { get; }
        IIndicator Indicator { get; }
        bool IsPopulated { get; set; }
        int MaxBar { get; }


        int GetBarNumber(int PeriodsAgo);
        int GetBarNumber(DateTime DateTime);
        DateTime GetDateTime(int PeriodsAgo);
        int GetPeriodsAgo(DateTime DateTime);
        bool HasItem(int PeriodsAgo);
        bool HasItem(DateTime DateTime);
        bool SetBar(int BarNumber);
        bool SetBar(DateTime DateTime);
    }
}