using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Cubes
{
    public static class HexExtensions
    {
        public static List<Hex> GetLine(this Hex a, Hex b)
        {
            var n = a.Distance(b);

            var aNudge = new FractionalHex(a.Q + 1e-06, a.R + 1e-06, a.S - 2e-06);
            var bNudge = new FractionalHex(b.Q + 1e-06, b.R + 1e-06, b.S - 2e-06);

            var results = new List<Hex>();
            var step = 1.0 / Math.Max(n, 1);

            for (var i = 0; i <= n; i++) results.Add(aNudge.HexLerp(bNudge, step * i).HexRound());

            return results;
        }

        public static IList<Hex> ReachableHexes(this Hex start, int movement, IHexGrid grid)
        {
            IList<Hex> visited = new List<Hex> { start };

            var fringes = new List<List<Hex>> { new() { start } };

            for (var k = 1; k <= movement; k++)
            {
                fringes.Add(new List<Hex>());

                foreach (var hex in fringes[k - 1])
                    for (var direction = 0; direction < 6; direction++)
                    {
                        var neighbor = hex.Neighbor(direction);

                        if (!visited.Contains(neighbor) && !grid.IsObstacle(neighbor))
                        {
                            visited.Add(neighbor);
                            fringes[k].Add(neighbor);
                        }
                    }
            }

            return visited;
        }

        public static HashSet<Hex> CalculateVisibleHexes(this Hex origin, int radius, IHexGrid grid)
        {
            HashSet<Hex> visibleHexes = new HashSet<Hex> { origin };

            List<Tuple<float, float>> dirs = new List<Tuple<float, float>>();

            for (float i = -1; i <= 1; i += 0.2f)
            {
                for (float j = -1; j <= 1; j += 0.2f)
                {
                    dirs.Add(new Tuple<float, float>(i, j));
                }
            }

            foreach (var tuple in dirs)
            {
                float dx = tuple.Item1;
                float dy = tuple.Item2;

                // Start stepping from the origin
                Hex currentHex = origin;
                for (int step = 1; step <= radius; step++)
                {
                    // Approximate the hex we're aiming for at this step along the ray.
                    // This is a simplification and might need refinement based on your grid layout.
                    Hex targetHex = ApproximateHexOnRay(origin, dx * step, dy * step);

                    if (targetHex.Distance(origin) > radius)
                    {
                        continue;
                    }

                    List<Hex> line = GetLine(currentHex, targetHex);

                    foreach (var hex in line)
                    {
                        if (!grid.IsValid(hex))
                            break; // Out of bounds

                        if (grid.IsObstacle(hex))
                            break; // Stop the ray if an obstacle is hit

                        visibleHexes.Add(hex);
                    }

                    // Update currentHex for the next step (this is a simplification; a proper
                    // ray stepping algorithm would be more precise).
                    if (line.Any())
                    {
                        foreach (var hex in line.Skip(1))
                        {
                            if (grid.IsObstacle(hex) || !grid.IsValid(hex))
                            {
                                break;
                            }

                            currentHex = hex;
                        }

                        //currentHex = line.Last();

                        //if (grid.IsObstacle(currentHex) || !grid.IsValid(currentHex))
                        //    break; // Stop the ray if an obstacle is hit
                    }
                    else
                    {
                        break; // No hexes in the line, something went wrong or we reached the edge
                    }
                }
            }

            return visibleHexes;
        }

        // A simplified approximation of a hex along a ray. This needs refinement
        // based on your specific hex grid layout and coordinate system.
        private static Hex ApproximateHexOnRay(Hex origin, float dx, float dy)
        {
            // This is a very basic approximation. You might need a more sophisticated
            // method that considers the skew of the hex grid.
            int q = Mathf.RoundToInt(origin.Q + dx);
            int r = Mathf.RoundToInt(origin.R + dy);
            return new Hex(q, r);
        }
    }
}
