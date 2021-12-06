using System.Linq;
using Birdie.Service;
using Birdie.Service.Business;
using Birdie.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Birdie.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<RoomController> _logger;
        private readonly IRoomBusiness roomBusiness;

        public RoomController(ILogger<RoomController> logger, IRoomBusiness roomBusiness)
        {
            this._logger = logger;
            this.roomBusiness = roomBusiness;
        }

        public IRabbitMQService RabbitMQService { get; }

        [HttpGet]
        public IQueryable<RoomItemModel> Get()
        {
            return roomBusiness.Get();
        }
    }
}
