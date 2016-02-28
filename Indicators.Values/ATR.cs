using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class ATR : ICloneable
    {
        public Decimal Value;

        public ATR()
            : this(0.0M)
        { }

        public ATR(Decimal Value)
        {
            this.Value = Value;
        }

        public object Clone()
        {
            return new ATR(this.Value);
        }
    }
}
