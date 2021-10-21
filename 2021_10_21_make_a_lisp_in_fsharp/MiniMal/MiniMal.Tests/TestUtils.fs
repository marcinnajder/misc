module TestUtils

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open Env
open Types
open Core

let emptyEnv outer = Env(Map [], outer)

let defaultEnv outer = Env(ns, outer)

let assertThrowsException f =
    Assert.ThrowsException<Exception>(fun () -> f () |> ignore)
    |> ignore

let assertMalNotEquals expected actual =
    Assert.IsFalse((malEquals expected actual), $"Expected:<{expected}>. Actual:<{actual}>.")

let assertMalEquals expected actual =
    Assert.IsTrue((malEquals expected actual), $"Expected:<{expected}>. Actual:<{actual}>.")
