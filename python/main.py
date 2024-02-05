from sys import exit
import finance.portfolio as pf


def main():
    stocks = ['A', 'B', 'C']
    mu = [0.06, 0.10, 0.14]
    sigma = [0.16, 0.22, 0.38]
    fig, _ = pf.plot_mu_sigma(mu, sigma, stocks)
    fig.show()
    input("Close the figure and press a key to continue")


if __name__ == "__main__":
    main()
