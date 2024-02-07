from sys import exit
from pprint import pprint
import finance.portfolio as pf
import numpy as np
import matplotlib.pyplot as plt


def main():
    stocks = ['A', 'B', 'C']
    means = [0.04, 0.09, 0.12]
    sigma = [0.15, 0.20, 0.35]
    weights = [0.2, 0.3, 0.5]

    mu_p = pf.expected_mu(weights, means)
    print('Expected portfolio return: ', mu_p)

    corr = [[1.0, 0.1, 0.17],
            [0.1, 1.0, 0.26],
            [0.17, 0.26, 1.0]]

    cov = pf.covariance_matrix(sigma, corr)

    sigma_p = pf.expected_std(weights, cov)
    print('Portfolio standard deviation: ', sigma_p)

    mu_mvp, sigma_mvp = pf.mu_sigma_portfolio(weights, means, cov)
    print("Mvp portfolio mu, sigma: ", round(mu_mvp, 4), round(sigma_mvp, 4))

    pf.plot_mu_sigma_with_random_portfolios(means, sigma, cov, stocks, 1000)
    plt.show()
    input("Close the figure and press a key to continue")


if __name__ == "__main__":
    main()
