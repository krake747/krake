from sys import exit
import keyboard
import matplotlib.pyplot as plt
import finance.portfolio as pf


def main():
    stocks = ['A', 'B', 'C']
    mu = [0.06, 0.10, 0.14]
    sigma = [0.16, 0.22, 0.38]
    fig, _ = pf.plot_mu_sigma(mu, sigma, stocks)
    plt.show(block=True)
    fig.show()

    print("Exit (y)")
    if keyboard.read_key() == "y":
        exit()


if __name__ == "__main__":
    main()
