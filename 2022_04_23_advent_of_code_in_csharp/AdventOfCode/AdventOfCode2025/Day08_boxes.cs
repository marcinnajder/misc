using static System.Math;

namespace AdventOfCode.AdventOfCode2025.Day8;

public record Point(int X, int Y, int Z);

public record Distance(double Value, Point Point1, Point Point2);

public static class Day8
{
    public static Point[] LoadData(string input)
    {
        var lines = input.Split(Environment.NewLine);

        return lines.Select(l => l.Split(','))
            .Select(s => new Point(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2])))
            .ToArray();
    }

    private static List<Distance> GetOrderedDistances(Point[] points)
    {
        var distances = new List<Distance>();
        for (int i = 0; i < points.Length - 1; i++)
        {
            var p1 = points[i];
            for (int j = i + 1; j < points.Length; j++)
            {
                var p2 = points[j];
                var distance = Sqrt(Pow(p1.X - p2.X, 2) + Pow(p1.Y - p2.Y, 2) + Pow(p1.Z - p2.Z, 2));
                distances.Add(new(distance, p1, p2));
            }
        }

        distances.Sort((a, b) => a.Value.CompareTo(b.Value));

        return distances;
    }


    private static IEnumerable<(List<List<Point>> Circuits, Distance Distance)> ProcessDistances(
        IEnumerable<Distance> distances)
    {
        var circuitsByPoints = new Dictionary<Point, List<Point>>();
        var circuits = new List<List<Point>>();

        foreach (var distance in distances)
        {
            var (_, point1, point2) = distance;

            switch (circuitsByPoints.TryGetValue(point1, out var circuits1),
                circuitsByPoints.TryGetValue(point2, out var circuits2))
            {
                // combine both circuits
                case (true, true):
                    if (circuits1 != circuits2) // if points are not already in the same circuit
                    {
                        // move points from shorter to longer circuit
                        var (circuitsFrom, circuitsTo) = circuits1!.Count > circuits2!.Count
                            ? (circuits2, circuits1)
                            : (circuits1, circuits2);
                        circuitsByPoints[point1] = circuitsByPoints[point2] = circuitsTo;
                        circuitsFrom.ForEach(c => circuitsByPoints[c] = circuitsTo);
                        circuitsTo.AddRange(circuitsFrom);
                        circuits.Remove(circuitsFrom);
                    }

                    break;

                // add point to existing circuit
                case (true, false):
                    circuits1!.Add(point2);
                    circuitsByPoints[point2] = circuits1;
                    break;

                // add point to existing circuit
                case (false, true):
                    circuits2!.Add(point1);
                    circuitsByPoints[point1] = circuits2;
                    break;

                // create new circuit
                default:
                    var circuit = new List<Point>() { point1, point2 };
                    circuits.Add(circuit);
                    circuitsByPoints[point1] = circuitsByPoints[point2] = circuit;
                    break;
            }

            yield return (circuits, distance);
        }
    }

    public static string Puzzle1(string input)
    {
        const int nFirstDistances = 1000; // 10 for sample data
        const int nLongestCircuits = 3;

        var points = LoadData(input);
        var distances = GetOrderedDistances(points);
        var (circuitStates, _) = ProcessDistances(distances.Take(nFirstDistances)).Last();

        var result = circuitStates
            .OrderByDescending(circuit => circuit.Count)
            .Take(nLongestCircuits)
            .Aggregate(1, (total, ps) => total * ps.Count);

        return result.ToString();
    }

    public static string Puzzle2(string input)
    {
        var points = LoadData(input);
        var distances = GetOrderedDistances(points);
        var circuitStates = ProcessDistances(distances);

        var (_, distance) =
            circuitStates.First(circuits => circuits.Circuits is [var circuit] && circuit.Count == points.Length);

        long result = (long)distance.Point1.X * distance.Point2.X;

        return result.ToString();
    }
}