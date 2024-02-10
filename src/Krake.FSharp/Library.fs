namespace Krake.FSharp

open FSharp.Stats


module Portfolio =
    let expectedMu weights means =
        let w = vector weights
        let mu = vector means
        let muPf = mu.Transpose * w
        muPf

    let expectedStd weights (covMatrix: float seq seq) =
        let w = vector weights
        let cov = matrix covMatrix
        let sigmaPf = (w.Transpose * cov * w) ** 0.5
        sigmaPf

    let covarianceMatrix sigma (corrMatrix: float seq seq) =
        let cov = (Matrix.diag (vector sigma)) * (matrix corrMatrix) * (Matrix.diag (vector sigma))
        Matrix.toJaggedSeq cov

    let muSigma weights means (covMatrix: float seq seq) =
        let muPf = expectedMu weights means
        let sigmaPf = expectedStd weights covMatrix
        muPf, sigmaPf

module PortfolioCharts =
    open Plotly.NET

    let plotMuSigma (means: float seq) (sigma: float seq) (stocks: string seq) =
        let x = sigma
        let y = means
        Chart.Scatter(x, y, MultiText = stocks, TextPosition = StyleParam.TextPosition.TopRight, mode=StyleParam.Mode.Markers_Text)
        |> Chart.withXAxisStyle(TitleText = "Standard Deviation", MinMax = (0, 0.45))
        |> Chart.withYAxisStyle(TitleText = "Mean", MinMax = (0, 0.25))
        |> Chart.show
