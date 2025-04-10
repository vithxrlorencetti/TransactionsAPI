using System.Text.Json.Serialization;

namespace TransactionsAPI.DTOs
{
    public class ViaCepResponseDTO
    {
        [JsonPropertyName("cep")]
        public string PostalCode { get; set; } = string.Empty;

        [JsonPropertyName("logradouro")]
        public string Street { get; set; } = string.Empty;

        [JsonPropertyName("complemento")]
        public string Complement { get; set; } = string.Empty;

        [JsonPropertyName("bairro")]
        public string Neighborhood { get; set; } = string.Empty;

        [JsonPropertyName("localidade")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("uf")]
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("erro")]
        public bool? Error { get; set; }
    }
}

