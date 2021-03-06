﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace InvestmentChecker2
{
    public partial class MainWindow : Form
    {
        // Constants
        int STOCK_ROW_HEIGHT = 65;
        int STOCK_ROW_LEFT_MARGIN = 5;
        Color oddRow = Color.FromArgb(255, 24, 24, 24);
        Color evenRow = Color.FromArgb(255, 32, 32, 32);

        string autoUpdateBaseText = "Auto update is ";
        Color autoUpdateOn = Color.FromArgb(255, 20, 195, 120);
        Color autoUpdateOff = Color.FromArgb(255, 165, 36, 61);

        // StockRow controls
        List<StockRow> stockRows = new List<StockRow>();
        List<CurrencyExchangeRow> currencyExchangeRows = new List<CurrencyExchangeRow>();

        // Ordering
        string currentOrderByValue = "";
        string currentOrderByStyle = "";
        
        public MainWindow()
        {
            InitializeComponent();

            // Read settings
            if (File.Exists(App.SETTINGS_PATH))
            {
                App.ReadSettings();
            } else
            {
                App.CreateDefaultSettings();
                App.ReadSettings();
            }

            // Check if python is available
            if (string.IsNullOrEmpty(App.settings.PythonExeFullPath))
            {
                AddPythonExeFullPathWindow window = new AddPythonExeFullPathWindow();
                DialogResult dr = window.ShowDialog();
                if (dr != DialogResult.OK)
                {
                    Environment.Exit(0);
                }
            }

            // Load profiles
            App.LoadProfileNames();
            comboBoxSelectProfile.DataSource = App.profileNames;

            // Icons
            btnAddProfile.Text = "\uE710";
        }

        private void DisplayCurrentStocks()
        {
            RemoveStockRows();
            ClearStockRows();

            // Display new stocks
            for (int i = 0; i < App.currentStocks.Count; i++)
            {
                Stock stock = App.currentStocks[i];
                StockRow stockRow = new StockRow(stock);
                stock.stockRow = stockRow;

                stockRow.Top = i * STOCK_ROW_HEIGHT;
                stockRow.Left = STOCK_ROW_LEFT_MARGIN;
                stockRow.BackColor = i % 2 == 0 ? evenRow : oddRow;

                stockRows.Add(stockRow);
                panelStocks.Controls.Add(stockRow);

                // Set next stock's id
                // There might be a better solution to set this
                App.NEXT_STOCK_ID = stock.id;
            }
            App.NEXT_STOCK_ID++;
        }

        private void RemoveStockRows()
        {
            // Clear out previously displayed stocks
            // Alternatively I could use 'panelStocks.Controls.Clear();', but I'd need the stockRow list anyways
            foreach (StockRow stockRow in stockRows)
            {
                panelStocks.Controls.Remove(stockRow);
            }
        }

        private void ClearStockRows()
        {
            stockRows.Clear();
        }

        private void ReorderStockRows(object sender, EventArgs e)
        {
            RemoveStockRows();
            
            Button orderBy = sender as Button;
            List<StockRow> orderedList;

            switch (orderBy.Text)
            {
                case "Ticker":
                    if (currentOrderByValue == "Ticker")
                    {
                        if (currentOrderByStyle == "descending")
                        {
                            orderedList = stockRows.OrderBy(stockRow => stockRow.stock.ticker).ToList();
                            currentOrderByStyle = "ascending";
                        } else
                        {
                            orderedList = stockRows.OrderByDescending(stockRow => stockRow.stock.ticker).ToList();
                            currentOrderByStyle = "descending";
                        }
                    }
                    else
                    {
                        orderedList = stockRows.OrderBy(stockRow => stockRow.stock.ticker).ToList();
                        currentOrderByValue = "Ticker";
                        currentOrderByStyle = "ascending";
                    }
                    break;
                case "Name":
                    if (currentOrderByValue == "Name")
                    {
                        if (currentOrderByStyle == "descending")
                        {
                            orderedList = stockRows.OrderBy(stockRow => stockRow.stock.name).ToList();
                            currentOrderByStyle = "ascending";
                        }
                        else
                        {
                            orderedList = stockRows.OrderByDescending(stockRow => stockRow.stock.name).ToList();
                            currentOrderByStyle = "descending";
                        }
                    }
                    else
                    {
                        orderedList = stockRows.OrderBy(stockRow => stockRow.stock.name).ToList();
                        currentOrderByValue = "Name";
                        currentOrderByStyle = "ascending";
                    }
                    break;
                case "Quantity":
                    if (currentOrderByValue == "Quantity")
                    {
                        if (currentOrderByStyle == "descending")
                        {
                            orderedList = stockRows.OrderBy(stockRow => stockRow.stock.quantity).ToList();
                            currentOrderByStyle = "ascending";
                        }
                        else
                        {
                            orderedList = stockRows.OrderByDescending(stockRow => stockRow.stock.quantity).ToList();
                            currentOrderByStyle = "descending";
                        }
                    }
                    else
                    {
                        orderedList = stockRows.OrderBy(stockRow => stockRow.stock.quantity).ToList();
                        currentOrderByValue = "Quantity";
                        currentOrderByStyle = "ascending";
                    }
                    break;
                case "Buying Price":
                    if (currentOrderByValue == "Buying Price")
                    {
                        if (currentOrderByStyle == "descending")
                        {
                            orderedList = stockRows.OrderBy(stockRow => stockRow.stock.buyingPrice).ToList();
                            currentOrderByStyle = "ascending";
                        }
                        else
                        {
                            orderedList = stockRows.OrderByDescending(stockRow => stockRow.stock.buyingPrice).ToList();
                            currentOrderByStyle = "descending";
                        }
                    }
                    else
                    {
                        orderedList = stockRows.OrderBy(stockRow => stockRow.stock.buyingPrice).ToList();
                        currentOrderByValue = "Buying Price";
                        currentOrderByStyle = "ascending";
                    }
                    break;
                case "Current Price":
                    if (currentOrderByValue == "Current Price")
                    {
                        if (currentOrderByStyle == "descending")
                        {
                            orderedList = stockRows.OrderBy(stockRow => stockRow.stock.CurrentPrice).ToList();
                            currentOrderByStyle = "ascending";
                        }
                        else
                        {
                            orderedList = stockRows.OrderByDescending(stockRow => stockRow.stock.CurrentPrice).ToList();
                            currentOrderByStyle = "descending";
                        }
                    }
                    else
                    {
                        orderedList = stockRows.OrderBy(stockRow => stockRow.stock.CurrentPrice).ToList();
                        currentOrderByValue = "Current Price";
                        currentOrderByStyle = "ascending";
                    }
                    break;
                case "Price Difference":
                    if (currentOrderByValue == "Price Difference")
                    {
                        if (currentOrderByStyle == "descending")
                        {
                            orderedList = stockRows.OrderBy(stockRow => stockRow.stock.PriceDifference).ToList();
                            currentOrderByStyle = "ascending";
                        }
                        else
                        {
                            orderedList = stockRows.OrderByDescending(stockRow => stockRow.stock.PriceDifference).ToList();
                            currentOrderByStyle = "descending";
                        }
                    }
                    else
                    {
                        orderedList = stockRows.OrderBy(stockRow => stockRow.stock.PriceDifference).ToList();
                        currentOrderByValue = "Price Difference";
                        currentOrderByStyle = "ascending";
                    }
                    break;
                case "Buying Market Value":
                    if (currentOrderByValue == "Buying Market Value")
                    {
                        if (currentOrderByStyle == "descending")
                        {
                            orderedList = stockRows.OrderBy(stockRow => stockRow.stock.BuyingMarketValue).ToList();
                            currentOrderByStyle = "ascending";
                        }
                        else
                        {
                            orderedList = stockRows.OrderByDescending(stockRow => stockRow.stock.BuyingMarketValue).ToList();
                            currentOrderByStyle = "descending";
                        }
                    }
                    else
                    {
                        orderedList = stockRows.OrderBy(stockRow => stockRow.stock.BuyingMarketValue).ToList();
                        currentOrderByValue = "Buying Market Value";
                        currentOrderByStyle = "ascending";
                    }
                    break;
                case "Current Market Value":
                    if (currentOrderByValue == "Current Market Value")
                    {
                        if (currentOrderByStyle == "descending")
                        {
                            orderedList = stockRows.OrderBy(stockRow => stockRow.stock.CurrentMarketValue).ToList();
                            currentOrderByStyle = "ascending";
                        }
                        else
                        {
                            orderedList = stockRows.OrderByDescending(stockRow => stockRow.stock.CurrentMarketValue).ToList();
                            currentOrderByStyle = "descending";
                        }
                    }
                    else
                    {
                        orderedList = stockRows.OrderBy(stockRow => stockRow.stock.CurrentMarketValue).ToList();
                        currentOrderByValue = "Current Market Value";
                        currentOrderByStyle = "ascending";
                    }
                    break;
                case "M. V. Difference":
                    if (currentOrderByValue == "M. V. Difference")
                    {
                        if (currentOrderByStyle == "descending")
                        {
                            orderedList = stockRows.OrderBy(stockRow => stockRow.stock.MarketValueDifference).ToList();
                            currentOrderByStyle = "ascending";
                        }
                        else
                        {
                            orderedList = stockRows.OrderByDescending(stockRow => stockRow.stock.MarketValueDifference).ToList();
                            currentOrderByStyle = "descending";
                        }
                    }
                    else
                    {
                        orderedList = stockRows.OrderBy(stockRow => stockRow.stock.MarketValueDifference).ToList();
                        currentOrderByValue = "M. V. Difference";
                        currentOrderByStyle = "ascending";
                    }
                    break; ;
                case "Change (%)":
                    if (currentOrderByValue == "Change (%)")
                    {
                        if (currentOrderByStyle == "descending")
                        {
                            orderedList = stockRows.OrderBy(stockRow => stockRow.stock.ChangePercent).ToList();
                            currentOrderByStyle = "ascending";
                        }
                        else
                        {
                            orderedList = stockRows.OrderByDescending(stockRow => stockRow.stock.ChangePercent).ToList();
                            currentOrderByStyle = "descending";
                        }
                    }
                    else
                    {
                        orderedList = stockRows.OrderBy(stockRow => stockRow.stock.ChangePercent).ToList();
                        currentOrderByValue = "Change (%)";
                        currentOrderByStyle = "ascending";
                    }
                    break;
                case "Currency":
                    if (currentOrderByValue == "Currency")
                    {
                        if (currentOrderByStyle == "descending")
                        {
                            orderedList = stockRows.OrderBy(stockRow => stockRow.stock.currency).ToList();
                            currentOrderByStyle = "ascending";
                        }
                        else
                        {
                            orderedList = stockRows.OrderByDescending(stockRow => stockRow.stock.currency).ToList();
                            currentOrderByStyle = "descending";
                        }
                    }
                    else
                    {
                        orderedList = stockRows.OrderBy(stockRow => stockRow.stock.currency).ToList();
                        currentOrderByValue = "Currency";
                        currentOrderByStyle = "ascending";
                    }
                    break;
                case "Date Bought":
                    if (currentOrderByValue == "Date Bought")
                    {
                        if (currentOrderByStyle == "descending")
                        {
                            orderedList = stockRows.OrderBy(stockRow => stockRow.stock.dateBought).ToList();
                            currentOrderByStyle = "ascending";
                        }
                        else
                        {
                            orderedList = stockRows.OrderByDescending(stockRow => stockRow.stock.dateBought).ToList();
                            currentOrderByStyle = "descending";
                        }
                    }
                    else
                    {
                        orderedList = stockRows.OrderBy(stockRow => stockRow.stock.dateBought).ToList();
                        currentOrderByValue = "Date Bought";
                        currentOrderByStyle = "ascending";
                    }
                    break;
                default:
                    orderedList = stockRows;
                    break;
            }

            for (int i = 0; i < orderedList.Count; i++)
            {
                StockRow stockRow = orderedList[i];

                stockRow.Top = i * STOCK_ROW_HEIGHT;
                stockRow.Left = STOCK_ROW_LEFT_MARGIN;
                stockRow.BackColor = i % 2 == 0 ? evenRow : oddRow;
                panelStocks.Controls.Add(stockRow);
            }
        }

        private void DisplayCurrentCurrencyExchanges()
        {
            foreach (CurrencyExchangeRow currencyExchangeRow in currencyExchangeRows)
            {
                panelCurrencyExhanges.Controls.Remove(currencyExchangeRow);
            }

            for (int i = 0; i < App.currentCurrencyExchanges.Count; i++)
            {
                CurrencyExchange ce = App.currentCurrencyExchanges[i];
                CurrencyExchangeRow cer = new CurrencyExchangeRow(ce);
                cer.Top = i * STOCK_ROW_HEIGHT;
                cer.BackColor = i % 2 == 0 ? evenRow : oddRow;

                currencyExchangeRows.Add(cer);
                panelCurrencyExhanges.Controls.Add(cer);

                App.NEXT_CURRENCY_EXCHANGE_ID = ce.id;
            }
            App.NEXT_CURRENCY_EXCHANGE_ID++;
        }

        public void SetAutoUpdateTimer()
        {
            autoUpdateTimer.Stop();
            
            if (App.settings.AutoUpdate)
            {
                labelAutoUpdate.Text = autoUpdateBaseText + $"ON (updating every {App.settings.AutoUpdateInterval} seconds)";
                labelAutoUpdate.ForeColor = autoUpdateOn;
                autoUpdateTimer.Start();
                autoUpdateTimer.Interval = App.settings.AutoUpdateInterval * 1000;
            }
            else
            {
                labelAutoUpdate.Text = autoUpdateBaseText + "OFF";
                labelAutoUpdate.ForeColor = autoUpdateOff;
            }
        }

        private void UpdateCurrentPrices(object sender, EventArgs e)
        {
            App.GetCurrentPricesForCurrentStocks();
        }

        private void OnProfileChange(object sender, EventArgs e)
        {
            if (App.currentProfile != null && App.stocksToBeDeleted)
            {
                App.UpdateStocksCSV();
            }

            if (App.currentProfile != null && App.currencyExchangeToBeDeleted)
            {
                App.UpdateCurrencyExchangesCSV();
            }

            App.currentProfile = comboBoxSelectProfile.SelectedValue.ToString();
            App.ReadStocksForProfile();
            App.ReadCurrencyExchangesForProfile();
            DisplayCurrentStocks();
            DisplayCurrentCurrencyExchanges();
            App.GetCurrentPricesForCurrentStocks();

            SetAutoUpdateTimer();
        }

        // Open new windows
        private void OpenAddProfileWindow(object sender, EventArgs e)
        {
            AddProfileWindow window = new AddProfileWindow();
            window.Show();
        }

        private void OpenSearchStockWindow(object sender, EventArgs e)
        {
            SearchStockWindow window = new SearchStockWindow();
            window.Show();
        }

        private void OpenSettingsWindow(object sender, EventArgs e)
        {
            SettingsWindow window = new SettingsWindow();
            DialogResult dr = window.ShowDialog();
            if (dr == DialogResult.OK)
            {
                SetAutoUpdateTimer();
            }
        }

        // Occurs when the form is being closed
        private void MainWindowClose(object sender, FormClosingEventArgs e)
        {
            if (App.stocksToBeDeleted)
            {
                App.UpdateStocksCSV();
            }

            if (App.currencyExchangeToBeDeleted)
            {
                App.UpdateCurrencyExchangesCSV();
            }
        }

        private void OpenAddCurrencyExchangeWindow(object sender, EventArgs e)
        {
            AddCurrencyExchangeWindow window = new AddCurrencyExchangeWindow();
            window.Show();
        }

        private void OpenProfileSummaryWindow(object sender, EventArgs e)
        {
            ProfileSummaryWindow window = new ProfileSummaryWindow(App.currentStocks, App.currentCurrencyExchanges);
            window.Show();
        }

        
    }
}
