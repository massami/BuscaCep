using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Web.Http;
using WebApiAuthenticate.Filters;

namespace BuscaCep.Controllers
{
    public class EnderecoController : ApiController
    {
        const bool EnableViaCep = true;
        const bool EnableWidenet = true;

        // GET api/values
        [BasicAuthentication]
        public string Get(string cep)
        {
            cep = (string.IsNullOrWhiteSpace(cep) ? cep : cep.Replace("-", ""));

            string mensagem = "Informe um CEP válido";
            Models.BuscaCep.Resultado buscaCep = new Models.BuscaCep.Resultado();

            if (System.Text.RegularExpressions.Regex.IsMatch(cep, ("[0-9]{5}")))
            {
                buscaCep = BuscarCep(cep);
                if (!string.IsNullOrWhiteSpace(buscaCep.Cidade))
                {
                    mensagem = string.Empty;
                }
            }

            if (string.IsNullOrEmpty(mensagem))
            { 
                buscaCep.Status = "sucesso";
            } 
            else
            {
                buscaCep.Status = "erro";
                buscaCep.StatusMensagem = mensagem;
            }

            return JsonConvert.SerializeObject(buscaCep);
        }

        private Models.BuscaCep.Resultado BuscarCep(string cep)
        {
            if (System.DateTime.Now.Second % 2 == 0)
            {
                return BuscarViaCep(cep);
            } 
            else
            {
                return BuscarWidenet(cep);
            }
        }

        private Models.BuscaCep.Resultado BuscarViaCep(string cep, bool isFromCatch = false)
        {
            string url = "https://viacep.com.br/ws/" + cep + "/json/";
            try
            {
                if (!EnableViaCep)
                {
                    throw new System.Exception();
                }

                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    string result = webClient.DownloadString(url);
                    var viaCep = JsonConvert.DeserializeObject<Models.ViaCep.Resultado>(result);
                    if (viaCep.erro.HasValue && viaCep.erro.Value && !isFromCatch)
                    {
                        throw new System.Exception();
                    }

                    return ConverterViaCepParaBuscaCep(viaCep);
                }
            }
            catch
            {
                return BuscarWidenet(cep, true);
            }
        }

        private Models.BuscaCep.Resultado BuscarWidenet(string cep, bool isFromCatch = false)
        {
            string url = "https://apps.widenet.com.br/busca-cep/api/cep.json?code=" + cep;
            try
            {
                if (!EnableWidenet) 
                {
                    throw new System.Exception();
                }

                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    string result = webClient.DownloadString(url);
                    var widenet = JsonConvert.DeserializeObject<Models.Widenet.Resultado>(result);
                    if (widenet.status != 200 && !isFromCatch)
                    {
                        throw new System.Exception();
                    }

                    return ConverterWidenetParaBuscaCep(widenet);
                }
            }
            catch
            {
                return BuscarViaCep(cep, true);
            }
        }

        private Models.BuscaCep.Resultado ConverterViaCepParaBuscaCep(Models.ViaCep.Resultado viaCep)
        {
            Models.BuscaCep.Resultado buscaCep = new Models.BuscaCep.Resultado();
            if (viaCep != null && (viaCep.erro == null || (viaCep.erro.HasValue && !viaCep.erro.Value)))
            {
                buscaCep.Logradouro = viaCep.logradouro;
                buscaCep.Bairro = viaCep.bairro;
                buscaCep.Cidade = viaCep.localidade;
                buscaCep.UF = viaCep.uf;
                buscaCep.CEP = (string.IsNullOrWhiteSpace(viaCep.cep) ? null : viaCep.cep.Replace("-", ""));
                buscaCep.Origem = "ViaCEP";
            }

            return buscaCep;
        }

        private Models.BuscaCep.Resultado ConverterWidenetParaBuscaCep(Models.Widenet.Resultado widenet)
        {
            Models.BuscaCep.Resultado buscaCep = new Models.BuscaCep.Resultado();
            if (widenet != null && widenet.status == 200)
            {
                buscaCep.Logradouro = widenet.address;
                buscaCep.Bairro = widenet.district;
                buscaCep.Cidade = widenet.city;
                buscaCep.UF = widenet.state;
                buscaCep.CEP = (string.IsNullOrWhiteSpace(widenet.code) ? null : widenet.code.Replace("-", ""));
                buscaCep.Origem = "Widenet";

                // trata texto com numero no endereço
                if (buscaCep.Logradouro.Contains(" - "))
                {
                    int index = buscaCep.Logradouro.IndexOf(" - ");
                    buscaCep.Logradouro = buscaCep.Logradouro.Substring(0, index + 1).Trim();
                }
            }

            return buscaCep;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
