using AutoMapper;
using CodemmyApi.DTO;
using CodemmyApi.Models;
using CodemmyApi.Utilidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodemmyApi.Controllers
{
    [Route("api/Articulo")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class ArticuloController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private IHttpContextAccessor _accessor;

        public ArticuloController(ApplicationDbContext context, IMapper mapper,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor accessor)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _accessor = accessor;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginacionDTO paginacionDTO,
            string titulo,
            string idCategoria,
            DateTime? desde = null,
            DateTime? hasta = null)
        {
            //var queryable = _context.Articulo.AsQueryable();
            //await HttpContext.InsertarParametrosDePaginacionEnCabeceraHTTP(queryable);
            //var lista = await queryable.OrderBy(x => x.Titulo).Paginar(paginacionDTO).ToListAsync();
            //return _mapper.Map<List<ArticuloDTO>>(lista);

            try
            {
                var queryable = _context.Articulo.AsQueryable();

                //if (!string.IsNullOrWhiteSpace(titulo))
                //{
                //    queryable.Where(x => x.Titulo.ToLower().Trim().Contains(titulo.ToLower().Trim()));
                //}

                //if (!string.IsNullOrWhiteSpace(idCategoria))
                //{
                //    queryable.Where(x => x.IdCategoria == Convert.ToInt32(idCategoria));
                //}

                //if (desde != null && hasta != null)
                //{
                //    queryable.Where(x => x.FechaHoraPublicacion >= desde && x.FechaHoraPublicacion <= hasta);
                //}

                queryable.Select(x => new
                {
                    Id = Convert.ToString(x.Id),
                    x.Titulo,
                    x.NombreDeRuta,
                    CategoriaNombre = x.Categoria.Nombre,
                    x.FechaHoraPublicacion,
                    x.FechaHoraAlta,
                    x.FechaHoraUltimaActualizacion,
                    x.FechaHoraBaja
                }).AsQueryable();

                await HttpContext.InsertarParametrosDePaginacionEnCabeceraHTTP(queryable);
                var lista = await queryable.OrderBy(x => x.Titulo).Paginar(paginacionDTO).ToListAsync();
                return Ok(lista);
                //return _mapper.Map<List<ArticuloDTO>>(lista);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Articulo>> Get(int id)
        {
            try
            {
                var articulo = await _context.Articulo.Include(x => x.ImagenesXArticulos).ThenInclude(x => x.Imagen).FirstOrDefaultAsync(x => x.Id == id);

                if (articulo == null)
                {
                    return NotFound();
                }

                if (articulo.ImagenesXArticulos.Any())
                {
                    var imagentXArticulo = articulo.ImagenesXArticulos.FirstOrDefault();

                    if (imagentXArticulo.Imagen != null)
                    {
                        var baseURL = _accessor.HttpContext.Request.Scheme + "://" +
                        _accessor.HttpContext.Request.Host +
                        _accessor.HttpContext.Request.PathBase;
                        var imagen = articulo.ImagenesXArticulos.FirstOrDefault().Imagen;
                        var ruta = baseURL + "/" + imagen.Ruta + "/" + imagen.Nombre + imagen.Extension;
                        articulo.ImagenesXArticulos.FirstOrDefault().Imagen.Ruta = ruta;
                    }
                }

                return articulo;
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticulo(int id, ArticuloDTO articuloDTO)
        {
            var articulo = await _context.Articulo.FindAsync(id);

            if (articulo == null)
            {
                return NotFound();
            }

            try
            {
                articuloDTO.Id = id;
                articulo = _mapper.Map(articuloDTO, articulo);
                _context.Entry(articulo).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                if (articuloDTO.IdImagen > 0)
                {
                    if (_context.ImagenXArticulo.Any(x => x.ArticuloId == id))
                    {
                        var imagenXArticulo = _context.ImagenXArticulo.FirstOrDefault(x => x.ArticuloId == id);
                        _context.Remove(imagenXArticulo);
                        await _context.SaveChangesAsync();

                        imagenXArticulo = new ImagenXArticulo()
                        {
                            ArticuloId = id,
                            ImagenId = articuloDTO.IdImagen
                        };

                        _context.Add(imagenXArticulo);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ArticuloExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Articulo>> PostArticulo(ArticuloDTO articuloDTO)
        {
            try
            {
                // se guarda el articulo
                var articulo = new Articulo();
                articulo = _mapper.Map(articuloDTO, articulo);
                articulo.FechaHoraAlta = DateTime.Now;
                _context.Articulo.Add(articulo);
                await _context.SaveChangesAsync();

                // si tiene imagen, se guarda la referencia
                if (articuloDTO.IdImagen > 0)
                {
                    var imagenXArticulo = new ImagenXArticulo()
                    {
                        ArticuloId = articulo.Id,
                        ImagenId = articuloDTO.IdImagen
                    };
                    _context.ImagenXArticulo.Add(imagenXArticulo);
                    await _context.SaveChangesAsync();
                }

                return CreatedAtAction("Get", new { id = articulo.Id }, articulo);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Articulo>> DeleteArticulo(int id)
        {
            var articulo = await _context.Articulo.FindAsync(id);
            if (articulo == null)
            {
                return NotFound();
            }

            // se eliminan las referencias a imagenes asociadas al articulo
            if (_context.ImagenXArticulo.Any(x => x.ArticuloId == id))
            {
                foreach (var item in _context.ImagenXArticulo.Where(x => x.ArticuloId == id).ToList())
                {
                    _context.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }

            // se elimina el articulo
            _context.Articulo.Remove(articulo);
            await _context.SaveChangesAsync();

            return articulo;
        }

        private bool ArticuloExists(int id)
        {
            return _context.Articulo.Any(e => e.Id == id);
        }
    }
}
