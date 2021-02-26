﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InvestmentChecker2
{
    public partial class StockRow : UserControl
    {
        int id;
        int fracDigits;

        Color inProfit = Color.FromArgb(255, 53, 255, 205);
        Color inLoss = Color.FromArgb(255, 255, 73, 92);

        private double priceDifference;

        public double PriceDifference
        {
            get { return priceDifference; }
            set { priceDifference = value; }
        }

        public StockRow(Stock stock)
        {
            InitializeComponent();
            id = stock.id;
            fracDigits = App.NUMBER_OF_FRAC_DIGITS;

            labelTicker.Text = stock.ticker;
            labelName.Text = stock.name;
            labelQuantity.Text = stock.quantity.ToString();
            labelBuyingPrice.Text = stock.buyingPrice.ToString();
            labelCurrentPrice.Text = Math.Round(stock.CurrentPrice, fracDigits).ToString();
            labelPriceDifference.Text = Math.Round(stock.PriceDifference, fracDigits).ToString();
            labelBuyingMarketValue.Text = stock.BuyingMarketValue.ToString();
            labelCurrentMarketValue.Text = Math.Round(stock.CurrentMarketValue, fracDigits).ToString();
            labelMarketValueDifference.Text = Math.Round(stock.MarketValueDifference, fracDigits).ToString();
            labelChangePercent.Text = Math.Round(stock.ChangePercent, fracDigits).ToString() + "%";
            labelDateBought.Text = stock.dateBought.ToShortDateString().ToString();

            // Might not be the best solution
            if (stock.PriceDifference > 0)
            {
                labelPriceDifference.ForeColor = inProfit;
                labelMarketValueDifference.ForeColor = inProfit;
                labelChangePercent.ForeColor = inProfit;
            }
            else if (stock.PriceDifference < 0)
            {
                labelPriceDifference.ForeColor = inLoss;
                labelMarketValueDifference.ForeColor = inLoss;
                labelChangePercent.ForeColor = inLoss;
            }

        }

        private void RemoveStock(object sender, EventArgs e)
        {
            // Delete from the stock's list
            // Find the index based on the stock's id
            int foundIndex = 0;
            while (id != App.currentStocks[foundIndex].id)
            {
                foundIndex++;
            }
            App.currentStocks.RemoveAt(foundIndex);

            // Remove the control
            Parent.Controls.Remove(this);

            // Schedule for deletion in stocks.csv
            App.stocksToBeDeleted = true;

        }

        private void StockRowLoad(object sender, EventArgs e)
        {

        }
    }
}
