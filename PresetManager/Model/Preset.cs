using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresetManager.Model
{
    class Preset
    {
        public string Title { get; set; }
        public string Explain { get; set; }
        public List<string> Characters { get; set; }
    }
}
