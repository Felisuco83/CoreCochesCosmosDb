using CoreCochesCosmosDb.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCochesCosmosDb.Services
{
    public class ServiceCosmosDb
    {
        DocumentClient client;
        string bbdd;
        string collection;
        public ServiceCosmosDb(IConfiguration configuration)
        {
            string endpoint = configuration["CosmosDb:endpoint"];
            string primarykey = configuration["CosmosDb:primarykey"];
            this.bbdd = "Vehiculos BBDD";
            this.collection = "VehiculosCollection";
            this.client = new DocumentClient(new Uri(endpoint), primarykey);
        }

        public async Task CrearBbddVehiculosAsync ()
        {
            Database bbdd = new Database() { Id = this.bbdd };
            await this.client.CreateDatabaseAsync(bbdd);
        }

        public async Task CrearColeccioVehiculosAsync ()
        {
            DocumentCollection coleccion = new DocumentCollection() { Id = this.collection };
            await this.client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(this.bbdd), coleccion);
        }

        public async Task InsertarVehiculo (Vehiculo car)
        {
            Uri uri = UriFactory.CreateDocumentCollectionUri(this.bbdd, this.collection);
            await this.client.CreateDocumentAsync(uri, car);
        }

        public List<Vehiculo> GetVehiculos ()
        {
            FeedOptions options = new FeedOptions() { MaxItemCount = -1 };
            string sql = "SELECT * FROM c";
            Uri uri = UriFactory.CreateDocumentCollectionUri(this.bbdd, this.collection);
            IQueryable<Vehiculo> consulta = this.client.CreateDocumentQuery<Vehiculo>(uri, sql, options);
            return consulta.ToList();
        }

        public async Task<Vehiculo> FindVehiculosAsync (string id)
        {
            Uri uri = UriFactory.CreateDocumentUri(this.bbdd, this.collection, id);
            Document document = await this.client.ReadDocumentAsync(uri);
            MemoryStream memory = new MemoryStream();
            using (var stream = new StreamReader(memory))
            {
                document.SaveTo(memory);
                memory.Position = 0;
                Vehiculo car = JsonConvert.DeserializeObject<Vehiculo>(await stream.ReadToEndAsync());
                return car;
            }            
        }

        public async Task ModificarVehiculo (Vehiculo car)
        {
            Uri uri = UriFactory.CreateDocumentUri(this.bbdd, this.collection, car.Id);
            await this.client.ReplaceDocumentAsync(uri, car);
        }

        public async Task EliminarVehiculo(string id)
        {
            Uri uri = UriFactory.CreateDocumentUri(this.bbdd, this.collection, id);
            await this.client.DeleteDocumentAsync(uri);
        }

        public List<Vehiculo> BuscarVehiculosMarca (string marca)
        {
            FeedOptions options = new FeedOptions() { MaxItemCount = -1 };
            Uri uri = UriFactory.CreateDocumentCollectionUri(this.bbdd, this.collection);
            string sql = "select * from c where c.Marca = '" + marca + "'";
            //PODEMOS HACERLO DE LAS 2 MANERAS
            IQueryable<Vehiculo> query = this.client.CreateDocumentQuery<Vehiculo>(uri, sql, options);
            IQueryable<Vehiculo> querylambda = this.client.CreateDocumentQuery<Vehiculo>(uri, options)
                .Where(z => z.Marca == marca);
            return query.ToList();
        }

        public List<Vehiculo> CrearCoches ()
        {
            List<Vehiculo> coches = new List<Vehiculo>()
            {
                new Vehiculo
                {
                    Id = "1", Marca = "Pontiac", Modelo = "FireBird",
                    Motor = new Motor { Tipo = "Gasolina", Caballos = 240, Potencia = 140 }
                    , VelocidadMaxima = 250
                },
                new Vehiculo
                {
                    Id = "2", Marca = "Audi", Modelo = "A6",
                    Motor = new Motor { Tipo = "Diesel", Caballos = 175, Potencia = 175 }
                    , VelocidadMaxima = 220
                },
                new Vehiculo
                {
                    Id = "3", Marca = "Seat", Modelo = "Ibiza"
                    , VelocidadMaxima = 200
                }
            };
            return coches;
        }
    }
}
