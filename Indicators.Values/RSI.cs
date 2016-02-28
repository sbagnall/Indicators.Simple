using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class RSI : ICloneable, IOscillatingValue
    {
        public decimal ThresholdPercentage { get; private set; }

        public decimal AverageGain;
        public decimal AverageLoss;
        public decimal RS;
        public decimal Value;

        public RSI(decimal ThresholdPercentage)
            : this(ThresholdPercentage, 0.0M, 0.0M, 0.0M, 0.0M)
        { }

        public RSI(decimal ThresholdPercentage, decimal AverageGain, decimal AverageLoss, decimal RS, decimal RSI)
        {
            this.ThresholdPercentage = ThresholdPercentage;

            this.AverageGain = AverageGain;
            this.AverageLoss = AverageLoss;
            this.RS = RS;
            this.Value = RSI;
        }

        public Oscillation Oscillation
        {
            get
            {
                //decimal threshold = Convert.ToDecimal(ParameterEngine.Instance.CurrentParameters.Get(THRESHOLD).Value);

                if (this.Value > (100.0M - this.ThresholdPercentage))
                    return Oscillation.OverBought;
                else if (this.Value < this.ThresholdPercentage)
                    return Oscillation.OverSold;
                else
                    return Oscillation.Unknown;
            }
        }

        public object Clone()
        {
            return new RSI(this.ThresholdPercentage, this.AverageGain, this.AverageLoss, this.RS, this.Value);
        }
    }
}
