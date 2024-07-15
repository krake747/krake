module Krake.FSharp.PortfolioMetrics

open Krake.FSharp.PortfolioMetrics
open NUnit.Framework.Constraints
open Xunit
open FsUnit

type Range = Within of float * float
let (+/-) (a: float) b = Within(a, b)

let equal x =
    match box x with
    | :? Range as r ->
        let (Within(x, within)) = r
        EqualConstraint(x).Within(within)
    | _ -> EqualConstraint(x)

module ``modern portfolio theory`` =
    // Arrange
    let stocks = [| "A" ; "B" ; "C" |]
    let means = [| 0.04 ; 0.09 ; 0.12 |]
    let sigma = [| 0.15 ; 0.20 ; 0.35 |]
    let weights = [| 0.2 ; 0.3 ; 0.5 |]
    let rf = 0.02
    let corrMatrix = [| [| 1.0 ; 0.1 ; 0.17 |] ; [| 0.1 ; 1.0 ; 0.26 |] ; [| 0.17 ; 0.26 ; 1.0 |] |]

    let covMatrix = PortfolioTheory.covarianceMatrix sigma corrMatrix

    [<Fact>]
    let ``portfolio mu test`` () =
        let expectedMu = 0.0950
        let actual = PortfolioTheory.muSigma weights means covMatrix
        actual |> fst |> should equal expectedMu

    [<Fact>]
    let ``portfolio sigma test`` () =
        let expectedStd = 0.2067
        let actual = PortfolioTheory.muSigma weights means covMatrix
        actual |> snd |> should equal (expectedStd +/- 0.0001)
