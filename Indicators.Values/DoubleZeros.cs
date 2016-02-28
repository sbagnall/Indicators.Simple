using SteveBagnall.Trading.Indicators.Values.AggregatedLines;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Shared;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class DoubleZeros : AggregateLines, ICloneable
    {
        public DoubleZeros()
            : this(new List<IAggregateLine>())
        { }

        public DoubleZeros(List<IAggregateLine> Lines)
            : base(Lines)
        { }

        public override object Clone()
        {
            return new DoubleZeros((List<IAggregateLine>)Utilities.DeepClone(this.Lines));
        }
    }
}
