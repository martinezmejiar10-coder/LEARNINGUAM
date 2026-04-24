using System.Collections.Generic;

namespace LEARNINGUAM.Web.Models
{
    public class ContenidoArrastrar
    {
        public string texto { get; set; }
        public List<EspacioArrastrar> espacios { get; set; }
    }

    public class EspacioArrastrar
    {
        public int id { get; set; }
        public string palabra { get; set; }
    }
}