using System.Threading.Tasks;
using Birdie.Data.Entities;
using Birdie.Data.Repositories;
using Birdie.Service.Models;
using System;

namespace Birdie.Service.Business
{
    public interface IStockBusiness
    {
        Task<int> CreateOrUpdate(StockAddOrUpdateModel model);
    }

    public class StockBusiness : IStockBusiness
    {
        private readonly IStockRepository stockRepository;

        public StockBusiness(IStockRepository stockRepository)
        {
            this.stockRepository = stockRepository;
        }

        public async Task<int> CreateOrUpdate(StockAddOrUpdateModel model)
        {
            Stock entity = stockRepository.GetBySymbol(model.Symbol);

            if (entity == null)
            {
                entity = await stockRepository.CreateStock(new Stock
                {
                    Symbol = model.Symbol,
                    Low = model.Low,
                    Open = model.Open,
                    High = model.High,
                    Close = model.Close,
                    Date = model.Date,
                    UserId = model.UserId
                });
            }
            else
            {
                entity.Low = model.Low;
                entity.Open = model.Open;
                entity.High = model.High;
                entity.Close = model.Close;
                entity.Date = model.Date;
                entity.UserId = model.UserId;
                entity.UpdatedAt = DateTimeOffset.Now;
                entity = await stockRepository.UpdateStock(entity);
            }

            return entity.Id;
        }
    }
}
