using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities.OrderEntities;
using Core.Entities.ProductEntities;
using Infrastructure.Data.Contexts;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.SeedData
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext storeContext, ILoggerFactory loggerFactory)
        {
            try
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                if (!storeContext.ProductBrands.Any())
                {
                    var brandsData = File.ReadAllText(path + @"/Data/SeedData/brands.json");

                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                    foreach (var brand in brands)
                    {
                        storeContext.ProductBrands.Add(brand);
                    }

                    await storeContext.SaveChangesAsync();
                }

                if (!storeContext.ProductTypes.Any())
                {
                    var typesData = File.ReadAllText(path + @"/Data/SeedData/types.json");

                    var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);

                    foreach (var type in types)
                    {
                        storeContext.ProductTypes.Add(type);
                    }

                    await storeContext.SaveChangesAsync();
                }

                if (!storeContext.Products.Any())
                {
                    var productsData = File.ReadAllText(path + @"/Data/SeedData/products.json");

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
                    var dmData = File.ReadAllText(path + @"/Data/SeedData/delivery.json");

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