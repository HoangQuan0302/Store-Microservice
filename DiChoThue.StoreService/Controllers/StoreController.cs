using DiChoThue.StoreService.Models;
using DiChoThue.StoreService.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DiChoThue.StoreService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreRepository _storeRepository;
        private readonly ILogger<StoreController> _logger;

        public StoreController(IStoreRepository storeRepository, ILogger<StoreController> logger)
        {
            _storeRepository = storeRepository ?? throw new ArgumentNullException(nameof(storeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(storeRepository));
        }

        [HttpGet("UserId={UserId}",Name = "GetStore")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(StoreEntity), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<StoreEntity>> GetStore(string UserId)
        {
            var _store = await _storeRepository.GetStore(UserId);
            return _store;
        }

        [HttpPost("RegisterStore")]
        [ProducesResponseType(typeof(StoreEntity), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<StoreEntity>> RegisterStore([FromBody] StoreEntity store)
        {
            await _storeRepository.RegisterStore(store);

            return CreatedAtRoute("GetStore", new { id = store.Id }, store);
        }

    }
}
