using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class Stochastic : ICloneable, IOscillatingValue
    {
        public decimal ThresholdPercentage { get; private set; }

        public decimal FastK;

        public Decimal PercentageK;
        public Decimal PercentageD;

        public Stochastic(decimal ThresholdPercentage)
            : this(ThresholdPercentage, 0.0M, 0.0M)
        { }

        public Stochastic(decimal ThresholdPercentage, Decimal PercentageK, Decimal PercentageD)
            : this(ThresholdPercentage, 0.0M, PercentageK, PercentageD)
        { }

        public Stochastic(decimal ThresholdPercentage, decimal FastK, decimal PercentageK, decimal PercentageD)
        {
            this.ThresholdPercentage = ThresholdPercentage;

            this.FastK = FastK;

            this.PercentageK = PercentageK;
            this.PercentageD = PercentageD;
        }

        public Oscillation Oscillation
        {
            get
            {
                //decimal threshold = Convert.ToDecimal(ParameterEngine.Instance.CurrentParameters.Get(THRESHOLD).Value);

                if (this.PercentageD > (100.0M - this.ThresholdPercentage))
                    return Oscillation.OverBought;
                else if (this.PercentageD < this.ThresholdPercentage)
                    return Oscillation.OverSold;
                else
                    return Oscillation.Unknown;
            }
        }

        public object Clone()
        {
            return new Stochastic(this.ThresholdPercentage, this.FastK, this.PercentageK, this.PercentageD);
        }
    }
}
