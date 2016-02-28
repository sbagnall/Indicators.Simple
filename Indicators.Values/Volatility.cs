using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class Volatility : ICloneable
    {
        public Decimal Value;

        public Volatility()
            : this(0.0M)
        { }

        public Volatility(Decimal Value)
        {
            this.Value = Value;
        }

        public object Clone()
        {
            return new Volatility(this.Value);
        }
    }
}
