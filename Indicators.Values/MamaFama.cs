using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class MamaFama : PriceRangeBarrierPropertiesBase, ITrendDirectionValue, IMovingAveragePairValue
    {
        private decimal _mama;
        [PriceRangeValue()]
        public decimal MAMA
        {
            get { return _mama; }
            set { _mama = value; }
        }

        public decimal Fast { get { return this.MAMA; } }

        private decimal _fama;
        [PriceRangeValue()]
        public decimal FAMA
        {
            get { return _fama; }
            set { _fama = value; }
        }

        public decimal Slow { get { return this.FAMA; } }

        public decimal Alpha { get; set; }

        public Direction Direction { get { return (this.MAMA > this.FAMA) ? Direction.Up : ((this.MAMA < this.FAMA) ? Direction.Down : Direction.Unknown); } }

        public MamaFama()
            : this(0.0M, 0.0M, 0.0M)
        { }

        public MamaFama(
            decimal MAMA,
            decimal FAMA,
            decimal Alpha)
        {
            this.MAMA = MAMA;
            this.FAMA = FAMA;
            this.Alpha = Alpha;
        }

        public override object Clone()
        {
            return new MamaFama(
                this.MAMA,
                this.FAMA,
                this.Alpha);
        }
    }
}
