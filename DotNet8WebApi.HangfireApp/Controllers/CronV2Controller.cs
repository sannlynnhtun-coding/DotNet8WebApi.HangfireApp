using DotNet8WebApi.HangfireApp.Models;
using DotNet8WebApi.HangfireApp.Services;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace DotNet8WebApi.HangfireApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CronV2Controller : ControllerBase
    {
        private readonly AppDbContext _context;

        public CronV2Controller(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Reschedule")]
        public IActionResult Reschedule(CronRequestModel requestModel)
        {
            string expression = requestModel.CronExpression;
            EnumBlogType enumBlogType = Enum.Parse<EnumBlogType>(requestModel.ServiceName, true);
            switch (enumBlogType)
            {
                case EnumBlogType.List:
                    AddOrUpdate("List", () => H_BlogList(), expression);
                    break;
                case EnumBlogType.Create:
                    AddOrUpdate("Create", () => H_BlogList(), expression);
                    break;
                case EnumBlogType.Update:
                    AddOrUpdate("Update", () => H_BlogList(), expression);
                    break;
                case EnumBlogType.Delete:
                    AddOrUpdate("Delete", () => H_BlogList(), expression);
                    break;
                case EnumBlogType.None:
                default:
                    return StatusCode(500, new { Message = "Invalid Service Name." });
            }
            return Ok();
        }

        [HttpPost]
        [Route("StopService")]
        public IActionResult StopService()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }
            return Ok();
        }

        [HttpPost]
        [Route("GetService")]
        public IActionResult GetService()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                return Ok(connection.GetRecurringJobs().Select(x=> new
                {
                    Id = x.Id,
                    Name = x.Job.Method.Name,
                    LastExecution = x.LastExecution
                }).ToList());
            }
        }

        [NonAction]
        public void AddOrUpdate(string recurringJobId, Expression<Func<Task>> methodCall, string cronExpression)
        {
            RecurringJob.AddOrUpdate(recurringJobId, methodCall, cronExpression);
        }

        [NonAction]
        public async Task<List<BlogModel>> H_BlogList()
        {
            return await _context.blogs.AsNoTracking().ToListAsync();
        }
    }
}
