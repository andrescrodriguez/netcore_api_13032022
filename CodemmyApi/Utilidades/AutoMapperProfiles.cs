
using AutoMapper;
using CodemmyApi.DTO;
using CodemmyApi.DTO.CreacionDTO;
using CodemmyApi.Models;

namespace CodemmyApi.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Articulo, ArticuloDTO>().ReverseMap(); // desde ArticuloDTO a Articulo
            CreateMap<Articulo, ArticuloCreacionDTO>(); // desde Articulo a ArticuloCreacionDTO
            CreateMap<Categoria, CategoriaDTO>()
                .ForMember(x => x.Imagen, options => options.Ignore()); // para ignorar la propiedad imagen
            CreateMap<Categoria, CategoriaDTO>().ReverseMap(); // desde ArticuloDTO a Articulo
        }
    }
}
