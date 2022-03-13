using AutoMapper.Configuration;
using CodemmyApi.DTO;
using CodemmyApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CodemmyApi.Controllers
{
    [Route("api/cuenta")]
    [ApiController]
    public class CuentaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public CuentaController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        // GET: api/Cuenta
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<RespuestaAutenticacionDTO>>> Get()
        //{
        //    return new List<RespuestaAutenticacionDTO>().ToList();
        //}

        [HttpPost("crear")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Crear([FromBody] CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            // setear usuario
            var usuario = new IdentityUser() { UserName = credencialesUsuarioDTO.Email, Email = credencialesUsuarioDTO.Email };
            // crear usuario
            var resultado = await _userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuarioDTO);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var resultado = await _signInManager.PasswordSignInAsync(
                credencialesUsuarioDTO.Email,
                credencialesUsuarioDTO.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuarioDTO);
            }
            else
            {
                return BadRequest("Login incorrecto.");
            }
        }

        private async Task<RespuestaAutenticacionDTO> ConstruirToken(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            // claims
            var claims = new List<Claim>()
            {
                new Claim("email", credencialesUsuarioDTO.Email)
            };

            var usuario = await _userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);
            var claimsDB = await _userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDB);

            // key
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            // expiracion
            var expiracion = DateTime.UtcNow.AddDays(1);

            // token
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiracion,
                signingCredentials: creds
            );

            return new RespuestaAutenticacionDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiracion
            };
        }

        // =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =
        // =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =
        // metodos convensionales

        

        [HttpPost]
        public async Task<IActionResult> PostCuenta([FromBody] CuentaDTO cuentaDTO)
        {
            // setear usuario
            var usuario = new IdentityUser() { UserName = cuentaDTO.Email, Email = cuentaDTO.Email };
            // crear usuario
            IdentityResult resultado = await _userManager.CreateAsync(usuario, cuentaDTO.Password);

            if (!string.IsNullOrWhiteSpace(cuentaDTO.Claim))
            {
                await _userManager.AddClaimAsync(usuario, new Claim("role", cuentaDTO.Claim.ToLower().Trim()));
            }

            if (resultado.Succeeded)
            {
                return Ok(resultado);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        // =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =
        // =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =  =

        //public async Task<ActionResult<CredencialesUsuarioDTO>> Get(int id)
        //{
        //    try
        //    {
        //        var articulo = await _context.Articulo.Include(x => x.ImagenesXArticulos).ThenInclude(x => x.Imagen).FirstOrDefaultAsync(x => x.Id == id);

        //        if (articulo == null)
        //        {
        //            return NotFound();
        //        }



        //        return articulo;
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest();
        //    }
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutArticulo(int id, ArticuloDTO articuloDTO)
        //{
        //    var articulo = await _context.Articulo.FindAsync(id);

        //    if (articulo == null)
        //    {
        //        return NotFound();
        //    }

        //    try
        //    {
        //        //articuloDTO.Id = id;
        //        //articulo = _mapper.Map(articuloDTO, articulo);
        //        _context.Entry(articulo).State = EntityState.Modified;
        //        await _context.SaveChangesAsync();

        //        //if (articuloDTO.IdImagen > 0)
        //        //{
        //        //    if (_context.ImagenXArticulo.Any(x => x.ArticuloId == id))
        //        //    {
        //        //        var imagenXArticulo = _context.ImagenXArticulo.FirstOrDefault(x => x.ArticuloId == id);
        //        //        _context.Remove(imagenXArticulo);
        //        //        await _context.SaveChangesAsync();

        //        //        imagenXArticulo = new ImagenXArticulo()
        //        //        {
        //        //            ArticuloId = id,
        //        //            ImagenId = articuloDTO.IdImagen
        //        //        };

        //        //        _context.Add(imagenXArticulo);
        //        //        await _context.SaveChangesAsync();
        //        //    }
        //        //}
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        //if (!ArticuloExists(id))
        //        //{
        //        //    return NotFound();
        //        //}
        //        //else
        //        //{
        //        //    throw;
        //        //}
        //    }

        //    return NoContent();
        //}








    }
}
