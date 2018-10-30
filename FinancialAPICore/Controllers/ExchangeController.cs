using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancialAPICore.Models.Exchange;
using FinancialAPICore.Services.Exchanges;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        public ExchangeController(IMemoryCache memorycache)
        {
            _exchange_service = new ExchangeService();
            _cache = memorycache;

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
            if(!_cache.TryGetValue(to, out ExchangeInfo value))
            {
                //일괄 등록이므로 해당 캐시 전체를 리프레시
                foreach (var item in _exchange_service.RefreshList())
                {
                    _cache.Set(item.Currency, item, TimeSpan.FromDays(1));
                }     
            }
            return JsonConvert.SerializeObject(value);
        }
    }
}