using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.Logic.Parsers
{
    public class MendleevParser
    {
        private readonly ScheduleDB Schedule = new ScheduleDB();

        public void ReadXlsx(string fileName)
        {

            Schedule.AddUniversity("им.Менделеева");
            

            XSSFWorkbook scheduleWorkbook;
            using (FileStream file = new FileStream(@"Schedule Files\Mendeleev\" + fileName + ".xlsx", FileMode.Open, FileAccess.Read))
            {
                scheduleWorkbook = new XSSFWorkbook(file);
            }

            int sheetCount = scheduleWorkbook.NumberOfSheets;

            for (int sheetNumber = 0; sheetNumber < sheetCount; sheetNumber++)
            {

                ISheet sheet = scheduleWorkbook.GetSheetAt(sheetNumber);

                try
                {
                    if (sheet.GetRow(0).GetCell(1).StringCellValue == "")
                        break;
                }
                catch
                {
                        break;
                }
                

                 int row = 3;
                 int cell = 10;


                while (sheet.GetRow(row-1).GetCell(cell + 1) != null) // перебор всех групп
                {
                    if (sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue == "")
                        break;

                        Schedule.AddFacility("им.Менделеева", 
                        GetFacility(sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue.Split('-')[0]));

                    Schedule.AddCourse("им.Менделеева",
                        GetFacility(sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue.Split('-')[0]), 
                        fileName[0].ToString());
                        Schedule.AddGroup("им.Менделеева",
                            GetFacility(sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue.Split('-')[0]),
                                    fileName[0].ToString(),
                                    sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue);

                    ScheduleWeek week1 = new ScheduleWeek();
                    ScheduleWeek week2 = new ScheduleWeek();

                    week1.Week = 1;
                    week1.Day = new List<ScheduleDay>();

                    week2.Week = 2;
                    week2.Day = new List<ScheduleDay>();

                    for (int weekDay = 1; weekDay <= 6; weekDay++) //перебор всех дней недели
                    {
                        ScheduleDay day1 = new ScheduleDay();
                        ScheduleDay day2 = new ScheduleDay();
                        bool isFind = true; //определяет, найдены ли все куски склеенной ячейки

                        string startTime = String.Empty;
                        string endTime = String.Empty;

                        int rowBufer = 0;

                        int lessonNumber = 1;

                        for (int lessonIndex = 0; lessonIndex < 10; lessonIndex++) //перебор всех пар в дне
                        {
                            if (sheet.GetRow(row).GetCell(cell) == null)
                                continue;

                            if (weekDay == 6 && lessonIndex > 5) //в суббту 
                                continue;

                            if (isFind) //если вся ячейка собрана
                            {
                                rowBufer = row;
                                startTime = sheet.GetRow(row).GetCell(1).StringCellValue; 
                                endTime = sheet.GetRow(row).GetCell(1).StringCellValue;

                                try
                                {
                                    if (sheet.GetRow(row + 1).GetCell(cell + 1).IsMergedCell
                                        && sheet.GetRow(row + 1).GetCell(cell + 1).StringCellValue
                                        == "") //если снизу находится нижняя часть ячейки, то она объединенная
                                        isFind = false;
                                }
                                catch
                                {
                            continue;
                                }
                                
                            }
                            else //продолжается поиск всех частей сборной ячейки
                            {
                                endTime = sheet.GetRow(row).GetCell(1).StringCellValue;

                                try
                                {
                                    if ((sheet.GetRow(row + 1).GetCell(cell + 1).StringCellValue == "")
                                        && !sheet.GetRow(row + 1).GetCell(cell + 1).IsMergedCell
                                        || (sheet.GetRow(row + 1).GetCell(cell + 1).IsMergedCell
                                            && sheet.GetRow(row + 1).GetCell(cell + 1).StringCellValue != "")
                                        || lessonIndex == 9) //если снизу находится часть объединения ячеек или блок кончился, то можно запоминать последнее время и сохранять
                                    {
                                        isFind = true;
                                        AddDay(sheet, rowBufer, cell, GetTime(startTime, endTime), lessonNumber, day1, day2);
                                        lessonNumber++;
                                    }
                                }
                                catch
                                {
                                    continue;
                                }
                                

                            }

                            row ++;
                        }


                        week1.Day.Add(day1);
                        week2.Day.Add(day2);
                        lessonNumber = 1;
                    }

                    row = 3;

                    Schedule.AddScheduleWeek("им.Менделеева", GetFacility(sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue.Split('-')[0]), fileName[0].ToString(), sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue, week1);
                    Schedule.AddScheduleWeek("им.Менделеева", GetFacility(sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue.Split('-')[0]), fileName[0].ToString(), sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue, week2);

                    cell += 4;
                 }
            }  
        }

        private string GetTime(string startTime, string endTime) //Собирает время для БД
        {
            return startTime.Split('-')[0]+":00 - "+endTime.Split('-')[1]+":00";
        }

        private void AddDay(ISheet sheet, int row, int cell, string time, int lessonNumber, ScheduleDay day1, ScheduleDay day2)
        {
            string room = String.Empty;
            try //определение, цифра ли кабинет или слово
            {
                room = sheet.GetRow(row).GetCell(cell).StringCellValue;
            }
            catch
            {
                room = sheet.GetRow(row).GetCell(cell).NumericCellValue.ToString();
            }

            string name = sheet.GetRow(row).GetCell(cell + 1).StringCellValue;
            string lessonType = sheet.GetRow(row).GetCell(cell + 2).StringCellValue;

                string name1 = StringSeparator(name,')')[0];
                string name2 = StringSeparator(name,')')[1];
            
                string lessonType1 = StringSeparator(lessonType,' ')[0];
                string lessonType2 = StringSeparator(lessonType,' ')[1];

                string room1 = StringSeparator(room, ' ')[0];
                string room2 = StringSeparator(room, ' ')[1];

            string fullName1 = name1 + " " + lessonType1;
                string fullName2 = name2 + " " + lessonType2;

            Lesson a = new Lesson()
            {
                Name = fullName1,
                Number = lessonNumber.ToString(),
                Time = time,
                Room = room1,
                Teacher = ""
            };

            if (a.Name == "")
                return;

            if (a.Name.Contains("1 "))
                day1.Lesson.Add(a);
            else if (a.Name.Contains("2 "))
                day2.Lesson.Add(a);
            else
            {
                day1.Lesson.Add(a);
                day2.Lesson.Add(a);
                return;
            }

            Lesson b = null;
            if (name2 != String.Empty)
            {
                b = new Lesson()
                {
                    Name = fullName2,
                    Number = lessonNumber.ToString(),
                    Time = time,
                    Room = room2,
                    Teacher = ""
                };

                if (b.Name == "")
                    return;

                if (b.Name.Contains("1 "))
                    day1.Lesson.Add(b);
                else if (b.Name.Contains("2 "))
                    day2.Lesson.Add(b);
            }


             

        }


        private string[] StringSeparator(string a, char separator)
        {

            bool isFirst = true;

            string a1 = String.Empty;
            string a2 = String.Empty;
            foreach (char letter in a)
            {
                if (isFirst)
                {
                    a1 += letter;
                    if (letter == '/')
                    {
                        a1 = a;
                        break;
                    }
                    if (letter == separator)
                        isFirst = false;
                }
                else
                {
                    if (letter == ' ')
                        continue;
                    a2 += letter;
                }
            }

            string[] result = {a1, a2};
            return result;
        }

        private string GetFacility(string abbreviation)
        {
            switch (abbreviation)
            {
                case "П":
                    return "НПМ";
                case "Н":
                    return "ТНВиВМ";
                case "О":
                    return "ХФТ";
                case "ТМ":
                    return "ФИХ";
                case "Тм":
                    return "ФИХ";
                case "И":
                    return "ИХТ";
                case "К":
                    return "ИТУ";
                case "КС":
                    return "ИТУ";
                case "Кс":
                    return "ИТУ";
                case "Э":
                    return "БПЭ";
                case "ЭК":
                    return "ГФ";
                case "Эк":
                    return "ГФ";
                case "ПР":
                    return "ИПУР";
                case "Пр":
                    return "ИПУР";
                case "А":
                    return "ИПУР";
                case "ЕН":
                    return "ФЕН";
                case "Ен":
                    return "ФЕН";
                case "Ф":
                    return "ИСМЭН-ИФХ";
                case "Юр":
                    return "Гуманитарный";
            }

            return "Другое";

        }
    }
}
