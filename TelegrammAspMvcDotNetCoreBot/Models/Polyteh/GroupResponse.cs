using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TelegrammAspMvcDotNetCoreBot.Models.Polyteh
{
    public class GroupResponse
    {
        [JsonProperty("groups")]
        public List<string> GroupsList { get; set; }
    }
}
