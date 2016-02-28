using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class StandardDeviation : ICloneable
    {
        public Decimal Value;

        public StandardDeviation()
        {
            this.Value = 0.0M;
        }

        public StandardDeviation(Decimal Value)
        {
            this.Value = Value;
        }

        public object Clone()
        {
            return new StandardDeviation(this.Value);
        }
    }
}
