namespace TelegrammAspMvcDotNetCoreBot.Models
{
	public class Facility
    {
		public int FacilityId { get; set; }
		public string Name { get; set; }
		public University University { get; set; }
	}
}
