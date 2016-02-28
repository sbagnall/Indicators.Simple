using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class MomentumAbsolute : ICloneable, ITrendDirectionValue
    {
        public Decimal Value;

        public MomentumAbsolute()
        {
            this.Value = 0.0M;
        }

        public MomentumAbsolute(Decimal Value)
        {
            this.Value = Value;
        }

        public object Clone()
        {
            return new MomentumAbsolute(this.Value);
        }

        public virtual Direction Direction { get { return (this.Value > 0.0M) ? Direction.Up : ((this.Value < 0.0M) ? Direction.Down : Direction.Unknown); } }
    }
}
