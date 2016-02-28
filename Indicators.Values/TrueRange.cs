using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class TrueRange : ICloneable
    {
        public Decimal Value;

        public TrueRange()
        {
            this.Value = 0.0M;
        }

        public TrueRange(Decimal Value)
        {
            this.Value = Value;
        }

        public object Clone()
        {
            return new TrueRange(this.Value);
        }
    }
}
