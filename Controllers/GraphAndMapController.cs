using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NicolaPIermatteiWec.Services;

namespace NicolaPIermatteiWec.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphAndMapController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;
        public GraphAndMapController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [HttpGet("Map")]
        public async Task<IActionResult> GetMap()
        {
            try
            {
                return Ok(await _dataAccess.RetreiveForMap());
            }
            catch (Exception)
            {
                return StatusCode(500, "There was an error on retreiving the data");
            }
        }

        [HttpGet("Graph")]
        public async Task<IActionResult> GetData()
        {
            try
            {
                return Ok(await _dataAccess.GetGraphData());
            }
            catch (Exception)
            {
                return StatusCode(500, "There was an error on retreiving the data");
            }
        }
    }
}
