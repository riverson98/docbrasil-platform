using DocAssociados.Domain.Validation;

namespace DocAssociados.Domain.Entities;

public sealed class Endereco
{
    public int Id { get; set; }
    public string Cep { get; private set; }
    public string Rua { get; private set; }
    public string Numero { get; private set; }
    public string Bairro { get; private set; }
    public string Cidade { get; private set; }
    public string Estado { get; private set; }
    public string ComprovanteDeResidenciaUpload { get; private set; }
    public DateTime DataDoUpload = DateTime.Now;
    
    public Guid AssociadoId { get; set; }
    public Associado Associado { get; set; }

    public Endereco() {}

    public Endereco(string cep, string rua, string numero, string bairro, string cidade, 
        string estado, string comprovanteDeResidenciaUpload, Associado associado, Guid associadoId)
    {
        ValidaDominio(cep, rua, numero, bairro, cidade, estado, comprovanteDeResidenciaUpload);

        Cep = cep;
        Rua = rua;
        Numero = numero;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
        ComprovanteDeResidenciaUpload = comprovanteDeResidenciaUpload;
        Associado = associado;
        AssociadoId = associadoId;
    }

    public void Atualiza(int id, string cep, string rua, string numero, string bairro, 
        string cidade, string estado, string comprovanteDeResidenciaUpload, Guid associadoId)
    {
        ValidaDominio(cep, rua, numero, bairro, cidade, estado, comprovanteDeResidenciaUpload);
        Id = id;
        Cep = cep;
        Rua = rua;
        Numero = numero;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
        ComprovanteDeResidenciaUpload = comprovanteDeResidenciaUpload;
        AssociadoId = associadoId;
    }

    private void ValidaDominio(string cep, string rua, string numero, string bairro, string cidade, 
        string estado, string comprovanteDeResidenciaUpload)
    {
        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(cep),
            "O cep é obrigatório");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(rua),
            "A rua é obrigatória");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(numero),
            "O número é obrigatório");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(bairro),
            "O bairro é obrigatório");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(cidade),
            "A cidade é obrigatória");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(estado),
            "O estado é obrigatório");

        ValidacaoDeDominioExecption.When(string.IsNullOrEmpty(comprovanteDeResidenciaUpload),
            "O comprovante de residência é obrigatório");
    }
}
