module AlgorithmsAndDataStructures.Utils

let (===) actual expected = if actual = expected then () else failwithf "assertion failed: %A <> %A" actual expected

let flip f x y = f y x
