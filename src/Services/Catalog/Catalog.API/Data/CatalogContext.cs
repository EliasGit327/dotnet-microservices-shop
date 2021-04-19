using System;
using Catalog.API.ConfigClasses;
using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public IMongoCollection<Product> Products { get; }
        private readonly IOptions<DatabaseSettings> _databaseSettings;

        public CatalogContext
        (
            IConfiguration configuration,
            IOptions<DatabaseSettings> databaseSettings
        )
        {
            _databaseSettings = databaseSettings;
            var client = new MongoClient(_databaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.Value.DatabaseName);
            this.Products = database.GetCollection<Product>(_databaseSettings.Value.CollectionName);

            // Fills database with dummy data in case when it is empty.
            bool existProduct = Products.Find(p => true).Any();
            if (!existProduct)
            {
                CatalogContextSeed.SeedData(Products);
            }
        }

    }
}
