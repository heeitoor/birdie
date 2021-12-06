using Birdie.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Birdie.Data.Repositories
{
    public interface IStockRepository
    {
        Stock GetBySymbol(string symbol);

        Task<Stock> CreateStock(Stock stock);

        Task<Stock> UpdateStock(Stock stock);
    }

    internal class StockRepository : RepositoryBase<Stock>, IStockRepository
    {
        public StockRepository(BirdieDbContext birdieDbContext) : base(birdieDbContext) { }

        public Task<Stock> CreateStock(Stock stock)
        {
            return base.Create(stock);
        }

        public Stock GetBySymbol(string symbol)
        {
            return base.Get(x => x.Symbol == symbol).FirstOrDefault();
        }

        public Task<Stock> UpdateStock(Stock stock)
        {
            return base.Update(stock);
        }
    }
}
