using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PresetManager.Model
{
    [DataContract]
    class Preset
    {
        public Preset()
        {
            Title = "";
            Explain = "";
            Characters = new List<string>();
        }

        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "explain")]
        public string Explain { get; set; }
        [DataMember(Name = "characters")]
        public List<string> Characters { get; set; }
    }
}
