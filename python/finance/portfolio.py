import matplotlib.pyplot as plt
from matplotlib.figure import Figure
from matplotlib.axes import Axes


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
