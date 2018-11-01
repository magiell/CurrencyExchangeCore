using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancialAPICore.Models.Exchange;
using FinancialAPICore.Services.Exchanges;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FinancialAPICore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        //private ExchangeService service;
        private readonly IMemoryCache _cache;
        private ExchangeService _exchange_service;
        private readonly ILogger<ExchangeController> _logger;
        public ExchangeController(IMemoryCache memorycache, ILogger<ExchangeController> logger)
        {
            _exchange_service = new ExchangeService();
            _cache = memorycache;
            _logger = logger;
            _exchange_service.GetExchangeInfo();
            List<ExchangeInfo> list = _exchange_service.GetList();
            foreach (var item in list)
            {
                _cache.Set(item.Currency, item, TimeSpan.FromDays(1));
            }
        }


        [HttpGet]        
        public ActionResult<string> Get(string to, string from = "KRW")
        {
            _logger.LogInformation(Request.QueryString.Value);
            if(!_cache.TryGetValue(to.ToUpper(), out ExchangeInfo value))
            {
                //일괄 등록이므로 해당 캐시 전체를 리프레시
                foreach (var item in _exchange_service.RefreshList())
                {
                    _cache.Set(item.Currency, item, TimeSpan.FromDays(1));
                }     
            }
            _logger.LogInformation(value.ConvertToJsonString());
            return value.ConvertToJsonString();
        }
    }
}