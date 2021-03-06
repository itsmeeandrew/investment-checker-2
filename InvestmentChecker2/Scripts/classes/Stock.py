from yahooquery import Ticker

class Stock:
    def __init__(self, ticker):
        self.ticker = ticker
        self.get_stock()

    def get_stock_info(self):
        return f"{self.ticker};{self.name};{self.current_price};{self.currency}"

    def get_stock_price(self):
        return self.current_price

    def get_stock(self):
        stock_info = Ticker(self.ticker)
        data = stock_info.price[self.ticker]
        
        try:
            self.name = data["shortName"]
            self.current_price = data["regularMarketPrice"]
            self.currency = data["currency"]
        except:
            self.name = -1
            self.current_price = -1
            self.currency = -1
