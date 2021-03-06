﻿using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    [Serializable()]
    public class KeltnerChannel : PriceRangeBarrierPropertiesBase, IOscillatingValue
    {
        public decimal MarginThreshold { get; private set; }

        [PriceRangeValue()]
        public Decimal Lower { get; set; }

        [PriceRangeValue()]
        public Decimal Mid { get; set; }

        [PriceRangeValue()]
        public Decimal Upper { get; set; }

        public decimal Price { get; set; }

        public Oscillation Oscillation
        {
            get
            {
                //decimal threshold = Convert.ToDecimal(ParameterEngine.Instance.CurrentParameters.Get(THRESHOLD).Value);
                decimal margin = ((this.Upper - this.Lower) * this.MarginThreshold);

                if (this.Price > (this.Upper - margin))
                    return Oscillation.OverBought;
                else if (this.Price < (this.Lower + margin))
                    return Oscillation.OverSold;
                else
                    return Oscillation.Unknown;
            }
        }

        public KeltnerChannel(decimal MarginThreshold)
            : this(MarginThreshold, 0.0M, 0.0M, 0.0M, 0.0M)
        { }

        public KeltnerChannel(decimal MarginThreshold, decimal Lower, decimal Mid, decimal Upper, decimal Price)
        {
            this.MarginThreshold = MarginThreshold;

            this.Upper = Upper;
            this.Mid = Mid;
            this.Lower = Lower;

            this.Price = Price;
        }

        public override object Clone()
        {
            return new KeltnerChannel(this.MarginThreshold, this.Lower, this.Mid, this.Upper, this.Price);
        }
    }
}
