using System;

namespace Assets.Scripts.Cubes
{
    internal readonly struct FractionalHex
    {
        public readonly double Q;
        public readonly double R;
        public readonly double S;

        public FractionalHex(double q, double r, double s)
        {
            Q = q;
            R = r;
            S = s;

            if (Math.Round(q + r + s) != 0) throw new ArgumentException("q + r + s must be 0");
        }

        public Hex HexRound()
        {
            var qi = (int)Math.Round(Q);
            var ri = (int)Math.Round(R);
            var si = (int)Math.Round(S);
            var qDiff = Math.Abs(qi - Q);
            var rDiff = Math.Abs(ri - R);
            var sDiff = Math.Abs(si - S);

            if (qDiff > rDiff && qDiff > sDiff)
                qi = -ri - si;
            else if (rDiff > sDiff)
                ri = -qi - si;
            else
                si = -qi - ri;

            return new Hex(qi, ri, si);
        }

        public FractionalHex HexLerp(FractionalHex b, double t)
        {
            return new FractionalHex(Q * (1.0 - t) + b.Q * t, R * (1.0 - t) + b.R * t, S * (1.0 - t) + b.S * t);
        }
    }
}