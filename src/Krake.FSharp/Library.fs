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

    let muSigmaPortfolio weights means (covMatrix: float seq seq) =
        let muPf = expectedMu weights means
        let sigmaPf = expectedStd weights covMatrix
        muPf, sigmaPf
