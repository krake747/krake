namespace Krake.FSharp

open FSharp.Stats
open Plotly.NET.StyleParam


module Portfolio =
    let expectedMu (weights: float seq) (means: float seq) =
        let w = vector weights
        let mu = vector means
        let muPf = mu.Transpose * w

        muPf

    let expectedStd (weights: float list) (covMatrix: float list list) =
        let w = vector weights
        let cov = matrix covMatrix
        let sigmaPf = (w.Transpose * cov * w) ** 0.5

        sigmaPf

    let covarianceMatrix (sigma: float list) (corrMatrix: float list list) =
        let cov =
            (Matrix.diag (vector sigma))
            * (matrix corrMatrix)
            * (Matrix.diag (vector sigma))

        Matrix.toJaggedSeq cov |> JaggedList.ofJaggedSeq

    let muSigma weights (means: float list) (covMatrix: float list list) =
        let muPf = expectedMu weights means
        let sigmaPf = expectedStd weights covMatrix

        muPf, sigmaPf

module PortfolioCharts =
    open Plotly.NET
    open Plotly.NET.TraceObjects
    open FSharp.Stats.Distributions

    let chartMuSigma (means: float list) (sigma: float list) (stocks: string list) =
        Chart.Scatter(
            x = sigma,
            y = means,
            MultiText = stocks,
            TextPosition = TextPosition.TopRight,
            Marker = Marker.init (Color = Color.fromKeyword Black),
            mode = Mode.Markers_Text
        )
        |> Chart.withTitle "Mu-Sigma Chart"
        |> Chart.withXAxisStyle (TitleText = "Standard Deviation", MinMax = (0, 0.45))
        |> Chart.withYAxisStyle (TitleText = "Mean", MinMax = (0, 0.25))


    let chartRandomPortfolios (means: float list) (covMatrix: float list list) (nSimulations: int) =
        let randomWeights (nAssets: int) : float list =
            let normal = Continuous.Normal.Init 0 1
            let k = Vector.init nAssets (fun _ -> normal.Sample())
            let sum = Vector.sum k
            k |> Vector.map (fun x -> x / sum) |> Vector.toArray |> Array.toList

        let nAssets = Seq.length means

        let simulations =
            [ 0..nSimulations ]
            |> List.map (fun _ -> Portfolio.muSigma (randomWeights nAssets) means covMatrix)

        Chart.Scatter(x = (List.map snd simulations), y = (List.map fst simulations), mode = Mode.Markers_Text)
        |> Chart.withXAxisStyle (TitleText = "Standard Deviation", MinMax = (0, 0.45))
        |> Chart.withYAxisStyle (TitleText = "Mean", MinMax = (0, 0.25))
