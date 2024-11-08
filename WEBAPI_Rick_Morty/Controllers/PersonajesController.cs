using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WEBAPI_Rick_Morty.Models;

namespace WEBAPI_Rick_Morty.Controllers
{
    public class PersonajesController
    {
        private HttpClient client;

        public PersonajesController()
        {
            client = new HttpClient();
        }

        public async Task<ListPersonajes>GetAllPersonajes()
        {
            try
            {
                ListPersonajes listPersonajes = new ListPersonajes();
                HttpResponseMessage response = await client.GetAsync("https://rickandmortyapi.com/api/character");

                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync();

                listPersonajes = JsonConvert.DeserializeObject<ListPersonajes>(responseJson);

                return listPersonajes;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error al obtener personajes: {ex.Message}", ex);
            }
        }




    }
}
