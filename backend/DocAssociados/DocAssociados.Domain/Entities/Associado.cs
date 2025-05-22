using DocAssociados.Domain.Validation;

namespace DocAssociados.Domain.Entities;

public sealed class Associado
{
    public Guid Id { get; set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public DateOnly DataDeNascimento { get; private set; }
    public string Genero {  get; private set; } 
    public int Funcao { get; private set; }
    public int Status { get; private set; }
    public string Cpf {  get; private set; }
    public string? CpfUploadUrl { get; private set; }
    public string? TermoDeAdessaoUploadUrl { get; private set; }
    public string? FichaAssociacaoUploadUrl { get; private set; }
    public string? RequerimentoJudicialUrl { get; private set; }
    public string? CodigoRepresentante {  get; private set; } 
    public string CodigoRepresentanteSuperior {  get; private set; }
    public int? CodigoAssociado { get; private set; }
    public DateTime DataDeCadastro { get; private set; } = DateTime.Now;
    public string? FotoDePerfilUrl { get; private set; }
    public Endereco Endereco { get; set; }

    public Associado() {}

    public Associado(string nome, string email, DateOnly dataDeNascimento, string genero, int funcao, int status, string cpf,
                     string? cpfUploadUrl ,string codigoRepresentante, string codigoRepresentanteSuperior, Endereco endereco,
                     string? termoDeAdessaoUrl, string? fichaAssociacaoUrl, int codigoAssociado, string? fotoDePerfilUrl, string? requerimentoJudicialUrl)
    {
        ValidaDominio(nome, email, dataDeNascimento, genero, funcao, status, cpf, codigoRepresentante, 
            codigoRepresentanteSuperior, cpfUploadUrl, termoDeAdessaoUrl, fichaAssociacaoUrl, requerimentoJudicialUrl);

        Nome = nome;
        Email = email;
        DataDeNascimento = dataDeNascimento;
        Genero = genero;
        Funcao = funcao;
        Status = status;
        Cpf = cpf;
        CodigoRepresentante = codigoRepresentante;
        CodigoRepresentanteSuperior = codigoRepresentanteSuperior;
        Endereco = endereco;
        CpfUploadUrl = cpfUploadUrl;
        TermoDeAdessaoUploadUrl = termoDeAdessaoUrl;
        FichaAssociacaoUploadUrl = fichaAssociacaoUrl;
        CodigoAssociado = codigoAssociado;
        FotoDePerfilUrl = fotoDePerfilUrl;
        RequerimentoJudicialUrl = requerimentoJudicialUrl;
    }


    public void Atualiza(Guid id, string nome, string email, DateOnly dataDeNascimento, string genero, int funcao, int status, string cpf,
                        string cpfUploadUrl, string codigoRepresentante, string codigoRepresentanteSuperior,
                        string termoDeAdessaoUrl, string fichaAssociacaoUrl, string fotoDePerfilUrl, string requerimento)
    {
        ValidaDominio(nome, email, dataDeNascimento, genero, funcao, status, cpf, cpfUploadUrl, codigoRepresentante, codigoRepresentanteSuperior,
            termoDeAdessaoUrl, fichaAssociacaoUrl, requerimento);

        Id = id;
        Nome = nome;
        Email = email;
        DataDeNascimento = dataDeNascimento;
        Genero = genero;
        Funcao = funcao;
        Status = status;
        Cpf = cpf;
        CodigoRepresentante = codigoRepresentante;
        CodigoRepresentanteSuperior = codigoRepresentanteSuperior;
        CpfUploadUrl = cpfUploadUrl;
        TermoDeAdessaoUploadUrl = termoDeAdessaoUrl;
        FichaAssociacaoUploadUrl = fichaAssociacaoUrl;
        FotoDePerfilUrl = fotoDePerfilUrl;
        RequerimentoJudicialUrl = requerimento;
    }

    private void ValidaDominio(string nome, string email, DateOnly dataDeNascimento, string genero, int funcao, int status, string cpf,
                                string? cpfUploadUrl, string codigoRepresentante, string codigoRepresentanteSuperior,
                                string? termoDeAdessaoUrl, string? fichaAssociacaoUrl, string? requerimento)
    {
        ValidacaoDeDominioException.When(string.IsNullOrEmpty(nome),
            "O nome é obrigatório");

        ValidacaoDeDominioException.When(string.IsNullOrEmpty(email),
            "O e-mail é obrigatório");

        ValidacaoDeDominioException.When(dataDeNascimento.Equals(DateOnly.MinValue),
            "A data de nascimento é obrigatória");

        ValidacaoDeDominioException.When(string.IsNullOrEmpty(genero),
            "O genêro é obrigatório");

        ValidacaoDeDominioException.When(funcao < 0,
            "A função é obrigatória");

        ValidacaoDeDominioException.When(status < 0,
            "O status é obrigatório");

        ValidacaoDeDominioException.When(string.IsNullOrEmpty(cpf),
            "O cpf é obrigatório");

        ValidacaoDeDominioException.When(string.IsNullOrEmpty(codigoRepresentanteSuperior),
            "O codigo do representante superior é obrigatório");

        ValidacaoDeDominioException.When(string.IsNullOrEmpty(termoDeAdessaoUrl),
            "O termo de adesão é obrigatório");

        ValidacaoDeDominioException.When(string.IsNullOrEmpty(fichaAssociacaoUrl),
            "A ficha de associação é obrigatório");

        ValidacaoDeDominioException.When(string.IsNullOrEmpty(cpfUploadUrl),
            "A foto do cpf é obrigatório");

        ValidacaoDeDominioException.When(string.IsNullOrEmpty(requerimento),
            "A requerimento judicial é obrigatório");
    }
}
