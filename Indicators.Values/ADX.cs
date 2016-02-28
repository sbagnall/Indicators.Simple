using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class ADXRelative : ICloneable, ITrendinessValue
    {
        public Decimal Value;

        public ADXRelative()
            : this(0.0M)
        { }

        public ADXRelative(decimal Value)
        {
            this.Value = Value;
        }

        public Trendiness Trendiness { get { return (this.Value > 0.0M) ? Trendiness.Trendy : ((this.Value < 0.0M) ? Trendiness.Choppy : Trendiness.Unknown); } }

        public object Clone()
        {
            return new ADXRelative(this.Value);
        }
    }

    public class ADX : ICloneable, ITrendinessValue, IOscillatingValue
    {
        public Trendiness Trendiness
        {
            get
            {
                //decimal threshold = Convert.ToDecimal(ParameterEngine.Instance.CurrentParameters.Get(THRESHOLD).Value);
                return (this.ADXValue > this.ThresholdADX) ? Trendiness.Trendy : ((this.ADXValue < this.ThresholdADX) ? Trendiness.Choppy : Trendiness.NotSet);
            }
        }

        public Oscillation Oscillation
        {
            get
            {
                if (this.PlusDI > this.MinusDI)
                    return Oscillation.OverSold;
                else if (this.MinusDI > this.PlusDI)
                    return Oscillation.OverBought;
                else
                    return Oscillation.Unknown;
            }
        }

        public decimal ThresholdADX;

        public decimal PlusSDI;
        public decimal MinusSDI;
        public Decimal PlusDI;
        public Decimal MinusDI;
        public decimal DX;
        public Decimal ADXValue;

        public ADX(decimal ThresholdADX)
            : this(ThresholdADX, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M)
        { }

        public ADX(
            decimal ThresholdADX,
            decimal PlusSDI,
            decimal MinusSDI,
            Decimal PlusDI,
            Decimal MinusDI,
            decimal DX,
            Decimal ADXValue)
        {
            this.ThresholdADX = ThresholdADX;

            this.PlusSDI = PlusSDI;
            this.MinusSDI = MinusSDI;
            this.PlusDI = PlusDI;
            this.MinusDI = MinusDI;
            this.DX = DX;
            this.ADXValue = ADXValue;
        }

        public object Clone()
        {
            return new ADX(this.ThresholdADX, this.PlusSDI, this.MinusSDI, this.PlusDI, this.MinusDI, this.DX, this.ADXValue);
        }
    }
}
