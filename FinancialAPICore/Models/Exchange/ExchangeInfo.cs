using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialAPICore.Models.Exchange
{
    public class ExchangeInfo
    {
        public string Currency { get; set; }
        public string Name { get; set; }
        public string Bases { get; set; }
        public string Buy { get; set; }
        public string Sales { get; set; }
        public string SandCash { get; set; }
        public string ReceiveCash { get; set; }
        
        public ExchangeInfo(HtmlNode node)
        {   
            Name = node.ChildNodes[1].InnerText.Trim();

            var filter_code = Name.Split(" ")[1];
            //// jpy string parse
            //if (filter_code.Count() > 3)
            //{
            //    filter_code = filter_code.Split('(')[0];
            //}

            Currency = filter_code;
            Bases = node.ChildNodes[3]?.InnerText;
            Buy = node.ChildNodes[5]?.InnerText;
            Sales = node.ChildNodes[7]?.InnerText;
            SandCash = node.ChildNodes[9]?.InnerText;
            ReceiveCash = node.ChildNodes[11]?.InnerText;            
        }

        public string ConvertToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
