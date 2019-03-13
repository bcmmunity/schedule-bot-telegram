//using System.IO;
//using System.Net;
//using System.Text;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
	public static class ScheduleDBUpdate
	{
		public static void Update()
		{

            ExcelParserToDB.ReadXls("ИТАСУ");
            ExcelParserToDB.ReadXls("ИНМИН");
            ExcelParserToDB.ReadXlsx("МГИ");
            ExcelParserToDB.ReadXls("ЭУПП");
            ExcelParserToDB.ReadXls("ЭкоТех");

        }

		//static string GetResponse(string uri)
		//{
		//	StringBuilder sb = new StringBuilder();
		//	byte[] buf = new byte[8192];
		//	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
		//	HttpWebResponse response = (HttpWebResponse)request.GetResponse();
		//	Stream resStream = response.GetResponseStream();
		//	int count = 0;
		//	do
		//	{
		//		count = resStream.Read(buf, 0, buf.Length);
		//		if (count != 0)
		//		{
		//			sb.Append(Encoding.Default.GetString(buf, 0, count));
		//		}
		//	}
		//	while (count > 0);
		//	return sb.ToString();
		//}

		//static void Download(string url, string name)
		//{

		//	WebClient wc = new WebClient();

		//	if (url[url.Length - 1] == 'x')
		//	{
		//		wc.DownloadFile(url, name + ".xlsx");
		//		ExcelParserController.ReadXlsx(name);
		//	}
		//	else
		//	{
		//		wc.DownloadFile(url, name + ".xls");
		//		ExcelParserController.ReadXls(name);
		//	}

		//}
	}
}
