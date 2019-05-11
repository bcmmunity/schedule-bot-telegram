using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TelegrammAspMvcDotNetCoreBot.Models.Polyteh;
using TelegrammAspMvcDotNetCoreBot.Models.VK;
using System.Windows.Forms;
//using WebClient = System.Net.WebClient;
//using WebRequest = System.Net.WebRequest;


namespace TelegrammAspMvcDotNetCoreBot.Logic.Parsers
{
    public class PolytehParser
    {
        public List<string> GetGroups()
        {

            //string cookie = "bpc=795676e101bb3acd45df4b5078c64831";
            //string url = "https://rasp.dmami.ru/groups-list.json";
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.ContentType = "application/json";
            //request.Method = "GET";
            //request.Headers["cookie"] = cookie;
            //request.Headers[HttpRequestHeader.Accept] = "application/json";
            //var resp = request.GetResponse();

            //string stream = "";

            //using (Stream data = resp.GetResponseStream())
            //{
            //    StreamReader reader = new StreamReader(data ?? throw new InvalidOperationException());
            //    stream = reader.ReadToEnd();
            //}
            //resp.Close();

            //WebClient webClient = new WebClient();


            //string url = "https://rasp.dmami.ru/groups-list.json";
            //string save_path = @"Schedule Files\Polyteh\";
            //string name = "groups-list.json";


            //webClient.DownloadFile(url, save_path + name);

            //          GroupResponse response = JsonConvert.DeserializeObject<GroupResponse>(file);

            //   return response.GroupsList;

         return null;
        }

        //public void GetSchedule()
        //{
        //    foreach (var group in GetGroups())
        //    {
        //        string cookie = "_ym_uid=1546284962160849905; _ym_d=1546284962; group=181-321; bpc=795676e101bb3acd45df4b5078c64831; _ym_isad=1";
        //        int session = 0;
        //        string url = $"https://rasp.dmami.ru/site/group?group={group}&session={session}";
        //        WebRequest request = WebRequest.Create(url);
        //        request.Method = "GET";
        //        request.Headers["cookie"] = cookie;
        //        request.Headers["referer"] = "https://rasp.dmami.ru/";

        //        var resp = request.GetResponse();

        //        var stream = "";
        //        using (Stream data = resp.GetResponseStream())
        //        {
        //            StreamReader reader = new StreamReader(data);
        //            stream = reader.ReadToEnd();
        //        }
        //        resp.Close();



        //    }
        //}
    }
}
