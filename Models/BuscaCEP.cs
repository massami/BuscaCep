using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuscaCep.Models.BuscaCep
{
    public class Resultado
    {
        public string Status { get; set; }
        public string StatusMensagem { get; set; }
        public string Origem { get; set; }

        // Dados do Endereço
        public string Logradouro { get; set; }
        public string Bairro { get; set; }
        public string UF { get; set; }
        public string  Cidade { get; set; }
        public string CEP { get; set; }
    }
}