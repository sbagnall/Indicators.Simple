using System;

namespace SteveBagnall.Trading.Indicators.Values.Shared
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PriceRangeValueAttribute : Attribute
    {
        public PriceRangeValueAttribute()
        {
        }
    }
}
