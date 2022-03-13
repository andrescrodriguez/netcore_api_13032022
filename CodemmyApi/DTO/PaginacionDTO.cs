namespace CodemmyApi.DTO
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;

        private int registrosPorPagina = 10;
        private readonly int _cantidadMaximaDeRegistrosPorPagina = 50;

        public int RegistrosPorPagina 
        { 
            get 
            {
                return registrosPorPagina;
            } 
            set 
            { 
                registrosPorPagina = (value > _cantidadMaximaDeRegistrosPorPagina ? _cantidadMaximaDeRegistrosPorPagina : value);
            } 
        }
    }
}
