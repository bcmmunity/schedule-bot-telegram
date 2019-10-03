using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public class ModeSelection
    {
        private static List<UserTeacherSchedule> userTeacherScheduleList = new List<UserTeacherSchedule>();
        private static List<UserHW> userHwList = new List<UserHW>();
        public void TeacherScheduleSwitch(long chatId, bool state, string teacher = "")
        {
            foreach (var item in userTeacherScheduleList)
            {
                if (item.ChatId == chatId)
                {
                    item.IsActive = state;
                    item.TeacherName = teacher;
                    return;
                }
                   
            }

            userTeacherScheduleList.Add(new UserTeacherSchedule{ChatId = chatId, IsActive = state, TeacherName = teacher});
        }

        public bool IsTeacherScheduleEnable(long chatId)
        {
            foreach (var item in userTeacherScheduleList)
            {
                if (item.ChatId == chatId && item.IsActive)
                    return true;
            }

            return false;
        }

        public void AddTeachersList(long chatId, List<Teacher> teachersList)
        {
            foreach (var item in userTeacherScheduleList)
            {
                if (item.ChatId == chatId)
                    item.TeachersList = teachersList;
            }
        }

        public string GetTeacherName(long chatId)
        {
            foreach (var item in userTeacherScheduleList)
            {
                if (item.ChatId == chatId)
                    return item.TeacherName;
            }

            return "";
        }

        public List<Teacher> GetTeacherList(long chatId)
        {
            foreach (var item in userTeacherScheduleList)
            {
                if (item.ChatId == chatId)
                    return item.TeachersList;
            }

            return new List<Teacher>();
        }

        public void HWSwitch(long chatId, bool state, string date = "")
        {
            foreach (var item in userHwList)
            {
                if (item.ChatId == chatId)
                {
                    item.IsActive = state;
                    item.Date = date;
                    return;
                }

            }

            userHwList.Add(new UserHW { ChatId = chatId, IsActive = state,Date = date});
        }

        public bool IsHWEnable(long chatId)
        {
            foreach (var item in userHwList)
            {
                if (item.ChatId == chatId && item.IsActive)
                    return true;
            }

            return false;
        }

        public string GetDate(long chatId)
        {
            foreach (var item in userHwList)
            {
                if (item.ChatId == chatId)
                    return item.Date;
            }

            return "";
        }
    }
}
