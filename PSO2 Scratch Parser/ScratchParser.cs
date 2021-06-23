using System;
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
    public class ScratchParser
    {
        private readonly List<Prize> m_ItemList;
        private readonly List<BonusPrize> m_BonusList;
        private string Prize_Url = "";
        private const string itemlistJson_relURL = "js/itemlist.json";
        private const string bonuslistJson_relURL = "js/bonuslist.json";
        private bool m_hasData = false;
        private Dictionary<string, bool> m_Options;

        public ScratchParser()
        {
            m_ItemList = new List<Prize>();
            m_BonusList = new List<BonusPrize>();
            m_Options = new Dictionary<string, bool>();
        }

        public void ParseScratch(string url)
        {
            Clear();
            Prize_Url = CleanScratchURL(url);
            try
            {
                parseFromItemlistJSON();
                parseFromBonuslistJSON();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);

                var innerException = e.InnerException;
                while (innerException != null)
                {
                    Trace.WriteLine(innerException.Message);
                    innerException = innerException.InnerException;
                }
            }
            
            m_hasData = true;
        }

        public void parseFromItemlistJSON()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(MergeURI(Prize_Url, itemlistJson_relURL));
                m_ItemList.AddRange(JsonConvert.DeserializeObject<List<Prize>>(json));
            }

        }

        public void parseFromBonuslistJSON()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(MergeURI(Prize_Url, bonuslistJson_relURL));
                m_BonusList.AddRange(JsonConvert.DeserializeObject<List<BonusPrize>>(json));
            }
        }

        public bool HasData
        {
            get
            {
                return m_hasData;
            }
        }

        private string MergeURI(string base_url, string rel_url)
        {
            var uri = new Uri(base_url);
            uri = new Uri(uri, rel_url);
            return uri.AbsoluteUri;
        }

        private string CleanScratchURL(string url)
        {
            if (url.EndsWith(".html"))
            {
                var i = url.LastIndexOf('/');
                return url.Substring(0, i + 1);
            } 
            else if (url.EndsWith("/"))
            {
                return url;
            }

            return url + '/';
        }

        public void WriteItemList(string fileName)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var jsonString = JsonConvert.SerializeObject(m_ItemList, settings);
            File.WriteAllText(fileName, jsonString);
        }

        public void WriteBonusList(string fileName)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var jsonString = JsonConvert.SerializeObject(m_BonusList, settings);
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
                catch (Exception e)
                {
                    Exception inner = e.InnerException;
                    
                    Trace.WriteLine($"Failed to download File: {filename} from: {url.AbsoluteUri}");
                    Trace.WriteLine($"Exception {inner.Message}.");
                }

            }
        }

        public async void SaveImages(string directory)
        {
            Dictionary<string, string> files = new Dictionary<string, string>();
            var downloadTasks = new List<Task>();
            Regex illus_regex = new Regex("il");
            Regex icon_regex = new Regex("icon");

            foreach (var prize in m_ItemList)
            {
                var illusList = prize.illust.Split(',');

                if (prize.voiceid.Length > 1)
                {
                    continue;
                } 
                else if (prize.ss.Length > 1)
                {
                    var pngList = prize.ss.Split(',');
                    for (var i = 0; i < pngList.Length; i++)
                    {
                        files.TryAdd(MergeURI(Prize_Url, "../../img/item/ss/" + pngList[i] + ".jpg"), Path.Combine(directory, pngList[i] + ".jpg"));
                    }
                } 
                else if (illusList.Length > 1)
                {
                    for (var i = 0; i < illusList.Length; i++)
                    {
                        
                        if (illus_regex.IsMatch(illusList[i]))
                        {
                            files.TryAdd(MergeURI(Prize_Url, "../../img/item/illust/" + illusList[i].Replace("il","") + ".png"), Path.Combine(directory, illusList[i].Replace("il", "") + ".png"));
                        }
                        else
                        {
                            files.TryAdd(MergeURI(Prize_Url, "img/ss/" + illusList[i] + ".png"), Path.Combine(directory, illusList[i] + ".png"));
                        }
                    }
                }
                else
                {
                    files.TryAdd(MergeURI(Prize_Url, "img/ss/" + illusList[0] + ".png"), Path.Combine(directory, illusList[0] + ".png"));
                }
            }

            foreach (var prize in m_BonusList)
            {
                var illusList = prize.illust.Split(',');

                if (prize.voiceid.Length > 1)
                {
                    continue;
                }
                else if (prize.ss.Length > 1)
                {
                    var pngList = prize.ss.Split(',');
                    for (var i = 0; i < pngList.Length; i++)
                    {
                        files.TryAdd(MergeURI(Prize_Url, "../../img/item/ss/" + pngList[i] + ".jpg"), Path.Combine(directory, pngList[i] + ".jpg"));
                    }
                }
                else if (illusList.Length > 1)
                {
                    for (var i = 0; i < illusList.Length; i++)
                    {

                        if (illus_regex.IsMatch(illusList[i]))
                        {
                            files.TryAdd(MergeURI(Prize_Url, "../../img/item/illust/" + illusList[i].Replace("il", "") + ".png"), Path.Combine(directory, illusList[i].Replace("il", "") + ".png"));
                        }
                        else
                        {
                            files.TryAdd(MergeURI(Prize_Url, "img/ss/" + illusList[i] + ".png"), Path.Combine(directory, illusList[i] + ".png"));
                        }
                    }
                }
                else if (icon_regex.IsMatch(illusList[0]))
                {
                    if (!m_Options.GetValueOrDefault("icon", false))
                        continue;

                    files.TryAdd(MergeURI(Prize_Url, "../../img/item/icon/" + illusList[0] + ".png"), Path.Combine(directory, illusList[0] + ".png"));
                }
                else
                {
                    files.TryAdd(MergeURI(Prize_Url, "img/ss/" + illusList[0] + ".png"), Path.Combine(directory, illusList[0] + ".png"));
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
            m_ItemList.Clear();
            m_BonusList.Clear();
            m_hasData = false;
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
