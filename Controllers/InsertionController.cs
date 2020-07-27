﻿using System;
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
    }
}
