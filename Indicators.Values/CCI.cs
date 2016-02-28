using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class CCI : ICloneable, ITrendDirectionValue, ITrendinessValue, IOscillatingValue
    {
        public decimal ThresholdValue;

        public decimal TypicalPrice;
        public decimal Value;

        public CCI(decimal ThresholdValue)
            : this(ThresholdValue, 0.0M, 0.0M)
        { }

        public CCI(decimal ThresholdValue, decimal TypicalPrice, decimal Value)
        {
            this.ThresholdValue = ThresholdValue;
            this.TypicalPrice = TypicalPrice;
            this.Value = Value;
        }

        public Oscillation Oscillation
        {
            get
            {
                if (this.Value > this.ThresholdValue)
                    return Oscillation.OverBought;
                else if (this.Value < (this.ThresholdValue * -1.0M))
                    return Oscillation.OverSold;
                else
                    return Oscillation.Unknown;
            }
        }

        public Trendiness Trendiness
        {
            get
            {
                if (Math.Abs(this.Value) > this.ThresholdValue)
                    return Trendiness.Trendy;
                else if (Math.Abs(this.Value) < this.ThresholdValue)
                    return Trendiness.Choppy;
                else
                    return Trendiness.Unknown;
            }
        }


        public Direction Direction
        {
            get
            {
                if (this.Value > this.ThresholdValue)
                    return Direction.Up;
                else if (this.Value < (this.ThresholdValue * -1.0M))
                    return Direction.Down;
                else
                    return Direction.Unknown;
            }
        }


        public object Clone()
        {
            return new CCI(this.ThresholdValue, this.TypicalPrice, this.Value);
        }
    }
}
