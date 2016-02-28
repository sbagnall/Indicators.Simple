using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class CTI : ICloneable, ITrendinessValue, ITrendDirectionValue
    {
        private int THRESHOLD = 1;

        public Decimal Value;

        public CTI()
        {
            this.Value = 1.0M;
        }

        public CTI(Decimal Value)
        {
            this.Value = Value;
        }

        public Trendiness Trendiness
        {
            get
            {
                if (Math.Abs(this.Value) < THRESHOLD)
                    return Trendiness.Choppy;
                else if (Math.Abs(this.Value) > THRESHOLD)
                    return Trendiness.Trendy;
                else
                    return Trendiness.Unknown;
            }
        }

        public Direction Direction
        {
            get
            {
                if (this.Value > THRESHOLD)
                    return Direction.Up;
                else if (this.Value < (THRESHOLD * -1.0M))
                    return Direction.Down;
                else
                    return Direction.Unknown;
            }
        }

        public object Clone()
        {
            return new CTI(this.Value);
        }
    }
}
