using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text;
using TelegrammAspMvcDotNetCoreBot.Controllers;
using TelegrammAspMvcDotNetCoreBot.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
	public static class ScheduleUpdateController
	{
		//тест
		public static void Update()
		{

			UserController.CheckDoc();
			//Download("http://misis.ru/files/-/b4052b5047b2fc91dc690229330036d2/%D0%98%D0%A2%D0%90%D0%A1%D0%A3.xls", "ИТАСУ");
			//Download("http://misis.ru/files/-/06c06d68c54dbf107f8b4de7361dd909/%D0%9C%D0%93%D0%98.xlsx", "МГИ");
			//Download("http://misis.ru/files/-/d2af8541a9ded82879ef11c1e11d3d7a/%D0%98%D0%9D%D0%9C%D0%B8%D0%9D.xlsx", "ИНМиН");
			//Download("http://misis.ru/files/-/f7a613220ae7811b69b14f06f8847b80/%D0%98%D0%AD%D0%BA%D0%BE%D0%A2%D0%B5%D1%85.xls", "ЭкоТех");
			//Download("http://misis.ru/files/-/deddde8e0f846e3302deb39450d2c379/%D0%98%D0%AD%D0%A3%D0%9F%D0%9F.xls", "ЭУПП");
			//UserController.CheckDoc();
			//Download("http://misis.ru/files/-/90e4e6635500e925dadf16da8ce53c5f/%D0%98%D0%91%D0%9E.xls", "ИБО");



			//ExcelParserController.Read("0мисис.xlsx");

		}

		static string GetResponse(string uri)
		{
			StringBuilder sb = new StringBuilder();
			byte[] buf = new byte[8192];
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			Stream resStream = response.GetResponseStream();
			int count = 0;
			do
			{
				count = resStream.Read(buf, 0, buf.Length);
				if (count != 0)
				{
					sb.Append(Encoding.Default.GetString(buf, 0, count));
				}
			}
			while (count > 0);
			return sb.ToString();
		}

		static void Download(string url, string name)
		{

			WebClient wc = new WebClient();

			if (url[url.Length - 1] == 'x')
			{
				wc.DownloadFile(url, name + ".xlsx");
				ExcelParserController.ReadXlsx(name);
			}
			else
			{
				wc.DownloadFile(url, name + ".xls");
				ExcelParserController.ReadXls(name);
			}

		}
	}
}
