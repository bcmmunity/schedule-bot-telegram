using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using TelegrammAspMvcDotNetCoreBot.Controllers;
using System.IO.Packaging;
using TelegrammAspMvcDotNetCoreBot.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
	public static class ExcelParserController
	{
		public static void ReadXls(string FileName)
		{
			//ScheduleController.CheckFile();

			ScheduleController.Unit();

			ScheduleController.AddUniversity("мисис");
			ScheduleController.AddFaculty("мисис", FileName);

			HSSFWorkbook hssfwb;
			using (FileStream file = new FileStream(@"" + FileName + ".xls", FileMode.Open, FileAccess.Read))
			{
				hssfwb = new HSSFWorkbook(file);
			}

			for (int course = 1; course < 7; course++)
			{

				if (hssfwb.GetSheet(course + " курс") == null)
					break;

				ScheduleController.AddCourse("мисис", FileName, course.ToString());

				ISheet sheet = hssfwb.GetSheet(course + " курс");

				int group = 4;
				//int myFlag = 0;

				while (sheet.GetRow(1).GetCell(group - 1) != null)
				{

					if (sheet.GetRow(1).GetCell(group - 1).NumericCellValue.ToString() == "1")
					{
						ScheduleController.AddGroup("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue + " 1 подгруппа");
					}
					else if (sheet.GetRow(1).GetCell(group - 1).NumericCellValue.ToString() == "2")
					{
						if (sheet.GetRow(0).GetCell(group - 1).ToString() == "")
							ScheduleController.AddGroup("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 3).StringCellValue + " 2 подгруппа");
						else
							ScheduleController.AddGroup("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue + " 2 подгруппа");
					}
					else
					{
						ScheduleController.AddGroup("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue);
					}

					ScheduleWeek week1 = new ScheduleWeek();
					ScheduleWeek week2 = new ScheduleWeek();

					week1.Week = 1;
					week1.Day = new List<ScheduleDay>();

					week2.Week = 2;
					week2.Day = new List<ScheduleDay>();

					for (int dayofweek = 3; dayofweek < 100; dayofweek += 14)
					{
						ScheduleDay day1 = new ScheduleDay();
						ScheduleDay day2 = new ScheduleDay();

						day1.Day = dayofweek / 14 + 1;
						day1.Lesson = new List<Lesson>();
						day2.Day = dayofweek / 14 + 1;
						day2.Lesson = new List<Lesson>();

						for (int para = dayofweek; para < dayofweek + 14; para += 2)
						{
							if (sheet.GetRow(para - 1).GetCell(group - 1) != null)
								if (sheet.GetRow(para - 1).GetCell(group - 1).StringCellValue != "")
								{
									Lesson a = new Lesson() { Name = sheet.GetRow(para - 1).GetCell(group - 1).StringCellValue, Time = sheet.GetRow(para - 1).GetCell(2).StringCellValue, Room = sheet.GetRow(para - 1).GetCell(group).StringCellValue, Teacher = "", Number = ((para - dayofweek) / 2 + 1).ToString() };
									day1.Lesson.Add(a);
								}

							if (sheet.GetRow(para).GetCell(group - 1) != null)
								if (sheet.GetRow(para).GetCell(group - 1).StringCellValue != "")
									day2.Lesson.Add(new Lesson() { Name = sheet.GetRow(para).GetCell(group - 1).StringCellValue, Time = sheet.GetRow(para - 1).GetCell(2).StringCellValue, Room = sheet.GetRow(para).GetCell(group).StringCellValue, Teacher = "", Number = ((para - dayofweek) / 2 + 1).ToString() });
						}

						week1.Day.Add(day1);
						week2.Day.Add(day2);

					}

					if (sheet.GetRow(1).GetCell(group - 1).NumericCellValue.ToString() == "1")
					{
						ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue + " 1 подгруппа", week1);
						ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue + " 1 подгруппа", week2);
					}
					else if (sheet.GetRow(1).GetCell(group - 1).NumericCellValue.ToString() == "2")
					{
						if (sheet.GetRow(0).GetCell(group - 1).ToString() == "")
						{
							ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 3).StringCellValue + " 2 подгруппа", week1);
							ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 3).StringCellValue + " 2 подгруппа", week2);
						}
						else
						{
							ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue + " 2 подгруппа", week1);
							ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue + " 2 подгруппа", week2);
						}
					}
					else
					{
						ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue, week1);
						ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue, week2);
					}

					group += 2;
				}
			}
		}

		public static void ReadXlsx(string FileName)
		{
			//ScheduleController.CheckFile();

			ScheduleController.Unit();

			ScheduleController.AddUniversity("мисис");
			ScheduleController.AddFaculty("мисис", FileName);


			XSSFWorkbook hssfwb;
			using (FileStream file = new FileStream(@"" + FileName + ".xlsx", FileMode.Open, FileAccess.Read))
			{
				hssfwb = new XSSFWorkbook(file);
			}

			for (int course = 1; course < 7; course++)
			{

				if (hssfwb.GetSheet(course + " курс") == null)
					break;
				ScheduleController.AddCourse("мисис", FileName, course.ToString());

				ISheet sheet = hssfwb.GetSheet(course + " курс");

				int group = 4;
				//int myFlag = 0;

				while (sheet.GetRow(1).GetCell(group - 1) != null)
				{

					if (sheet.GetRow(1).GetCell(group - 1).NumericCellValue.ToString() == "1")
					{
						ScheduleController.AddGroup("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue + " 1 подгруппа");
					}
					else if (sheet.GetRow(1).GetCell(group - 1).NumericCellValue.ToString() == "2")
					{
						if (sheet.GetRow(0).GetCell(group - 1).ToString() == "")
							ScheduleController.AddGroup("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 3).StringCellValue + " 2 подгруппа");
						else
							ScheduleController.AddGroup("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue + " 2 подгруппа");
					}
					else
					{
						ScheduleController.AddGroup("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue);
					}

					ScheduleWeek week1 = new ScheduleWeek();
					ScheduleWeek week2 = new ScheduleWeek();

					week1.Week = 1;
					week1.Day = new List<ScheduleDay>();

					week2.Week = 2;
					week2.Day = new List<ScheduleDay>();

					for (int dayofweek = 3; dayofweek < 100; dayofweek += 14)
					{
						ScheduleDay day1 = new ScheduleDay();
						ScheduleDay day2 = new ScheduleDay();

						day1.Day = dayofweek / 14 + 1;
						day1.Lesson = new List<Lesson>();
						day2.Day = dayofweek / 14 + 1;
						day2.Lesson = new List<Lesson>();

						for (int para = dayofweek; para < dayofweek + 14; para += 2)
						{
							if (sheet.GetRow(para - 1).GetCell(group - 1) != null)
								if (sheet.GetRow(para - 1).GetCell(group - 1).StringCellValue != "")
								{
									Lesson a = new Lesson() { Name = sheet.GetRow(para - 1).GetCell(group - 1).StringCellValue, Time = sheet.GetRow(para - 1).GetCell(2).StringCellValue, Room = sheet.GetRow(para - 1).GetCell(group).StringCellValue, Teacher = "" };
									day1.Lesson.Add(a);
								}

							if (sheet.GetRow(para).GetCell(group - 1) != null)
								if (sheet.GetRow(para).GetCell(group - 1).StringCellValue != "")
									day2.Lesson.Add(new Lesson() { Name = sheet.GetRow(para).GetCell(group - 1).StringCellValue, Time = sheet.GetRow(para - 1).GetCell(2).StringCellValue, Room = sheet.GetRow(para).GetCell(group).StringCellValue, Teacher = "" });
						}

						week1.Day.Add(day1);
						week2.Day.Add(day2);

					}

					if (sheet.GetRow(1).GetCell(group - 1).NumericCellValue.ToString() == "1")
					{
						ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue + " 1 подгруппа", week1);
						ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue + " 1 подгруппа", week2);
					}
					else if (sheet.GetRow(1).GetCell(group - 1).NumericCellValue.ToString() == "2")
					{
						if (sheet.GetRow(0).GetCell(group - 1).ToString() == "")
						{
							ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 3).StringCellValue + " 2 подгруппа", week1);
							ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 3).StringCellValue + " 2 подгруппа", week2);
						}
						else
						{
							ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue + " 2 подгруппа", week1);
							ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue + " 2 подгруппа", week2);
						}
					}
					else
					{
						ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue, week1);
						ScheduleController.AddScheduleWeek("мисис", FileName, course.ToString(), sheet.GetRow(0).GetCell(group - 1).StringCellValue, week2);
					}

					group += 2;
				}
			}
		}
	}
}
