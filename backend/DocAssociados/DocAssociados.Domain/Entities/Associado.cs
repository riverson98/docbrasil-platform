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
    public string CpfUploadUrl { get; private set; }
    public string TermoDeAdessaoUploadUrl { get; private set; }
    public string FichaAssociacaoUploadUrl { get; private set; }
    public string? CodigoRepresentante {  get; private set; } 
    public string CodigoRepresentanteSuperior {  get; private set; }
    public DateTime DataDeCadastro { get; private set; } = DateTime.Now;
    public Endereco Endereco { get; set; }

    public Associado() {}

    public Associado(string nome, string email, DateOnly dataDeNascimento, string genero, int funcao, int status, string cpf,
                     string cpfUploadUrl ,string codigoRepresentante, string codigoRepresentanteSuperior, Endereco endereco,
                     string termoDeAdessaoUrl, string fichaAssociacaoUrl)
    {
        ValidaDominio(nome, email, dataDeNascimento, genero, funcao, status, cpf, codigoRepresentante, 
            codigoRepresentanteSuperior, cpfUploadUrl, termoDeAdessaoUrl, fichaAssociacaoUrl);

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
    }


    public void Atualiza(Guid id, string nome, string email, DateOnly dataDeNascimento, string genero, int funcao, int status, string cpf,
                        string cpfUploadUrl, string codigoRepresentante, string codigoRepresentanteSuperior,
                        string termoDeAdessaoUrl, string fichaAssociacaoUrl, Endereco endereco)
    {
        ValidaDominio(nome, email, dataDeNascimento, genero, funcao, status, cpf, cpfUploadUrl, codigoRepresentante, codigoRepresentanteSuperior,
            termoDeAdessaoUrl, fichaAssociacaoUrl);

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
        Endereco = endereco;
        CpfUploadUrl = cpfUploadUrl;
        TermoDeAdessaoUploadUrl = termoDeAdessaoUrl;
        FichaAssociacaoUploadUrl = fichaAssociacaoUrl;
    }

    private void ValidaDominio(string nome, string email, DateOnly dataDeNascimento, string genero, int funcao, int status, string cpf,
                                string cpfUploadUrl, string codigoRepresentante, string codigoRepresentanteSuperior,
                                string termoDeAdessaoUrl, string fichaAssociacaoUrl)
    {
        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(nome),
            "O nome é obrigatório");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(email),
            "O e-mail é obrigatório");

        ValidacaoDeDominioExecption.When(dataDeNascimento.Equals(DateOnly.MinValue),
            "A data de nascimento é obrigatória");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(genero),
            "O genêro é obrigatório");

        ValidacaoDeDominioExecption.When(funcao < 0,
            "A função é obrigatória");

        ValidacaoDeDominioExecption.When(status < 0,
            "O status é obrigatório");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(cpf),
            "O cpf é obrigatório");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(codigoRepresentante),
            "O codigo do representante é obrigatório");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(codigoRepresentanteSuperior),
            "O codigo do representante superior é obrigatório");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(termoDeAdessaoUrl),
            "O termo de adesão é obrigatório");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(fichaAssociacaoUrl),
            "A ficha de associação é obrigatório");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(cpfUploadUrl),
            "A foto do cpf é obrigatório");
    }
}
