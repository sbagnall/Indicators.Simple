using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    [Serializable()]
    public class EMAPair : PriceRangeBarrierPropertiesBase, ITrendDirectionValue, IMovingAveragePairValue
    {
        private Decimal _fast;
        [PriceRangeValue()]
        public decimal Fast
        {
            get { return _fast; }
            set { _fast = value; }
        }

        private Decimal _slow;
        [PriceRangeValue()]
        public Decimal Slow
        {
            get { return _slow; }
            set { _slow = value; }
        }

        public Direction Direction { get { return (this.Fast > this.Slow) ? Direction.Up : ((this.Fast < this.Slow) ? Direction.Down : Direction.NotSet); } }

        public EMAPair()
            : this(0.0M, 0.0M)
        { }

        public EMAPair(Decimal Fast, Decimal Slow)
        {
            this.Fast = Fast;
            this.Slow = Slow;
        }

        public override object Clone()
        {
            return new EMAPair(this.Fast, this.Slow);
        }
    }
}
