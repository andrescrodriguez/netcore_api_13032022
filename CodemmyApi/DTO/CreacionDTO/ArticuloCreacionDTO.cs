using CodemmyApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodemmyApi.DTO.CreacionDTO
{
    public class ArticuloCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerído")]
        [StringLength(maximumLength:1500)]
        //[PrimeraLetraMayuscula] validacion personalizada, a desarrollar
        public string Titulo { get; set; }
        public string PreLectura { get; set; }
        public string Contenido { get; set; }
        public string NombreDeRuta { get; set; }
        public int IdCategoria { get; set; }
        public Categoria Categoria { get; set; }
        public DateTime? FechaHoraPublicacion { get; set; }
        public DateTime FechaHoraAlta { get; set; }
        public DateTime? FechaHoraUltimaActualizacion { get; set; }
        public DateTime? FechaHoraBaja { get; set; }
        public List<ImagenXArticulo> ImagenesXArticulos { get; set; }
    }
}
