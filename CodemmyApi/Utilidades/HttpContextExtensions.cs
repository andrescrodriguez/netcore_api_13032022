using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CodemmyApi.Utilidades
{
    public static class HttpContextExtensions  // Extensions porque va a ser metodo de extension
    {
        public async static Task InsertarParametrosDePaginacionEnCabeceraHTTP<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Add("cantidadTotalDeRegistros", cantidad.ToString());
        }
    }
}
