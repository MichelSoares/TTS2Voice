using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS2Voice.Model
{
    public class ArquivoCSVMap : ClassMap<ArquivoCSV>
    {
        public ArquivoCSVMap()
        {
            Map(m => m.Arquivo).Name("Arquivo");
            Map(m => m.Frase).Name("Frase");
        }
    }
}
