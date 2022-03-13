using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodemmyApi;
using CodemmyApi.Models;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using CodemmyApi.Utilidades;
using CodemmyApi.DTO;

namespace CodemmyApi.Controllers
{
    [Route("api/imagen")]
    [ApiController]
    public class ImagenController : ControllerBase
    {
        private IWebHostEnvironment _env;
        private IHttpContextAccessor _accessor;
        private readonly ApplicationDbContext _context;

        public ImagenController(ApplicationDbContext context, 
            IWebHostEnvironment env,
            IHttpContextAccessor accessor)
        {
            _context = context;
            _env = env;
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
                var queryable = _context.Imagen.AsQueryable();

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

                //queryable.Select(x => new
                //{ 
                //    Id = Convert.ToString(x.Id),
                //    x.Titulo,
                //    x.NombreDeRuta,
                //    CategoriaNombre = x.Categoria.Nombre,
                //    x.FechaHoraPublicacion,
                //    x.FechaHoraAlta,
                //    x.FechaHoraUltimaActualizacion,
                //    x.FechaHoraBaja
                //}).AsQueryable();

                await HttpContext.InsertarParametrosDePaginacionEnCabeceraHTTP(queryable);
                var lista = await queryable.OrderByDescending(x => x.Id).Paginar(paginacionDTO).ToListAsync();

                var baseURL = _accessor.HttpContext.Request.Scheme + "://" +
                        _accessor.HttpContext.Request.Host +
                        _accessor.HttpContext.Request.PathBase;

                foreach (var imagen in lista)
                {
                    imagen.Ruta = baseURL + "/" + imagen.Ruta + "/" + imagen.Nombre + imagen.Extension;
                }

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // GET: api/Imagen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Imagen>> GetImagen(int id)
        {
            var imagen = await _context.Imagen.FindAsync(id);

            if (imagen == null)
            {
                return NotFound();
            }

            return imagen;
        }

        // PUT: api/Imagen/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImagen(int id, Imagen imagen)
        {
            if (id != imagen.Id)
            {
                return BadRequest();
            }

            _context.Entry(imagen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImagenExists(id))
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

        // POST: api/Imagen
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPost]
        //public async Task<ActionResult<Imagen>> PostImagen(Imagen imagen)
        //{
        //    _context.Imagen.Add(imagen);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetImagen", new { id = imagen.Id }, imagen);
        //}

        //[HttpPost("Upload"), DisableRequestSizeLimit]
        //[Produces("application/json")]
        [HttpPost("Upload")]
        public IActionResult Upload(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return BadRequest();
                }

                FileInfo fileInfo = new FileInfo(file.FileName);
                
                if (!ExtensionValida(fileInfo.Extension))
                {
                    return BadRequest();
                }

                string year = DateTime.Now.Year.ToString();
                string month = DateTime.Now.Month.ToString();
                string day = DateTime.Now.Day.ToString();

                var localPath = "FileServer" + "/" + year + "/" + month + "/" + day;
                var absolutePath = Path.Combine(_env.WebRootPath, localPath);

                if (!Directory.Exists(absolutePath)) // si no existe, crea el directorio day
                {
                    Directory.CreateDirectory(absolutePath);
                }

                string nombre = Guid.NewGuid().ToString();
                absolutePath = absolutePath + "/" + nombre + fileInfo.Extension;

                using (var fileStream = new FileStream(absolutePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                Imagen i = new Imagen();

                if (System.IO.File.Exists(absolutePath))
                {
                    i.Nombre = nombre;
                    i.Extension = fileInfo.Extension;
                    i.FechaHoraAlta = DateTime.Now;
                    i.Ruta = localPath;
                    i.TipoDeContenido = file.ContentType;
                    _context.Imagen.Add(i);
                    _context.SaveChanges();
                }

                if (i.Id == 0)
                {
                    // deberia eliminar la imagen de la carpeta
                    return BadRequest("No fué posible guardar la imagen");
                }

                var baseURL = _accessor.HttpContext.Request.Scheme + "://" +
                    _accessor.HttpContext.Request.Host +
                    _accessor.HttpContext.Request.PathBase;

                return Ok(new Imagen
                {
                    Id = i.Id,
                    Nombre = nombre,
                    Extension = fileInfo.Extension,
                    FechaHoraAlta = DateTime.UtcNow,
                    Ruta = baseURL + "/" + localPath + "/" + nombre + fileInfo.Extension,
                    TipoDeContenido = file.ContentType
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        // DELETE: api/Imagen/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Imagen>> DeleteImagen(int id)
        {
            var imagen = await _context.Imagen.FindAsync(id);
            if (imagen == null)
            {
                return NotFound();
            }

            _context.Imagen.Remove(imagen);
            await _context.SaveChangesAsync();

            return imagen;
        }

        private bool ImagenExists(int id)
        {
            return _context.Imagen.Any(e => e.Id == id);
        }

        private bool ExtensionValida(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                    return true;
                case ".jpeg":
                    return true;
                case ".png":
                    return true;
                case ".svg":
                    return true;
                case ".pdf":
                    return true;
                case ".docx":
                    return true;
                case ".xlsx":
                    return true;
                case ".pptx":
                    return true;
                case ".xlsm":
                    return true;
                case ".xlsb":
                    return true;
                case ".xltx":
                    return true;
                case ".txt":
                    return true;
                case ".gif":
                    return true;
                case ".csv":
                    return true;
                case ".odt":
                    return true;
                case ".ods":
                    return true;
                default:
                    return false;
            }
        }
    }
}
