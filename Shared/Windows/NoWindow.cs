using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteveBagnall.Trading.Shared.Windows
{
    public class NoWindow : IWindow
    {
        public NoWindow()
        { }

        public double ValueAt(int N, int n)
        {
            return 1.0;
        }

    }
}
