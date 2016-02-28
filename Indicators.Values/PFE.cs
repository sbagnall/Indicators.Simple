using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class PFE : ICloneable, ITrendDirectionValue, ITrendinessValue, IOscillatingValue
    {
        public decimal ThresholdPercentage;
        public decimal Value;

        public PFE(decimal ThresholdPercentage)
            : this(ThresholdPercentage, 0.0M)
        { }

        public PFE(decimal ThresholdPercentage, decimal Value)
        {
            this.ThresholdPercentage = ThresholdPercentage;
            this.Value = Value;
        }

        public Oscillation Oscillation
        {
            get
            {
                if (this.Value > this.ThresholdPercentage)
                    return Oscillation.OverBought;
                else if (this.Value < (this.ThresholdPercentage * -1.0M))
                    return Oscillation.OverSold;
                else
                    return Oscillation.Unknown;
            }
        }

        public Trendiness Trendiness
        {
            get
            {
                if (Math.Abs(this.Value) > this.ThresholdPercentage)
                    return Trendiness.Trendy;
                else if (Math.Abs(this.Value) < this.ThresholdPercentage)
                    return Trendiness.Choppy;
                else
                    return Trendiness.Unknown;
            }
        }


        public Direction Direction
        {
            get
            {
                if (this.Value > this.ThresholdPercentage)
                    return Direction.Up;
                else if (this.Value < (this.ThresholdPercentage * -1.0M))
                    return Direction.Down;
                else
                    return Direction.Unknown;
            }
        }


        public object Clone()
        {
            return new PFE(this.ThresholdPercentage, this.Value);
        }
    }
}
