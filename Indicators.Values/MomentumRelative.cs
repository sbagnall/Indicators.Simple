using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class MomentumRelative : ICloneable, ITrendDirectionValue
    {
        public Decimal Value;

        public MomentumRelative()
            : this(0.0M)
        { }

        public MomentumRelative(decimal Value)
        {
            this.Value = Value;
        }

        public Direction Direction { get { return (this.Value > 0.0M) ? Direction.Up : ((this.Value < 0.0M) ? Direction.Down : Direction.Unknown); } }

        public object Clone()
        {
            return new MomentumRelative(this.Value);
        }
    }
}
