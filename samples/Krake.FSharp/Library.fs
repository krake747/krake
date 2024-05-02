namespace Krake.FSharp

open FSharp.Stats
open Plotly.NET.StyleParam


module Portfolio =
    let expectedMu (weights: float array) (means: float array) =
        let w = vector weights
        let mu = vector means
        let muPf = mu.Transpose * w

        muPf

    let expectedStd (weights: float array) (covMatrix: float array array) =
        let w = vector weights
        let cov = matrix covMatrix
        let sigmaPf = (w.Transpose * cov * w) ** 0.5

        sigmaPf

    let covarianceMatrix (sigma: float array) (corrMatrix: float array array) =
        let cov =
            (Matrix.diag (vector sigma))
            * (matrix corrMatrix)
            * (Matrix.diag (vector sigma))

        Matrix.toJaggedArray cov

    let muSigma weights (means: float array) (covMatrix: float array array) =
        let muPf = expectedMu weights means
        let sigmaPf = expectedStd weights covMatrix

        muPf, sigmaPf

module PortfolioCharts =
    open Plotly.NET
    open Plotly.NET.TraceObjects
    open FSharp.Stats.Distributions

    let chartMuSigma (means: float array) (sigma: float array) (stocks: string array) =
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


    let chartRandomPortfolios (means: float array) (covMatrix: float array array) (nSimulations: int) =
        let randomWeights (nAssets: int) : float array =
            let normal = Continuous.Normal.Init 0 1
            let k = Vector.init nAssets (fun _ -> normal.Sample())
            let sum = Vector.sum k
            k |> Vector.map (fun x -> x / sum) |> Vector.toArray

        let nAssets = Seq.length means

        let simulations =
            [ 0..nSimulations ]
            |> List.map (fun _ -> Portfolio.muSigma (randomWeights nAssets) means covMatrix)

        Chart.Scatter(x = (List.map snd simulations), y = (List.map fst simulations), mode = Mode.Markers_Text)
        |> Chart.withXAxisStyle (TitleText = "Standard Deviation", MinMax = (0, 0.45))
        |> Chart.withYAxisStyle (TitleText = "Mean", MinMax = (0, 0.25))
