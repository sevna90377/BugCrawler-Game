using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Cubes
{
    partial class Hex
    {
        /// <summary>
        /// Sum of G and H.
        /// </summary>
        private int F => _g + _h;

        /// <summary>
        /// Cost from start tile to this tile.
        /// </summary>
        private int _g;

        /// <summary>
        /// Estimated cost from this tile to destination tile.
        /// </summary>
        private int _h;

        private Hex Parent { get; set; }

        public List<Hex> FindPath(Hex goal, IHexGrid grid)
        {
            if (grid.IsObstacle(goal) || !grid.IsValid(goal))
            {
                return null;
            }

            Hex start = this;

            var openList = new List<Hex> { start };
            var closedList = new HashSet<Hex>();

            while (openList.Count > 0)
            {
                openList.Sort((node1, node2) => node1.F.CompareTo(node2.F));

                var current = openList[0];

                if (current.Q == goal.Q && current.R == goal.R)
                {
                    return ReconstructPath(current);
                }

                openList.Remove(current);
                closedList.Add(current);

                var neighbors = current.GetAdjacent().ToList();

                foreach (var neighbor in neighbors)
                {
                    if (grid.IsObstacle(neighbor) || !grid.IsValid(neighbor))
                    {
                        continue;
                    }

                    if (closedList.Contains(neighbor))
                    {
                        continue;
                    }

                    neighbor._h = neighbor.Distance(goal);
                    neighbor._g = current._g + 1;
                    neighbor.Parent = current;

                    var openNode = openList.Find(node => node.Q == neighbor.Q && node.R == neighbor.R);
                    if (openNode == null || neighbor._g < openNode._g)
                    {
                        openList.Add(neighbor);
                    }
                }
            }

            return null;
        }

        private static List<Hex> ReconstructPath(Hex node)
        {
            var path = new List<Hex>();
            while (node != null)
            {
                path.Add(node);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }
    }
}
