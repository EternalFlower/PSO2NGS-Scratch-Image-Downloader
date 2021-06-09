using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PSO2_Scratch_Parser
{
    public enum ImageNameOption
    {
        Original,
        Japanese,
        English
    }

    public class ScratchParser
    {
        private readonly List<Prize> m_prizeList;
        private string Prize_Url = "";

        public ScratchParser()
        {
            m_prizeList = new List<Prize>();
        }

        public void parseFromWebsiteURL(string url)
        {
            Clear();

            Prize_Url = url;
            HtmlWeb web = new HtmlWeb() { OverrideEncoding = Encoding.UTF8 };
            var htmlDoc = web.Load(url);

            Trace.WriteLine($"Parsing data from {url}.");

            parseHTMLDoc(htmlDoc);
        }

        public void parseFromHTMLFile(string filename)
        {
            Clear();

            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(filename);

            Trace.WriteLine($"Parsing data from {filename}.");

            parseHTMLDoc(htmlDoc);
        }

        public int Count
        {
            get
            {
                return m_prizeList.Count;
            }
        }

        private string GetImageUrl(string rel_url)
        {
            if (String.IsNullOrEmpty(rel_url))
            {
                return "";
            }
            else if (String.IsNullOrEmpty(Prize_Url))
            {
                return rel_url;
            }

            var uri = new Uri(Prize_Url);
            uri = new Uri(uri, rel_url);
            return uri.AbsoluteUri;
        }

        private void parseHTMLDoc(HtmlDocument htmlDoc)
        {
            var prizes = htmlDoc.DocumentNode.SelectNodes("//dl[@class='item-list-l']");

            foreach (var prize in prizes)
            {
                var prize_name = prize.SelectSingleNode("dt").InnerText;
                var concept_url = prize.SelectSingleNode(".//a[@title='設定画']")?.GetAttributeValue("href", "");
                concept_url = GetImageUrl(concept_url);

                var prize_details = prize.SelectNodes(".//td");

                if (prize_details.Count < 2)
                    continue;

                var prize_list = prize.SelectSingleNode(".//ul[@class='image']");

                if (prize_list != null)
                {
                    var parse_list = new List<PrizeBoxItem>();
                    var prize_contents = prize_list.SelectNodes(".//li");

                    foreach (var item in prize_contents)
                    {
                        Match name_match = Regex.Match(item.InnerText, "「(.*?)」");
                        var item_name = name_match.Success ? name_match.Groups?[1].Value : "";

                        Match genre_match = Regex.Match(item.InnerText, "（(.*?)）");
                        var item_genre = genre_match.Success ? genre_match.Groups?[1].Value : "";

                        var item_image = GetImageUrl(prize.SelectSingleNode($".//a[@title='{item_name}']")?.GetAttributeValue("href", ""));


                        parse_list.Add(new PrizeBoxItem
                        {
                            Name_jp = item_name,
                            Name_en = "",
                            Image_url = item_image,
                            Genre_jp = item_genre,
                            Genre_en = ""
                        });
                    }

                    m_prizeList.Add(new Prize
                    {
                        Name_jp = prize_name,
                        Name_en = "",
                        Concept_art = concept_url,
                        Genre_jp = prize_details[0]?.InnerText,
                        Genre_en = "",
                        Rate = prize_details[1]?.InnerText,
                        Contents = parse_list
                    });
                }
                else
                {
                    var item_image = GetImageUrl(prize.SelectSingleNode($".//a[@title='{prize_name}']")?.GetAttributeValue("href", ""));

                    m_prizeList.Add(new Prize
                    {
                        Name_jp = prize_name,
                        Name_en = "",
                        Concept_art = concept_url,
                        Image_url = item_image,
                        Genre_jp = prize_details[0]?.InnerText,
                        Genre_en = "",
                        Rate = prize_details[1]?.InnerText
                    });
                }
            }

            Trace.WriteLine("Finish parsing.");
        }

        public void Write(string fileName)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var jsonString = JsonConvert.SerializeObject(m_prizeList, settings);
            File.WriteAllText(fileName, jsonString);
        }

        private async Task DownloadImage(Uri url, string filename)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFileCompleted += ((sender, args) =>
                {
                    if (args.Cancelled)
                    {
                        Trace.WriteLine("File download cancelled.");
                    }

                    Trace.WriteLine($"{filename} has been downloaded.");
                });
                try
                {
                    await client.DownloadFileTaskAsync(url, filename);
                }
                catch (Exception)
                {
                    Trace.WriteLine($"Failed to download File: {filename} from: {url.AbsoluteUri}");
                }

            }
        }

        public async void SaveImages(string directory, ImageNameOption option)
        {
            Dictionary<string, string> files = new Dictionary<string, string>();
            var downloadTasks = new List<Task>();

            foreach (var prize in m_prizeList)
            {
                if (!String.IsNullOrEmpty(prize.Concept_art))
                {
                    string imageName = option == ImageNameOption.Original ? prize.Concept_art.Substring(prize.Concept_art.LastIndexOf("/") + 1) : $"{MakeValidFileName(prize.Name_jp)}_concept.jpg";
                    files.TryAdd(prize.Concept_art, Path.Combine(directory, imageName));
                }

                if (!String.IsNullOrEmpty(prize.Image_url))
                {
                    string imageName = option == ImageNameOption.Original ? prize.Image_url.Substring(prize.Image_url.LastIndexOf("/") + 1) : $"{MakeValidFileName(prize.Name_jp)}.jpg";
                    files.TryAdd(prize.Image_url, Path.Combine(directory, imageName));
                }

                if (prize.Contents != null)
                {
                    foreach (var item in prize.Contents)
                    {
                        if (!String.IsNullOrEmpty(item.Image_url))
                        {
                            string imageName = option == ImageNameOption.Original ? item.Image_url.Substring(item.Image_url.LastIndexOf("/") + 1) : $"{MakeValidFileName(item.Name_jp)}.jpg";
                            files.TryAdd(item.Image_url, Path.Combine(directory, imageName));
                        }
                    }
                }
            }

            foreach (var file in files)
            {
                downloadTasks.Add(DownloadImage(new Uri(file.Key), file.Value));
            }

            await Task.WhenAll(downloadTasks.ToArray());

            Trace.WriteLine("Finish downloading.");
        }

        public void Clear()
        {
            Prize_Url = "";
            m_prizeList.Clear();
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
