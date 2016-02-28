using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class MESA : ICloneable
    {
        public decimal Smooth { get; set; }
        public decimal Period { get; set; }
        public decimal SmoothPeriod { get; set; }
        public decimal Detrender { get; set; }
        public decimal I1 { get; set; }
        public decimal Q1 { get; set; }
        public decimal I2 { get; set; }
        public decimal Q2 { get; set; }
        public decimal Re { get; set; }
        public decimal Im { get; set; }
        public decimal Phase { get; set; }
        public decimal DeltaPhase { get; set; }

        public MESA()
            : this(0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M)
        { }

        public MESA(
            decimal Smooth,
            decimal Period,
            decimal SmoothPeriod,
            decimal Detrender,
            decimal I1,
            decimal Q1,
            decimal I2,
            decimal Q2,
            decimal Re,
            decimal Im,
            decimal Phase,
            decimal DeltaPhase)
        {
            this.Smooth = Smooth;
            this.Period = Period;
            this.SmoothPeriod = SmoothPeriod;
            this.Detrender = Detrender;
            this.I1 = I1;
            this.Q1 = Q1;
            this.I2 = I2;
            this.Q2 = Q2;
            this.Re = Re;
            this.Im = Im;
            this.Phase = Phase;
            this.DeltaPhase = DeltaPhase;
        }

        public object Clone()
        {
            return new MESA(
                this.Smooth,
                this.Period,
                this.SmoothPeriod,
                this.Detrender,
                this.I1,
                this.Q1,
                this.I2,
                this.Q2,
                this.Re,
                this.Im,
                this.Phase,
                this.DeltaPhase);
        }
    }
}
