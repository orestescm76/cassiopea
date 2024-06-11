using Newtonsoft.Json;
using System;

namespace Cassiopeia.Model
{
    public class Genre
    {
        public String Id { get; set; }

        [JsonIgnore]
        public String Name { get; set; }

        public Genre(String i) { Id = i; Name = ""; }
    }
}