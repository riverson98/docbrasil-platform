namespace DocAssociados.Domain.Entities;

public class ResultadoPaginado<T>
{
    public List<T> Itens { get; set; } = new List<T>();
    public int TotalDeItens { get; set; }
    public int Pagina { get; set; }
    public int QuantidadeDeItensPorPagina { get; set; }
    public int TotalDePaginas => (int)Math.Ceiling((double)TotalDeItens / QuantidadeDeItensPorPagina);
}
