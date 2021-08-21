using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Models.OrderModels;
using Domain.Models.ProductModels;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;

namespace Persistence.Seeds
{
    public class StoreContextSeed
    {
        public class ProductSeedModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string PictureUrl { get; set; }
            public decimal Price { get; set; }
            public int ProductTypeId { get; set; }
            public int ProductBrandId { get; set; }
        }
        public static async Task SeedAsync(StoreContext storeContext, ILoggerFactory loggerFactory)
        {
            try
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;

                if (!storeContext.ProductBrands.Any())
                {
                    var brandsData = File.ReadAllText(path + @"/Seeds/JsonData/brands.json");

                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                    foreach (var brand in brands)
                    {
                        storeContext.ProductBrands.Add(brand);
                    }

                    await storeContext.SaveChangesAsync();
                }

                if (!storeContext.ProductTypes.Any())
                {
                    var typesData = File.ReadAllText(path + @"/Seeds/JsonData/types.json");

                    var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);

                    foreach (var type in types)
                    {
                        storeContext.ProductTypes.Add(type);
                    }

                    await storeContext.SaveChangesAsync();
                }

                if (!storeContext.Products.Any())
                {
                    var productsData = File.ReadAllText(path + @"/Seeds/JsonData/products.json");

                    var products = JsonSerializer.Deserialize<List<ProductSeedModel>>(productsData);

                    foreach (var product in products)
                    {
                        var pictureFileName = product.PictureUrl.Substring(16);

                        var productWithPicture = new Product
                        {
                            Name = product.Name,
                            Description = product.Description,
                            Price = product.Price,
                            ProductBrandId = product.ProductBrandId,
                            ProductTypeId = product.ProductTypeId
                        };

                        productWithPicture.AddPhoto(product.PictureUrl, pictureFileName);
                        productWithPicture.AddReview(3, "Best product ever");
                        storeContext.Products.Add(productWithPicture);
                    }

                    await storeContext.SaveChangesAsync();
                }

                if (!storeContext.DeliveryMethods.Any())
                {
                    var dmData = File.ReadAllText(path + @"/Seeds/JsonData/delivery.json");

                    var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);

                    foreach (var deliveryMethod in deliveryMethods)
                    {
                        storeContext.DeliveryMethods.Add(deliveryMethod);
                    }

                    await storeContext.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                var logger = loggerFactory.CreateLogger<StoreContextSeed>();
                logger.LogError(exception, "An error occurred during seeding");
            }
        }
    }
}