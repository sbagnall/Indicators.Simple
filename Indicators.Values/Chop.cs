using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class Chop : ICloneable, ITrendinessValue
    {
        public decimal Threshold;

        public decimal Value;

        public Chop(decimal Threshold) : this(Threshold, 0.0M)
        { }

        public Chop(decimal Threshold, Decimal Value)
        {
            this.Threshold = Threshold;
            this.Value = Value;
        }

        public Trendiness Trendiness
        {
            get
            {
                //decimal threshold = Convert.ToDecimal(ParameterEngine.Instance.CurrentParameters.Get(CHOPPINESS_THRESHOLD).Value);

                if (this.Value <= this.Threshold)
                    return Trendiness.Trendy;
                else if (this.Value > (1.0M - this.Threshold))
                    return Trendiness.Choppy;
                else
                    return Trendiness.Unknown;
            }
        }

        public object Clone()
        {
            return new Chop(this.Threshold, this.Value);
        }
    }
}
