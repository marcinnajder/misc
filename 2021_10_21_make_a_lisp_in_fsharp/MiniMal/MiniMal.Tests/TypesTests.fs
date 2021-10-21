namespace MiniMal.Tests

open Microsoft.VisualStudio.TestTools.UnitTesting
open Types
open TestUtils

[<TestClass>]
type TypesTests() =

    [<TestMethod>]
    member this.MalEqualTest() =
        let fn = Fn((fun args -> Nil), false)
        assertMalNotEquals fn fn

        assertMalEquals False False
        assertMalNotEquals False fn

        assertMalEquals True True
        assertMalNotEquals True fn

        assertMalEquals Nil Nil
        assertMalNotEquals Nil fn

        assertMalEquals (Str("a")) (Str("a"))
        assertMalNotEquals (Str("a")) (Str("b"))
        assertMalNotEquals (Str("a")) fn

        assertMalEquals (Number(1.)) (Number(1.))
        assertMalNotEquals (Number(1.)) (Number(2.))
        assertMalNotEquals (Number(1.)) fn

        assertMalEquals (Symbol("a")) (Symbol("a"))
        assertMalNotEquals (Symbol("a")) (Symbol("b"))
        assertMalNotEquals (Symbol("a")) fn

        let mals = [ Str("name"); Nil ]
        assertMalEquals (MalList(mals, ListType.List)) (MalList(mals |> (List.map id), ListType.List))
        assertMalEquals (MalList(mals, ListType.List)) (MalList(mals |> (List.map id), ListType.Vector))
        assertMalNotEquals (MalList(mals, ListType.List)) (MalList(mals |> (List.take 1), ListType.List))
        assertMalNotEquals (MalList(mals, ListType.List)) (MalList(mals |> List.rev, ListType.List))
        assertMalNotEquals (MalList(mals, ListType.List)) fn

        let map = Map [ "name", Nil ]
        let malMap = MalMap(map)
        assertMalEquals malMap (MalMap(map |> Map.toList |> (List.map id) |> Map.ofList))

        assertMalNotEquals
            malMap
            (MalMap(
                map
                |> Map.toList
                |> (List.map (fun (k, v) -> (k + "!", v)))
                |> Map.ofList
            ))


        assertMalNotEquals malMap (MalMap(Map []))
        assertMalNotEquals malMap fn

        let map1 =
            Map([ ("name", Nil); ("age", Number(1.)) ])
        let map2 =
            Map([ ("age", Number(1.)); ("name", Nil) ])
        assertMalEquals (MalMap(map1)) (MalMap(map2))
