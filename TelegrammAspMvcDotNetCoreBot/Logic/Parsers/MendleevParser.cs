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

            Schedule.AddUniversity("РХТУ им.Менделеева");
            

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

                        Schedule.AddFacility("РХТУ им.Менделеева", 
                        GetFacility(sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue.Split('-')[0]));

                    Schedule.AddCourse("РХТУ им.Менделеева",
                        GetFacility(sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue.Split('-')[0]), 
                        fileName[0].ToString());
                        Schedule.AddGroup("РХТУ им.Менделеева",
                            GetFacility(sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue.Split('-')[0]),
                                    fileName[0].ToString(),
                                    sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue);

                    ScheduleWeek week1 = new ScheduleWeek();
                    ScheduleWeek week2 = new ScheduleWeek();

                    week1.Week = 2;
                    week1.Day = new List<ScheduleDay>();
                        //номера недель отличаются от мисис
                    week2.Week = 1;
                    week2.Day = new List<ScheduleDay>();

                    for (int weekDay = 1; weekDay <= 6; weekDay++) //перебор всех дней недели
                    {
                        ScheduleDay day1 = new ScheduleDay();
                        ScheduleDay day2 = new ScheduleDay();

                        day1.Day = weekDay;
                        day1.Lesson = new List<Lesson>();
                        day2.Day = weekDay;
                        day2.Lesson = new List<Lesson>();

                        bool isFind = true; //определяет, найдены ли все куски склеенной ячейки

                        string startTime = String.Empty;
                        string endTime = String.Empty;

                        int rowBufer = 0;

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
                                    else if (sheet.GetRow(row).GetCell(cell + 1).StringCellValue
                                             != "")
                                    {
                                        
                                            AddDay(sheet, row, cell, GetTime(startTime, endTime), GetLessonNumber(GetTime(startTime, startTime)), day1, day2,true);
                                    }
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
                                    if ( !sheet.GetRow(row + 1).GetCell(cell + 1).IsMergedCell
                                        || (sheet.GetRow(row + 1).GetCell(cell + 1).IsMergedCell
                                            && sheet.GetRow(row + 1).GetCell(cell + 1).StringCellValue != "")
                                        || lessonIndex == 9) //если снизу находится часть объединения ячеек или блок кончился, то можно запоминать последнее время и сохранять
                                    {
                                        isFind = true;
                                       
                                            AddDay(sheet, rowBufer, cell, GetTime(startTime, endTime), GetLessonNumber(GetTime(startTime, startTime)), day1, day2);
                                       
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
                    }

                    row = 3;

                    Schedule.AddScheduleWeek("РХТУ им.Менделеева", GetFacility(sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue.Split('-')[0]), fileName[0].ToString(), sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue, week1);
                    Schedule.AddScheduleWeek("РХТУ им.Менделеева", GetFacility(sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue.Split('-')[0]), fileName[0].ToString(), sheet.GetRow(row - 1).GetCell(cell + 1).StringCellValue, week2);

                    cell += 4;
                 }
            }  
        }

        private string GetTime(string startTime, string endTime) //Собирает время для БД
        {
            return startTime.Split('-')[0]+":00 - "+endTime.Split('-')[1]+":00";
        }

        private int GetLessonNumber(string lessonTime)
        {
            DateTime time = DateTime.Parse(lessonTime.Split('-')[0]);

            if (time >= DateTime.Parse("9:00:00") && time <= DateTime.Parse("10:35:00"))
                return 1;
            if (time >= DateTime.Parse("10:45:00") && time <= DateTime.Parse("12:20:00"))
                return 2;
            if (time >= DateTime.Parse("13:00:00") && time <= DateTime.Parse("14:35:00"))
                return 3;
            if (time >= DateTime.Parse("14:45:00") && time <= DateTime.Parse("16:20:00"))
                return 4;
            if (time >= DateTime.Parse("16:30:00") && time <= DateTime.Parse("18:05:00"))
                return 5;

            return 0;
        }

        private void AddDay(ISheet sheet, int row, int cell, string time, int lessonNumber, ScheduleDay day1, ScheduleDay day2, bool onlyOne = false)
        {
            if (onlyOne)
            {
                string roomx = String.Empty;
                try //определение, цифра ли кабинет или слово
                {
                    roomx = sheet.GetRow(row).GetCell(cell).StringCellValue;
                }
                catch
                {
                    roomx = sheet.GetRow(row).GetCell(cell).NumericCellValue.ToString();
                }

                string namex = sheet.GetRow(row).GetCell(cell + 1).StringCellValue;
                string lessonTypex = sheet.GetRow(row).GetCell(cell + 2).StringCellValue;
                string fullNamex = String.Empty;

                IColor colorx = sheet.GetRow(row).GetCell(cell).CellStyle.FillBackgroundColorColor;
                if (colorx != null && !namex.Contains("Физическая культура"))
                {
                    fullNamex = namex + " " + lessonTypex + " (пара в Тушино)";
                   
                }
                else
                {
                    fullNamex = namex + " " + lessonTypex;
                }

                
     
                Lesson ax = new Lesson()
                {
                    Name = fullNamex,
                    Number = lessonNumber.ToString(),
                    Time = time,
                    Room = roomx,
                    Teacher = ""
                };

                if (ax.Name == "")
                    return;

                
                    day1.Lesson.Add(ax);
                    day2.Lesson.Add(ax);
                    

                    return;
            }
             
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

                string lessonType1 = String.Empty;
                string lessonType2 = String.Empty;

                string room1 = String.Empty;
                string room2 = String.Empty;
                if (room.Contains('/'))
                {
                    room1 = StringSeparator(room, '/')[0];
                    room2 = StringSeparator(room, '/')[1];
            }
                else
                {
                room1 = StringSeparator(room, ' ')[0];
                room2 = StringSeparator(room, ' ')[1];
            }
               

            if (name2 != "")
            {
                if (lessonType.Contains('/'))
                {
                    lessonType1 = StringSeparator(lessonType, '/')[0];
                    lessonType2 = StringSeparator(lessonType, '/')[1];
                }
                else
                {
                    lessonType1 = StringSeparator(lessonType, ' ')[0];
                    lessonType2 = StringSeparator(lessonType, ' ')[1];
                }

                if (lessonType2 == "")
                {
                    lessonType2 = sheet.GetRow(row+1).GetCell(cell + 2).StringCellValue;
                }

                if (room2 == "")
                {
                    try //определение, цифра ли кабинет или слово
                    {
                        room2 = sheet.GetRow(row+1).GetCell(cell).StringCellValue;
                    }
                    catch
                    {
                        room2 = sheet.GetRow(row+1).GetCell(cell).NumericCellValue.ToString();
                    }
                }
            }
            else
            {
                if (room2 == "")
                {
                    try //определение, цифра ли кабинет или слово
                    {
                        room2 = sheet.GetRow(row+1).GetCell(cell).StringCellValue;
                    }
                    catch
                    {
                        room2 = sheet.GetRow(row+1).GetCell(cell).NumericCellValue.ToString();
                    }
                    if (room2 == "0" || room2 == "")
                    {
                        room1 = room;
                        room2 = room;
                    }                 
                }

                lessonType1 = lessonType;
                lessonType2 = lessonType;
       
            }

            string fullName1 = String.Empty;
            string fullName2 = String.Empty;

            IColor color = sheet.GetRow(row).GetCell(cell).CellStyle.FillForegroundColorColor;
            if (color?.RGB != null && color.RGB[0] == 204 && color.RGB[1] == 255 && color.RGB[2] == 204)
            {
                fullName1 = name1 + " " + lessonType1+" (пара в Тушино)";
                fullName2 = name2 + " " + lessonType2+ " (пара в Тушино)";
            }
            else
            {
                fullName1 = name1 + " " + lessonType1;
                fullName2 = name2 + " " + lessonType2;
            }
                

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

            if (name1.Contains("(1"))
                day1.Lesson.Add(a);
            else if (name1.Contains("(2"))
                day2.Lesson.Add(a);
            else
            {
                day1.Lesson.Add(a);
                if (room2 != "")
                {
                    Lesson x = new Lesson()
                    {
                        Name = fullName1,
                        Number = lessonNumber.ToString(),
                        Time = time,
                        Room = room2,
                        Teacher = ""
                    };
                    day2.Lesson.Add(x);
                }
                else
                {
                    day2.Lesson.Add(a);
                }

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

                if (name2.Contains("(1"))
                {
                    if (room1 == "")
                    {
                        Lesson x = new Lesson()
                        {
                            Name = fullName2,
                            Number = lessonNumber.ToString(),
                            Time = time,
                            Room = room2,
                            Teacher = ""
                        };
                        day1.Lesson.Add(x);
                    }
                    else
                        day1.Lesson.Add(b);
                }
                   
                else if (name2.Contains("(2"))
                {
                    if (room2 == "")
                    {
                        Lesson x = new Lesson()
                        {
                            Name = fullName2,
                            Number = lessonNumber.ToString(),
                            Time = time,
                            Room = room1,
                            Teacher = ""
                        };
                        day2.Lesson.Add(x);
                    }
                    else
                        day2.Lesson.Add(b);
                }
                    
            }


             

        }


        private string[] StringSeparator(string a, char separator)
        {

            int isFirst = 0;

            string a1 = String.Empty;
            string a2 = String.Empty;
            foreach (char letter in a)
            {
                if (isFirst == 0)
                {
                    a1 += letter;
                    if (letter == separator)
                        isFirst = 1;
                }
                else if (isFirst == 1)
                {
                    if (letter != ' ')
                    {
                        a2 += letter;
                        isFirst = 2;
                    }
                }
                else
                {
                    a2 += letter;
                }
            }

            string[] result = {a1.Trim(), a2.Trim()};
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
                    return "ГФ";
                case "ЮР":
                    return "ГФ";
                case "Ю":
                    return "ГФ";
                case "МН":
                    return "ТНВиВМ";
                case "МК":
                    return "ИТУ";
                case "МЭ":
                    return "БПЭ";
                case "МТ":
                    return "ФИХ";
                case "МП":
                    return "НПМ";
                case "МО":
                    return "ХФТ";
                case "МЕН":
                    return "ФЕН";

            }

            return "Другое";

        }
    }
}
