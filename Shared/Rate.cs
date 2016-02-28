using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteveBagnall.Trading.Shared
{
    public struct Rate
    {
        public Symbol Pair;
        public DateTime DateTime;
        public Decimal Bid;
        public Decimal Ask;

        public Rate(Symbol Pair, DateTime DateTime, Decimal Bid, Decimal Ask)
        {
            this.Pair = Pair;
            this.DateTime = DateTime;
            this.Bid = Bid;
            this.Ask = Ask;
        }
    }
}
