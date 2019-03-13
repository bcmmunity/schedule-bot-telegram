namespace TelegrammAspMvcDotNetCoreBot.Models
{
    public class Para
    {

        public string Name;
        public string Time;
        public string Prepod;
        public string Room;


        public Para(string name, string time, string room, string prepod)
        {
            Name = name;
            Time = time;
            Room = room;
            Prepod = prepod;
        }
    }
}