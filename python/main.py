from sys import exit
import finance.portfolio as pf
import numpy as np


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
    print('Covariance matrix: \n', cov)

    sigma_p = pf.expected_std(weights, cov)
    print('Portfolio standard deviation: ', sigma_p)

    mu_mvp, sigma_mvp = pf.mu_sigma_portfolio(weights, means, cov)
    print("Mvp portfolio mu, sigma: ", mu_mvp, sigma_mvp)
    
    # fig, _ = pf.plot_mu_sigma(mu, sigma, stocks)
    # fig.show()
    # input("Close the figure and press a key to continue")


if __name__ == "__main__":
    main()
