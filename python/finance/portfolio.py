import numpy as np
import matplotlib.pyplot as plt


def expected_mu(weights: list[float], means: list[float]) -> float:
    w = np.array(weights)
    mu = np.array(means)
    mu_p = mu @ w.T
    return mu_p.item()


def expected_std(weights: list[float], cov_matrix: list[list[float]]) -> float:
    w = np.array(weights)
    Cov = np.array(cov_matrix)
    sigma_p = (w @ Cov @ w) ** 0.5
    return sigma_p.item()


def covariance_matrix(sigma: list[float], corr_matrix: list[list[float]]) -> list[list[float]]:
    Cov = np.diag(sigma) @ np.array(corr_matrix) @ np.diag(sigma)
    return Cov.tolist()


def correlation_matrix(sigma: list[float], cov_matrix: list[list[float]]) -> list[list[float]]:
    raise NotImplementedError()


def mu_sigma_portfolio(weights: list[float], means: list[float], cov_matrix: list[list[float]]) -> tuple[float, float]:
    w = np.array(weights)
    mu = np.array(means)
    cov = np.array(cov_matrix)
    mu_p = mu @ w.T
    sigma_p = (w @ cov @ w.T) ** 0.5
    return mu_p.item(), sigma_p.item()


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


def plot_random_portfolios(
        means: list[float],
        cov_matrix: list[list[float]],
        n_simulations: int) -> None:

    def random_weights(n_assets: int) -> list[float]:
        k = np.random.randn(n_assets)
        return (k / sum(k)).tolist()

    mu_p_sims = []
    sigma_p_sims = []
    n_assets = len(means)
    for _ in range(n_simulations):
        w = random_weights(n_assets)
        mu_p, sigma_p = mu_sigma_portfolio(w, means, cov_matrix)
        mu_p_sims.append(mu_p)
        sigma_p_sims.append(sigma_p)

    plt.scatter(sigma_p_sims, mu_p_sims, s=12)
    return None


def _compute_ABC(means, cov_matrix):
    n_assets = len(means)
    mu = np.array(means)
    cov_inv = np.linalg.inv(cov_matrix)
    ones = np.ones(n_assets)
    A = ones @ cov_inv @ ones
    B = ones @ cov_inv @ mu
    C = mu @ cov_inv @ mu
    return A, B, C


def plot_min_var_frontier(means: list[float], cov_matrix: list[list[float]]) -> None:

    mu = np.array(means)
    A, B, C = _compute_ABC(mu, cov_matrix)
    # efficient frontier
    y1 = np.linspace(B / A, 0.45, 100)
    x1 = np.sqrt((A * y1 * y1 - 2 * B * y1 + C) / (A * C - B * B))

    # bottom half
    y2 = np.linspace(0, B / A, 100)
    x2 = np.sqrt((A * y2 * y2 - 2 * B * y2 + C) / (A * C - B * B))

    # y = np.linspace(0, 0.45, 100)
    # x = np.sqrt((A * y * y - 2 * B * y + C) / (A * C - B * B))
    plt.plot(x1, y1, color='black', lw=2, label='Efficient Frontier')
    plt.plot(x2, y2, color='grey', lw=2, ls='--')
    plt.legend()
    return None


def plot_capital_allocation_line(rf: float, means: list[float], cov_matrix: list[list[float]]) -> None:
    mu = np.array(means)
    A, B, C = _compute_ABC(mu, cov_matrix)
    x = np.linspace(0, 0.45, 100)
    y = rf + x * (C - 2 * B * rf + A * rf**2)**0.5
    plt.plot(x, y, color='black', lw=2)
    plt.annotate('$r_f$', xy=(0, rf), xycoords='data', xytext=(0, rf), ha='right', va='center', size=12)
    return None
