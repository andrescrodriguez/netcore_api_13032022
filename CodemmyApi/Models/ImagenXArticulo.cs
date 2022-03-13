using System.ComponentModel.DataAnnotations.Schema;

namespace CodemmyApi.Models
{
    public class ImagenXArticulo
    {
        public int ArticuloId { get; set; }
        public Articulo Articulo { get; set; }
        public int ImagenId { get; set; }
        public Imagen Imagen { get; set; }
    }
}
