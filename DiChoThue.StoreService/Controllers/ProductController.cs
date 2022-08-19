using DiChoThue.StoreService.Models;
using DiChoThue.StoreService.Repository.Interface;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiChoThue.StoreService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController:ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductController> _logger;
        private readonly IConfiguration _configuration;

        public ProductController(IProductRepository productRepository, ILogger<ProductController> logger, IConfiguration configuration)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(productRepository));
            _configuration = configuration;

        }

        [HttpGet("GetAllProduct")]
        [ProducesResponseType(typeof(IEnumerable<ProductEntity>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProductEntity>>> GetAllProduct()
        {
            var products = await _productRepository.GetAllProduct();
            return Ok(products);
        }

        [HttpGet("ProductId={id:length(24)}", Name = "GetProductById")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProductEntity), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductEntity>> GetProductById(string id)
        {
            var product = await _productRepository.GetProductById(id);

            if (product == null)
            {
                _logger.LogError($"Product with id: {id}, not found.");
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("StoreId={storeId:length(24)}",Name ="GetProductStore")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProductEntity), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductEntity>> GetProductStore(string storeId)
        {
            var product = await _productRepository.GetProductsStore(storeId);

            if (product == null)
            {
                _logger.LogError($"Product of: {storeId}, not found.");
                return NotFound();
            }
            return Ok(product);
        }


        [HttpGet("Category={category}")]
        [ProducesResponseType(typeof(ProductEntity), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProductEntity>>>GetProductByCategory(string category)
        {
            var product = await _productRepository.GetProductByCategory(category);

            if (product == null)
            {
                _logger.LogError($"Product with id: {category}, not found.");
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost("CreateProduct")]
        [ProducesResponseType(typeof(ProductEntity), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductEntity>> CreateProduct([FromForm] string productJsonData,IFormFile file)
        {
            string link = string.Empty;
            if (file.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(_configuration.GetSection("InforFirebase:ApiKey").Value));
                    var claim = await auth.SignInWithEmailAndPasswordAsync(_configuration.GetSection("InforFireBase:AuthEmail").Value, _configuration.GetSection("InforFirebase:AuthPassword").Value);

                    var cancelToken = new CancellationTokenSource();
                    var task = new FirebaseStorage(
                        _configuration.GetSection("InforFireBase:Bucket").Value,
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(claim.FirebaseToken),
                            ThrowOnCancel = true
                        })
                        .Child("Images")
                        .Child(file.FileName)
                        .PutAsync(stream, cancelToken.Token);
                    try
                    {
                        link = await task;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            var product= JsonConvert.DeserializeObject<ProductEntity>(productJsonData);
            product.ImagesUrl = link;
            await _productRepository.CreateProduct(product);

            return CreatedAtRoute("GetProductById", new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ProductEntity), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductEntity product)
        {
            return Ok(await _productRepository.UpdateProduct(product));
        }

        [HttpPost("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(ProductEntity), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            return Ok(await _productRepository.DeleteProduct(id));
        }
    }
}
