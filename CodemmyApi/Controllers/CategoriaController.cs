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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodemmyApi.Controllers
{
    [Route("api/categoria")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class CategoriaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public CategoriaController(ApplicationDbContext context, IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoriaDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {   
            var queryable = _context.Categoria.AsQueryable();
            await HttpContext.InsertarParametrosDePaginacionEnCabeceraHTTP(queryable);
            var lista = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();
            return _mapper.Map<List<CategoriaDTO>>(lista);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous] // con allow anonymus es posible que este metodo sea accedido sin jwt 
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            var categoria = await _context.Categoria.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return _mapper.Map<CategoriaDTO>(categoria);
        }

        /// <summary>
        /// Listado de todas las categorias del usuario con acceso público
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAll")] 
        [AllowAnonymous] // con allow anonymus es posible que este metodo sea accedido sin jwt 
        public async Task<ActionResult<List<CategoriaDTO>>> GetAll()
        {
            var categorias = await _context.Categoria.OrderBy(x => x.Nombre).ToListAsync();

            if (categorias == null)
            {
                return NotFound();
            }
            
            var categoriasDTO = new List<CategoriaDTO>();
            _mapper.Map(categorias, categoriasDTO);

            return categoriasDTO;
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, CategoriaDTO categoriaDTO)
        {
            var categoria = await _context.Categoria.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            categoriaDTO.Id = id;
            categoria = _mapper.Map(categoriaDTO, categoria);
            _context.Entry(categoria).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(id))
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
        public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDTO) // en vez de fromBody va fromForm para recuperar la imagen
        {
            // identity - para que funcione el email, tengo que apagar un mapeo automatico que hace identity (desde la clase startup)
            //var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email").Value;
            //var usuario = await _userManager.FindByEmailAsync(email);
            //var usuarioId = usuario.Id;

            var categoria = new Categoria();
            categoria = _mapper.Map(categoriaDTO, categoria);
            _context.Categoria.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = categoria.Id }, categoria);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Categoria>> Delete(int id)
        {
            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            _context.Categoria.Remove(categoria);
            await _context.SaveChangesAsync();

            return categoria;
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categoria.Any(e => e.Id == id);
        }
    }
}
