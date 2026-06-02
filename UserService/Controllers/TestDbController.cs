using Dapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Data;

namespace UserService.Controllers
{
    [ApiController]
    [Route("testdb")]
    public class TestDbController : ControllerBase
    {
        private readonly UserDb _db;

        public TestDbController(UserDb db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using var conn = _db.CreateConnection();

            var data = await conn.QueryAsync("SELECT TOP 1 * FROM Usuario");

            return Ok(data);
        }
    }
}