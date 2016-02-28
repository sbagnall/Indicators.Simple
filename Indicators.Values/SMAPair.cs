using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class SMAPair : PriceRangeBarrierPropertiesBase, ITrendDirectionValue, IMovingAveragePairValue
    {
        private decimal _fast;
        [PriceRangeValue()]
        public decimal Fast
        {
            get { return _fast; }
            set { _fast = value; }
        }

        private decimal _slow;
        [PriceRangeValue()]
        public decimal Slow
        {
            get { return _slow; }
            set { _slow = value; }
        }

        public Direction Direction { get { return (this.Fast > this.Slow) ? Direction.Up : ((this.Fast < this.Slow) ? Direction.Down : Direction.Unknown); } }

        public SMAPair()
            : this(0.0M, 0.0M)
        { }

        public SMAPair(decimal Fast, decimal Slow)
        {
            this.Fast = Fast;
            this.Slow = Slow;
        }

        public override object Clone()
        {
            return new SMAPair(this.Fast, this.Slow);
        }
    }
}
