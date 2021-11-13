using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClienteVuelosa181G0250.Models
{
    public class VueloClase
    {
        [JsonProperty("hora")]
        public string Hora { get; set; }
        [JsonProperty("destino")]
        public string Destino { get; set; }
        [JsonProperty("vuelo")]
        public string Vuelo { get; set; }
        [JsonProperty("estado")]
        public string Estado { get; set; }
    }
}
