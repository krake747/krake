import yfinance as yf
import pandas as pd
import pandas_datareader as pdr
import os


if __name__ == "__main__":
    stocks = ["AAPL", "FE", "WMT"]
    file_path = ".data/stock_prices.csv"

    stock_prices: pd.DataFrame
    if not os.path.isfile(file_path):
        stock_prices = yf.download(stocks, start="2000-01-03", end="2023-12-30")["Adj Close"]
        os.makedirs(".data", exist_ok=True)
        stock_prices.to_csv(file_path, sep=";")
    else:
        print("Read stock prices file")
        stock_prices = pd.read_csv(file_path, sep=";", header=0, index_col=0).fillna(0)

    stock_returns = stock_prices.pct_change()
    stock_means = stock_returns.mean() * 252

    print(stock_means)

    # python version 3.12 removed distutils -> resolve pip install setuptools
    rf = pdr.DataReader("TB3MS", "fred", start="2000-01-01", end="2023-12-30")
    print(rf.head())

    mkt_prices = yf.download("SPY", start="2000-01-03", end="2023-12-30")["Adj Close"]
    print(mkt_prices.head())
