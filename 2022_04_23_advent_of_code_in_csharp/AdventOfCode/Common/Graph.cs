using System.Transactions;
using Microsoft.VisualBasic;

namespace AdventOfCode;

public record Link<T>(T To, int Weight);

public record Edge<T>(T From, T To, int Weight);

public record Visited<T>(T Node, int Length, Dictionary<T, List<T>> CameFrom) where T : notnull;


public class Graph
{
    public static IEnumerable<Link<T>> DijkstraTraverse<T>(T start, Func<T, Link<T>[]> getNeighbours)
        where T : notnull
    {
        var neighborsCache = new Dictionary<T, Link<T>[]>();
        var costSoFar = new Dictionary<T, int>() { { start, 0 } };
        var queue = new PriorityQueue<T, int>([(start, 0)]);

        while (queue.TryDequeue(out var current, out var currentCost))
        {
            yield return new Link<T>(current, currentCost);

            var neighbors = neighborsCache.GetOrAdd(current, getNeighbours);

            foreach (var next in neighbors)
            {
                var newCost = costSoFar[current] + next.Weight;
                if (!costSoFar.TryGetValue(next.To, out var nextCost) || (newCost < nextCost))
                {
                    costSoFar[next.To] = newCost;
                    queue.Enqueue(next.To, newCost);
                }
            }
        }
    }


    public static IEnumerable<Link<T>> DijkstraTraverse<T>(T start, Edge<T>[] edges)
        where T : notnull
    {
        var edgesDic = edges
            .GroupBy(e => e.From)
            .ToDictionary(gr => gr.Key, gr => gr.Select(e => new Link<T>(e.To, e.Weight)).ToArray());

        return DijkstraTraverse(start, e => edgesDic[e]);
    }

    public static int? DijkstraShortestPath<T>(T start, T finish, Edge<T>[] edges)
        where T : notnull
    {
        return DijkstraTraverse(start, edges).FirstOrDefault(l => EqualityComparer<T>.Default.Equals(l.To, finish))?.Weight;
    }

    public static int? DijkstraShortestPath<T>(T start, T finish, Func<T, Link<T>[]> getNeighbours)
        where T : notnull
    {
        return DijkstraTraverse(start, getNeighbours).FirstOrDefault(l => EqualityComparer<T>.Default.Equals(l.To, finish))?.Weight;
    }

    public static IEnumerable<Edge<(int r, int c)>> LoadGraphFromGrid(int[][] rows)
    {
        var rowCount = rows.Length - 1;
        var columnCount = rows[0].Length - 1;

        for (int r = 0; r <= rowCount; r++)
        {
            for (int c = 0; c <= columnCount; c++)
            {
                var from = (r, c);
                foreach (var to in Grid.GetNeighbours(r, c, rowCount, columnCount))
                {
                    yield return new Edge<(int r, int c)>(from, to, rows[to.r][to.c]);
                }
            }
        }
    }

    // saving and returning all possible paths
    public static IEnumerable<Visited<T>> DijkstraTraverseAll<T>(T start, Func<T, Link<T>[]> getNeighbours)
        where T : notnull
    {
        var neighborsCache = new Dictionary<T, Link<T>[]>();
        var costSoFar = new Dictionary<T, int>() { { start, 0 } };
        var queue = new PriorityQueue<T, int>([(start, 0)]);

        var cameFrom = new Dictionary<T, List<T>>(); // mutable dictionary !

        while (queue.TryDequeue(out var current, out var currentCost))
        {
            yield return new Visited<T>(current, currentCost, cameFrom);

            var neighbors = neighborsCache.GetOrAdd(current, getNeighbours);

            foreach (var next in neighbors)
            {
                var newCost = costSoFar[current] + next.Weight;
                var visited = costSoFar.TryGetValue(next.To, out var nextCost);
                if (!visited || (newCost < nextCost))
                {
                    costSoFar[next.To] = newCost;
                    queue.Enqueue(next.To, newCost);
                    cameFrom[next.To] = new List<T>() { current };
                }
                else if (visited && newCost == nextCost) // many paths of the same length to the same node
                {
                    cameFrom[next.To].Add(current);
                }
            }
        }
    }
}