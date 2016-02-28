using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteveBagnall.Trading.Shared.Windows
{
    public class TurkeyWindow : IWindow
    {
        private const double DEFAULT_ALPHA = 0.5;

        private double _alpha;

        public TurkeyWindow() : this(DEFAULT_ALPHA)
        { }

        public TurkeyWindow(double Alpha)
        {
            _alpha = Alpha;
        }

        public double ValueAt(int N, int n)
        {
            n = (int)Math.Floor(Math.Abs(n - (N / 2.0)));

            if ((n <= ((_alpha * N) / 2.0)) || (n >= (N - ((_alpha * N) / 2.0))))
                return 1;
            else
                return 0.5 * (1.0 + Math.Cos(Math.PI * ((n - ((_alpha * N) / 2.0)) / ((1 - _alpha) * (N / 2.0)))));

        }
    }
}
