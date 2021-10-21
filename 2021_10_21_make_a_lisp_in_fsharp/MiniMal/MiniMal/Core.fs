module Core

open Types
open Printer

let internal executeArithmeticFn args operation =
    match args with
    | _ :: _ :: _ ->
        List.reduce
            (fun totalMal nextMal ->
                match (totalMal, nextMal) with
                | (Number (total), Number (next)) -> Number(operation total next)
                | _ ->
                    let m =
                        match totalMal with
                        | Number (_) -> nextMal
                        | _ -> totalMal
                    failwith
                        $"""All arguments of arithmetic operations must be of the 'Number' type, but got an argument '{(printStr m)}' in {joinWithSeparator args (Some(","))}""")
            args
    | _ ->
        failwith
            $"""Arithmetic operation required at least two arguments, but got '{List.length args}', arguments: {joinWithSeparator args (Some(","))}"""


let internal executeComparisonFn args comparison =
    match args with
    | [ Number (value1); Number (value2) ] -> if comparison value1 value2 then True else False
    | _ ->
        failwith
            $"""Number comparison operation requires two arguments of type 'Number', but got {joinWithSeparator args (Some(","))}"""


let ns =
    Map [ ("+", Fn((fun args -> executeArithmeticFn args (+)), false))
          ("-", Fn((fun args -> executeArithmeticFn args (-)), false))
          ("*", Fn((fun args -> executeArithmeticFn args (*)), false))
          ("/", Fn((fun args -> executeArithmeticFn args (/)), false))

          ("<", Fn((fun args -> executeComparisonFn args (<)), false))
          ("<=", Fn((fun args -> executeComparisonFn args (<=)), false))
          (">", Fn((fun args -> executeComparisonFn args (>)), false))
          (">=", Fn((fun args -> executeComparisonFn args (>=)), false))

           ]
