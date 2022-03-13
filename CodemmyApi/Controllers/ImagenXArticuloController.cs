using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodemmyApi;
using CodemmyApi.Models;

namespace CodemmyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagenXArticuloController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ImagenXArticuloController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ImagenXArticulo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImagenXArticulo>>> GetImagenXArticulo()
        {
            return await _context.ImagenXArticulo.ToListAsync();
        }

        // GET: api/ImagenXArticulo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ImagenXArticulo>> GetImagenXArticulo(int id)
        {
            var imagenXArticulo = await _context.ImagenXArticulo.FindAsync(id);

            if (imagenXArticulo == null)
            {
                return NotFound();
            }

            return imagenXArticulo;
        }

        // PUT: api/ImagenXArticulo/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImagenXArticulo(int id, ImagenXArticulo imagenXArticulo)
        {
            if (id != imagenXArticulo.ImagenId)
            {
                return BadRequest();
            }

            _context.Entry(imagenXArticulo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImagenXArticuloExists(id))
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

        // POST: api/ImagenXArticulo
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ImagenXArticulo>> PostImagenXArticulo(ImagenXArticulo imagenXArticulo)
        {
            _context.ImagenXArticulo.Add(imagenXArticulo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ImagenXArticuloExists(imagenXArticulo.ImagenId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetImagenXArticulo", new { id = imagenXArticulo.ImagenId }, imagenXArticulo);
        }

        // DELETE: api/ImagenXArticulo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ImagenXArticulo>> DeleteImagenXArticulo(int id)
        {
            var imagenXArticulo = await _context.ImagenXArticulo.FindAsync(id);
            if (imagenXArticulo == null)
            {
                return NotFound();
            }

            _context.ImagenXArticulo.Remove(imagenXArticulo);
            await _context.SaveChangesAsync();

            return imagenXArticulo;
        }

        private bool ImagenXArticuloExists(int id)
        {
            return _context.ImagenXArticulo.Any(e => e.ImagenId == id);
        }
    }
}
