using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace CodemmyApi.DTO
{
    public class CategoriaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombreDeRuta { get; set; }
        public List<ArticuloDTO> Articulos { get; set; }
        public IFormFile Imagen { get; set; }
    }
}
