using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PresetManager.Model
{
    [DataContract]
    class Preset
    {
        [DataMember(Name="title")]
        public string Title { get; set; }
        [DataMember(Name="explain")]
        public string Explain { get; set; }
        [DataMember(Name="characters")]
        public List<string> Characters { get; set; }
    }
}
