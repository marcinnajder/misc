using System.Collections;
using System.Numerics;

namespace AdventOfCode;

using System;

//using Line<T> = (T From, T To);

public static class RangeList
{
// let rec mergeLines lines line =
//     match lines, line with
//     | [], _ -> [ line ]
//     | ((fromH, toH) as head) :: tail, (fromL, toL) ->
//         if toH + 1 < fromL then head :: mergeLines tail line
//         elif toL + 1 < fromH then line :: lines
//         else mergeLines tail (min fromH fromL, max toH toL)


    public static LList<(T From, T To)> MergeLines<T>(LList<(T From, T To)>? lines, (T From, T To) line)
        where T : INumber<T> =>
        (lines, line) switch
        {
            (null, _) => new LList<(T From, T To)>(line, null),
            ({ Head: (var fromH, var toH) head, Tail: var tail }, var (fromL, toL)) =>
                true switch
                {
                    // head is completely before line -> keep head and continue merging into tail
                    _ when toH + T.One < fromL => new LList<(T From, T To)>(head, MergeLines(tail, line)),

                    // line is completely before head -> prepend line to the whole list
                    _ when toL + T.One < fromH => new LList<(T From, T To)>(line, lines),

                    // overlap/adjacent -> merge and continue with tail
                    _ => MergeLines<T>(tail, (T.Min(fromH, fromL), T.Max(toH, toL)))
                }
        };

// copilot generated code below from F#   
// public static LList<Line> MergeLines(LList<Line>? lines, Line line) =>
//     lines switch
//     {
//         null => new LList<Line>(line, null),
//
//         // head is completely before line -> keep head and continue merging into tail
//         { Head: (var fromH, var toH), Tail: var tail } when toH + 1 < line.From
//             => new LList<(int From, int To)>((fromH, toH), MergeLines(tail, line)),
//
//         // line is completely before head -> prepend line to the whole list
//         { Head: (var fromH2, var toH2) } when line.To + 1 < fromH2
//             => new LList<(int From, int To)>(line, lines),
//
//         // overlap/adjacent -> merge and continue with tail
//         { Head: (var fromH3, var toH3), Tail: var tail3 }
//             => MergeLines(tail3, (Math.Min(fromH3, line.From), Math.Max(toH3, line.To))),
//     };
}