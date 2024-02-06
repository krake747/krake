import numpy as np
import matplotlib.pyplot as plt
from matplotlib.figure import Figure
from matplotlib.axes import Axes


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


def plot_mu_sigma(mu: list[float], sigma: list[float], stocks: list[str]) -> tuple[Figure, Axes]:
    fig: Figure
    ax: Axes
    fig, ax = plt.subplots(figsize=(8, 6))
    ax.scatter(sigma, mu, c='black')
    ax.set_xlim(0, 0.45)
    ax.set_ylim(0, 0.25)
    ax.set_ylabel('Mean')
    ax.set_xlabel('Standard Deviation')
    for i, stock in enumerate(stocks):
        ax.annotate(stock, (sigma[i], mu[i]), ha='center', va='bottom', weight='bold')

    return (fig, ax)
