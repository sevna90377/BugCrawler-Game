using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Cubes
{
    public partial class Hex
    {
        internal static readonly List<Hex> Directions = new()
        {
            new Hex(1, 0, -1),
            new Hex(1, -1, 0),
            new Hex(0, -1, 1),
            new Hex(-1, 0, 1),
            new Hex(-1, 1, 0),
            new Hex(0, 1, -1)
        };

        private static readonly List<Hex> Diagonals = new()
        {
            new Hex(2, -1, -1),
            new Hex(1, -2, 1),
            new Hex(-1, -1, 2),
            new Hex(-2, 1, 1),
            new Hex(-1, 2, -1),
            new Hex(1, 1, -2)
        };

        public readonly int Q;
        public readonly int R;
        public readonly int S;

        public Hex(int q, int r, int s)
        {
            Q = q;
            R = r;
            S = s;

            if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
        }

        public Hex(int q, int r)
        {
            Q = q;
            R = r;
            S = -q - r;

            if (q + r + S != 0) throw new ArgumentException("q + r + s must be 0");
        }

        //Unity is YXZ!!! odd-q -> shoves odd columns by +½ row
        public static Vector3Int QoffsetFromCube(Hex h)
        {
            //offset must be EVEN (+1) or ODD (-1)
            var offset = -1;


            var row = h.Q;
            var col = h.R + (h.Q + offset * (h.Q & 1)) / 2;

            return new Vector3Int(col, row, 0);
        }

        //Unity is YXZ!!! odd-q -> shoves odd columns by +½ row
        public static Hex QoffsetToCube(Vector3Int h)
        {
            //offset must be EVEN (+1) or ODD (-1)
            var offset = -1;

            var q = h.y;
            var r = h.x - (h.y + offset * (h.y & 1)) / 2;
            var s = -q - r;

            return new Hex(q, r, s);
        }

        public Hex Add(Hex b)
        {
            return new Hex(Q + b.Q, R + b.R, S + b.S);
        }

        public Hex Subtract(Hex b)
        {
            return new Hex(Q - b.Q, R - b.R, S - b.S);
        }

        public Hex Scale(int k)
        {
            return new Hex(Q * k, R * k, S * k);
        }

        public Hex RotateLeft()
        {
            return new Hex(-S, -Q, -R);
        }

        public Hex RotateRight()
        {
            return new Hex(-R, -S, -Q);
        }

        public static Hex Direction(int direction)
        {
            return Directions[direction];
        }

        public Hex Neighbor(int direction)
        {
            return Add(Direction(direction));
        }

        public IList<Hex> GetAdjacent()
        {
            var result = new List<Hex>();

            for (var i = 0; i < 6; i++) result.Add(Neighbor(i));

            return result;
        }

        public Hex DiagonalNeighbor(int direction)
        {
            return Add(Diagonals[direction]);
        }

        public int Length()
        {
            return (Math.Abs(Q) + Math.Abs(R) + Math.Abs(S)) / 2;
        }

        public int Distance(Hex b)
        {
            return Subtract(b).Length();
        }

        public override string ToString()
        {
            return $"({Q}, {R}, {S})";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Hex);
        }

        public bool Equals(Hex hex)
        {
            if (hex is null) return false;

            // Optimization for a common success case.
            if (ReferenceEquals(this, hex)) return true;

            // If run-time types are not exactly the same, return false.
            if (GetType() != hex.GetType()) return false;

            return Q == hex.Q && R == hex.R && S == hex.S;
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(Q, R, S);
        }
    }
}