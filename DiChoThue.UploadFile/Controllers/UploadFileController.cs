using DiChoThue.UploadFile.Models;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DiChoThue.UploadFile.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadFileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UploadFileController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<ActionResult<string>> UploadFile([FromForm] IFormFile input)
        {
            if (input.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    input.CopyTo(stream);
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
                        .Child(input.FileName)
                        .PutAsync(stream, cancelToken.Token);
                    try
                    {
                        string link = await task;
                        return link;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return "Upload Failed";
        }
    }
}
