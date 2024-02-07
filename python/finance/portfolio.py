import numpy as np
import matplotlib.pyplot as plt


def expected_mu(weights: list[float], means: list[float]) -> float:
    w = np.array(weights)
    mu = np.array(means)
    mu_p = mu @ w.T
    return float(mu_p)


def expected_std(weights: list[float], cov_matrix: list[list[float]]) -> float:
    w = np.array(weights)
    cov = np.array(cov_matrix)
    sigma_p = (w @ cov @ w) ** 0.5
    return float(sigma_p)


def covariance_matrix(sigma: list[float], corr_matrix: list[list[float]]) -> list[list[float]]:
    cov = np.diag(sigma) @ np.array(corr_matrix) @ np.diag(sigma)
    return cov.tolist()


def correlation_matrix(sigma: list[float], cov_matrix: list[list[float]]) -> list[list[float]]:
    raise NotImplementedError()


def mu_sigma_portfolio(weights: list[float], means: list[float], cov_matrix: list[list[float]]) -> tuple[float, float]:
    w = np.array(weights)
    mu = np.array(means)
    cov = np.array(cov_matrix)
    mu_p = mu @ w.T
    sigma_p = (w @ cov @ w.T) ** 0.5
    return float(mu_p), float(sigma_p)


def plot_mu_sigma(means: list[float], sigma: list[float], stocks: list[str]) -> None:
    plt.figure(figsize=(8, 6))
    plt.scatter(sigma, means, c='black')
    plt.xlim(0, 0.45)
    plt.ylim(0, 0.25)
    plt.ylabel('Mean')
    plt.xlabel('Standard Deviation')
    for i, stock in enumerate(stocks):
        plt.annotate(stock, (sigma[i], means[i]), ha='center', va='bottom', weight='bold')
    return None


def plot_mu_sigma_with_random_portfolios(
        means: list[float],
        sigma: list[float],
        cov_matrix: list[list[float]],
        stocks: list[str],
        n_simulations: int) -> None:

    def random_weights(n_assets: int) -> list[float]:
        k = np.random.randn(n_assets)
        return list(k / sum(k))

    mu_p_sims = []
    sigma_p_sims = []
    n_assets = len(means)
    for _ in range(n_simulations):
        w = random_weights(n_assets)
        mu_p, sigma_p = mu_sigma_portfolio(w, means, cov_matrix)
        mu_p_sims.append(mu_p)
        sigma_p_sims.append(sigma_p)

    plot_mu_sigma(means, sigma, stocks)
    plt.scatter(sigma_p_sims, mu_p_sims, s=12)
    return None
