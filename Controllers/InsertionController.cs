using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NicolaPIermatteiWec.Models;
using NicolaPIermatteiWec.Models.InsertModels;
using NicolaPIermatteiWec.Services;

namespace NicolaPIermatteiWec.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsertionController : ControllerBase
    {
        private readonly IDataAccess _da;
        public InsertionController(IDataAccess dataAccess)
        {
            _da = dataAccess;
        }

        [HttpPost("Daily")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertContact(DailyInsert model)
        {
            var response = new ResponseModel();
            try
            {
                if (ModelState.IsValid && model.Latitude != 0 && model.Longitude != 0)
                {
                    response = await _da.DailyInsertion(model);
                    return StatusCode(response.StatusCode, response);
                }
                if (model.Latitude == 0 && model.Longitude == 0)
                {
                    response.StatusCode = 400;
                    response.Message = "Latitude and Longitude are 0, the data in invalid";
                    return StatusCode(response.StatusCode, response);
                }
                response.StatusCode = 400;
                response.Message = "One or more fields are not correct";
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return StatusCode(response.StatusCode, response);
            }
        }

        [HttpPost("Positive")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertPositive(PositiveInsert model)
        {
            var response = new ResponseModel();
            try
            {
                if (ModelState.IsValid)
                {
                    response = await _da.PositiveInsertion(model);
                    return StatusCode(response.StatusCode, response);
                }
                response.StatusCode = 400;
                response.Message = "One or more fields are not correct";
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return StatusCode(response.StatusCode, response);
            }
        }

        [HttpPost("BulkInsert")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> BulkInsert(BulkInsert model)
        {
            var res = new BulkInsertResponse();
            res.DailyInserted = new List<DailyInsert>();
            res.DailyNotInserted = new List<DailyInsert>();
            res.PositiveNotInserted = new List<PositiveInsert>();
            res.PositiveInserted = new List<PositiveInsert>();
            foreach (var item in model.DailyInserts)
            {
                if (ModelState.IsValid)
                {
                    var dailyRes = await _da.DailyInsertion(item);
                    if (dailyRes.StatusCode == 200)
                    {
                        res.DailyInserted.Add(item);
                    }
                    else
                    {
                        res.DailyNotInserted.Add(item);
                    }
                }
                else
                {
                    res.DailyNotInserted.Add(item);
                }
            }
            foreach (var item in model.PositiveInserts)
            {
                if (ModelState.IsValid)
                {
                    var dailyRes = await _da.PositiveInsertion(item);
                    if (dailyRes.StatusCode == 200)
                    {
                        res.PositiveInserted.Add(item);
                    }
                    else
                    {
                        res.PositiveNotInserted.Add(item);
                    }
                }
                else
                {
                    res.PositiveNotInserted.Add(item);
                }
            }
            return Ok(res);
        }
    }
}
