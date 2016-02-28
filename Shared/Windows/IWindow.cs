using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteveBagnall.Trading.Shared.Windows
{
    public interface IWindow
    {
        double ValueAt(int N, int n);
    }
}
