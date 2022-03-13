using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodemmyApi;
using CodemmyApi.Models;
using CodemmyApi.DTO;
using CodemmyApi.Utilidades;

namespace CodemmyApi.Controllers.Front
{
    [Route("api/articulofront")]
    [ApiController]
    public class ArticuloFrontController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private IHttpContextAccessor _accessor;

        public ArticuloFrontController(ApplicationDbContext context,
            IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Articulo>>> GetArticulo([FromQuery] PaginacionDTO paginacionDTO)
        {
            var baseURL = _accessor.HttpContext.Request.Scheme + "://" + _accessor.HttpContext.Request.Host + _accessor.HttpContext.Request.PathBase;

            var queryable = _context.Articulo.Select(x => new
            {
                Id = Convert.ToString(x.Id),
                x.Titulo,
                x.NombreDeRuta,
                CategoriaNombre = x.Categoria.Nombre,
                x.FechaHoraPublicacion,
                x.FechaHoraAlta,
                x.FechaHoraUltimaActualizacion,
                x.FechaHoraBaja,
                ImagenRuta = baseURL 
                    + "/" 
                    + x.ImagenesXArticulos.FirstOrDefault().Imagen.Ruta 
                    + "/" 
                    + x.ImagenesXArticulos.FirstOrDefault().Imagen.Nombre 
                    + x.ImagenesXArticulos.FirstOrDefault().Imagen.Extension
            }).AsQueryable();

            await HttpContext.InsertarParametrosDePaginacionEnCabeceraHTTP(queryable);
            var lista = await queryable.OrderBy(x => x.Titulo).Paginar(paginacionDTO).ToListAsync();
            return Ok(lista);
        }

        [HttpGet("getById/{id}")]
        public async Task<ActionResult<Articulo>> GetArticulo(int id)
        {
            var articulo = await _context.Articulo
                .Include(x => x.ImagenesXArticulos)
                .ThenInclude(x => x.Imagen)
                .Include(x => x.Categoria)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (articulo == null)
            {
                return NotFound();
            }

            var baseURL = _accessor.HttpContext.Request.Scheme + "://" + _accessor.HttpContext.Request.Host + _accessor.HttpContext.Request.PathBase;

            foreach (var item in articulo.ImagenesXArticulos)
            {
                if (item.Imagen != null)
                {
                    var ruta = baseURL + "/" + item.Imagen.Ruta + "/" + item.Imagen.Nombre + item.Imagen.Extension;
                    item.Imagen.Ruta = ruta;
                }
            }

            return articulo;
        }
    }
}
