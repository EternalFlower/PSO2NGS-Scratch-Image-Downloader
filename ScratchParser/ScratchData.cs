using System;
using HtmlAgilityPack;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScratchDataParser
{
    public class ScratchData
    {

        public ScratchData()
        {

        }

        private void parseHTMLDoc(HtmlDocument htmlDoc)
        {
            var prizes = htmlDoc.DocumentNode.SelectNodes("//dl[@class='item-list-l']");

            foreach (var prize in prizes)
            {
                var prize_name = prize.SelectSingleNode("dt").InnerText;
                var concept_url = prize.SelectSingleNode(".//a[@title='設定画']")?.GetAttributeValue("href", "");
                concept_url = String.IsNullOrEmpty(concept_url) ? concept_url : "";

                var prize_details = prize.SelectNodes(".//td");

                var prize_list = prize.SelectSingleNode(".//ul[@class='image']");

                if (prize_list != null)
                {
                    var prize_contents = prize.SelectNodes(".//li");
                }
            }
        }
        
        public void parseURL(string url)
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            parseHTMLDoc(htmlDoc);
        }

        public void parseHTML(string filename)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(filename);
            parseHTMLDoc(htmlDoc);   
        }
    }
}
