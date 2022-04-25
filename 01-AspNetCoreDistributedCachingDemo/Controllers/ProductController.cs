using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreDistributedCachingDemo.Data;
using AspNetCoreDistributedCachingDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AspNetCoreDistributedCachingDemo.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class ProductController : ControllerBase
    //{
    //    private readonly AdventureWorksDbContext _context;

    //    public ProductController(AdventureWorksDbContext context)
    //    {
    //        _context = context;
    //    }

    //    [HttpGet]
    //    public async Task<IActionResult> GetAll()
    //    {
    //        var products = await _context.Products.ToListAsync();

    //        return Ok(products);
    //    }
    //}

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AdventureWorksDbContext _context;
        private readonly IDistributedCache _cache;

        public ProductController(AdventureWorksDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cacheKey = "GET_ALL_PRODUCTS";
            List<Product> products = new List<Product>();

            // Get data from cache
            var cachedData = await _cache.GetAsync(cacheKey);
            if (cachedData != null)
            {
                // If data found in cache, encode and deserialize cached data
                var cachedDataString = Encoding.UTF8.GetString(cachedData);
                products = JsonConvert.DeserializeObject<List<Product>>(cachedDataString);
            }
            else
            {
                // If not found, then fetch data from database
                products = await _context.Products.ToListAsync();

               // serialize data
                var cachedDataString = JsonConvert.SerializeObject(products);
                var newDataToCache = Encoding.UTF8.GetBytes(cachedDataString);

                // set cache options 
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(2))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                // Add data in cache
                await _cache.SetAsync(cacheKey, newDataToCache, options);
            }

            return Ok(products);
        }
    }
 
 
}
