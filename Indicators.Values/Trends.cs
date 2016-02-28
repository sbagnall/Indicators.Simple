using SteveBagnall.Trading.Indicators.Values.AggregatedLines;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class Trends : ICloneable, ITrendDirectionValue
    {
        public TrendLine PeakToPeak { get; set; }
        public TrendLine DipToDip { get; set; }

        public Trends()
            : this(null, null)
        { }

        public Trends(TrendLine PeakToPeak, TrendLine DipToDip)
        {
            this.PeakToPeak = PeakToPeak;
            this.DipToDip = DipToDip;
        }

        public Direction Direction
        {
            get
            {
                if ((this.PeakToPeak == null) || (this.DipToDip == null))
                    return Direction.Unknown;

                if ((this.PeakToPeak.Gradient < 0.0M) && (this.DipToDip.Gradient <= 0.0M))
                    return Direction.Down;
                else if ((this.DipToDip.Gradient > 0.0M) && (this.PeakToPeak.Gradient >= 0.0M))
                    return Direction.Up;
                else
                    return Direction.Unknown;
            }
        }

        public object Clone()
        {
            return new Trends(
                (this.PeakToPeak == null) ? null : (TrendLine)Utilities.DeepClone(this.PeakToPeak),
                (this.DipToDip == null) ? null : (TrendLine)Utilities.DeepClone(this.DipToDip));
        }
    }
}
