namespace Krake.FSharp.PortfolioMetrics

open FSharp.Stats

module PortfolioTheory =

    [<CompiledName("ExpectedMu")>]
    let expectedMu (weights: float array) (means: float array) =
        let w = vector weights
        let mu = vector means
        let muPf = mu.Transpose * w

        muPf

    [<CompiledName("ExpectedStd")>]
    let expectedStd (weights: float array) (covMatrix: float array array) =
        let w = vector weights
        let cov = matrix covMatrix
        let sigmaPf = (w.Transpose * cov * w) ** 0.5

        sigmaPf

    [<CompiledName("CovarianceMatrix")>]
    let covarianceMatrix (sigma: float array) (corrMatrix: float array array) =
        let cov =
            (Matrix.diag (vector sigma))
            * (matrix corrMatrix)
            * (Matrix.diag (vector sigma))

        Matrix.toJaggedArray cov

    [<CompiledName("MuSigma")>]
    let muSigma weights (means: float array) (covMatrix: float array array) =
        let muPf = expectedMu weights means
        let sigmaPf = expectedStd weights covMatrix

        muPf, sigmaPf
