using Birdie.Service.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Birdie.Service
{
    public interface IStockHttpService
    {
        Task<StockAddOrUpdateModel> RetrieveStock(string symbol);
    }

    public class StockHttpService : IStockHttpService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;

        public StockHttpService(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
        }

        public async Task<StockAddOrUpdateModel> RetrieveStock(string symbol)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format(configuration["StockUrl"], symbol));
            HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            string[] resultArray = result.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            string[] values = resultArray[1].Split(',');

            if (values.Any(x => x == "N/D"))
            {
                throw new Exception("Given search yielded no results");
            }

            StockAddOrUpdateModel stock = new StockAddOrUpdateModel
            {
                Symbol = values[0],
                Date = DateTimeOffset.Parse($"{values[1]} {values[2]}"),
                Open = decimal.Parse(values[3]),
                High = decimal.Parse(values[4]),
                Low = decimal.Parse(values[5]),
                Close = decimal.Parse(values[6])
            };

            return stock;
        }
    }
}
