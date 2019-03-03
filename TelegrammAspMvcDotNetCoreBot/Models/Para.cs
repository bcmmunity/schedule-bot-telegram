namespace TelegrammAspMvcDotNetCoreBot.Models
{
    public class Para
    {

        public string name;
        public string time;
        public string prepod;
        public string room;


        public Para(string name, string time, string room, string prepod)
        {
            this.name = name;
            this.time = time;
            this.room = room;
            this.prepod = prepod;
        }
    }
}