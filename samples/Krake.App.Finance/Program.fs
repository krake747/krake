open Krake.FSharp
open Plotly.NET

let stocks = [| "A" ; "B" ; "C" |]
let means = [| 0.04 ; 0.09 ; 0.12 |]
let sigma = [| 0.15 ; 0.20 ; 0.35 |]
let weights = [| 0.2 ; 0.3 ; 0.5 |]
let rf = 0.02

let corrMatrix = [| [| 1.0 ; 0.1 ; 0.17 |] ; [| 0.1 ; 1.0 ; 0.26 |] ; [| 0.17 ; 0.26 ; 1.0 |] |]

let covMatrix = Portfolio.covarianceMatrix sigma corrMatrix

let muPf, sigmaPf = Portfolio.muSigma weights means covMatrix

printfn $"Expected portfolio return {muPf} and standard deviation: {sigmaPf}"

[
    PortfolioCharts.chartRandomPortfolios means covMatrix 1000
    |> Chart.withTraceInfo "random"
    PortfolioCharts.chartMuSigma means sigma stocks
    |> Chart.withTraceInfo "mu-sigma"
]
|> Chart.combine
|> Chart.show
