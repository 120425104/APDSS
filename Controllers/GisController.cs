using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GIS.Model;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GisController : ControllerBase
    {

        private readonly GisContext gisDb;

        //构造函数把TodoContext 作为参数，Asp.net core 框架可以自动注入TodoContext对象
        public GisController(GisContext context)
        {
            this.gisDb = context;
        }

        // 根据名称查询大气条件
        // GET: api/airCondition?name=xx
        [HttpGet("airCondition")]
        public ActionResult<List<AirCondition>> GetAirCondition(string name)
        {
            var query = gisDb.AirConditions.Where(t => t.Name == name).ToList();
            if (query == null)
            {
                return NotFound();
            }
            return query;
        }

        // GET: api/gis/allAirCondition
        [HttpGet("allAirCondition")]
        public ActionResult<List<AirCondition>> GetAllAirCondition()
        {
            var query=gisDb.AirConditions;
            return query.ToList();
        }

        // GET: api/gis/allPollutionSource 
        [HttpGet("allPollutionSource")]
        public ActionResult<List<PollutionSource>> GetAllPollutionSource()
        {
            var query=gisDb.PollutionSources.ToList();
            return query;
        }

        // GET: api/gis/pollutionSource?name=xx
        [HttpGet("pollutionSource")]
        public ActionResult<List<PollutionSource>> GetPollutionSource(string name)
        {
            var query=gisDb.PollutionSources.Where(t=>t.Name==name).ToList();
            if (query == null)
            {
                return NotFound();
            }
            
            return query;
        }

        // GET: api/gis/allPollutedPoint
        [HttpGet("allPollutedPoint")]
        public ActionResult<List<PollutedPointMap>> GetAllPollutedPoint()
        {
            var query=gisDb.PollutedPoints.ToList();
            return query;
        }

        [HttpGet("risk")]
        public ActionResult<double> GetRiskLevel(double x,double y,AirCondition airCondition)
        {
            double result=gisDb.risk(x,y,airCondition);
            return result;
        }

        // POST: api/gis/addAirCondition
        [HttpPost("addAirCondition")]
        public ActionResult<AirCondition> PostAirCondtion(AirCondition airCondition)
        {
            try
            {
                gisDb.AirConditions.Add(airCondition);
                gisDb.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
            return airCondition;
        }

        // POST: api/gis/addPollutionSource
        [HttpPost("addPollutionSource")]
        public ActionResult<PollutionSource> PostPollutionSource(PollutionSource pollutionSource)
        {
            try
            {
                gisDb.PollutionSources.Add(pollutionSource);
                gisDb.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
            return pollutionSource;
        }

        // POST: api/gis/addPollutedPoints
        [HttpPost("addPollutedPoints")]
        public ActionResult<PollutedPointMap> PostPollutedPoint(AirCondition airCondition)
        {
            gisDb.mock(airCondition);

            return NoContent();
        }

        // PUT: api/gis/putAirCondition/name
        [HttpPut("putAirCondition/{name}")]
        public ActionResult<AirCondition> PutAirCondition(string name,AirCondition airCondition)
        {
            if(name!=airCondition.Name)
            {
                return BadRequest("Id cannot be modified!");
            }

            try
            {
                gisDb.Entry(airCondition).State=EntityState.Modified;
                gisDb.SaveChanges();
            }
            catch (Exception e)
            {
                string error = e.Message;
                if (e.InnerException != null) error = e.InnerException.Message;
                return BadRequest(error);
            }
            return NoContent();
        }

        // PUT: api/gis/putPollutionSource/name
        [HttpPut("putPollutionSource/{name}")]
        public ActionResult<PollutionSource> PutPollutionSource(string name,PollutionSource pollutionSource)
        {
            if(name!=pollutionSource.Name)
            {
                return BadRequest("Id cannot be modified!");
            }

            try
            {
                gisDb.Entry(pollutionSource).State=EntityState.Modified;
                gisDb.SaveChanges();
            }
            catch (Exception e)
            {
                string error = e.Message;
                if (e.InnerException != null) error = e.InnerException.Message;
                return BadRequest(error);
            }
            return NoContent();
        }

        // PUT: api/gis/putPollutedPoint/name
        [HttpPut("putPollutedPoint/{name}")]
        public ActionResult<PollutedPoint> PutPollutedPoint(int x,int y,PollutedPoint pollutedPoint)
        {
            if(x!=pollutedPoint.X||y!=pollutedPoint.Y)
            {
                return BadRequest("Id cannot be modified!");
            }

            try
            {
                gisDb.Entry(pollutedPoint).State=EntityState.Modified;
                gisDb.SaveChanges();
            }
            catch (Exception e)
            {
                string error = e.Message;
                if (e.InnerException != null) error = e.InnerException.Message;
                return BadRequest(error);
            }
            return NoContent();
        }

        // DELETE: api/gis/deleteAirCondition/name
        [HttpDelete("deleteAirCondition/{name}")]
        public ActionResult DeleteAirCondition(string name)
        {
            try
            {
                var query=gisDb.AirConditions.FirstOrDefault(t=>t.Name==name);
                if(query!=null)
                {
                    gisDb.Remove(query);
                    gisDb.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
            return NoContent(); 
        }

        // DELETE: api/gis/deletePollutionSource
        [HttpDelete("deletePollutionSource/{name}")]
        public ActionResult DeletePollutionSource(string name)
        {
            try
            {
                var query=gisDb.PollutionSources.FirstOrDefault(t=>t.Name==name);
                if(query!=null)
                {
                    gisDb.Remove(query);
                    gisDb.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }
            return NoContent(); 
        }

    }
}