using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class Complexity : ICloneable
    {
        public Decimal ComplexPath;
        public Decimal DirectPath;
        public Decimal FractalComplexity;

        public Complexity()
            : this(1.0M, 1.0M, 1.0M)
        { }

        public Complexity(Decimal ComplexPath, Decimal DirectPath, Decimal FractalComplexity)
        {
            this.ComplexPath = ComplexPath;
            this.DirectPath = DirectPath;
            this.FractalComplexity = FractalComplexity;
        }

        public object Clone()
        {
            return new Complexity(this.ComplexPath, this.DirectPath, this.FractalComplexity);
        }
    }
}
