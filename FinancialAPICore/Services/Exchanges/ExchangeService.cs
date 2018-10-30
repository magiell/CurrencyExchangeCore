using FinancialAPICore.Models.Exchange;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace FinancialAPICore.Services.Exchanges
{
    public class ExchangeService
    {
        private const string CRAWLING_URL = "https://finance.naver.com/marketindex/exchangeList.nhn";
        private List<ExchangeInfo> cache_list = new List<ExchangeInfo>();

        public ExchangeService()
        {
            GetExchangeInfo();
        }

        public List<ExchangeInfo> RefreshList()
        {
            cache_list.Clear();
            GetExchangeInfo();
            List<ExchangeInfo> list = GetList();

            return list;
        }

        public void GetExchangeInfo()
        {           
            HtmlDocument document = HttpRequest.LoadHtmlAsync(CRAWLING_URL).ConfigureAwait(false).GetAwaiter().GetResult();            
            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//tbody//tr");
            if (nodes.Count == 0)
                return;
            foreach (var node in nodes)
            {
                var init_code = node.ChildNodes[1].InnerText.Trim();
                var filter_code = init_code.Split(" ")[0];
                // jpy string parse
                if(filter_code.Count() > 3)
                {
                    filter_code = filter_code.Split('(')[0];
                }
                cache_list.Add(new ExchangeInfo(node));
            }            
        }

        public List<ExchangeInfo> GetList()
        {
            return cache_list;
        }
    }

    public static class HttpRequest
    {
        public static async Task<HtmlDocument> LoadHtmlAsync(string url)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                HtmlWeb web = new HtmlWeb();                                
                return await web.LoadFromWebAsync(url, System.Text.Encoding.GetEncoding(51949));
            }
            catch (Exception e)
            {
                return null;
            }            
        }
    }
}
