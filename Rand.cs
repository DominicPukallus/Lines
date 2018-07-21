using System;
using System.Collections.Generic;
using System.Text;

namespace Lines2_1
{
    class Rand
    {
        private double c;
        private double x;
        private double range;

        public Rand()
        {
            c = -1.99999999999999;
            x = c;
            range = c * -1;
        }

        public double Get()
        {
            x = (x * x) + c;

            return (x / range);
        }
    }
}
