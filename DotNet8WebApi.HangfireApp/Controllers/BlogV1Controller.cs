using DotNet8WebApi.HangfireApp.Models;
using DotNet8WebApi.HangfireApp.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DotNet8WebApi.HangfireApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogV1Controller : ControllerBase
    {
        private readonly AppDbContext _context;

        public BlogV1Controller(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult BlogList()
        {
            var jobId = BackgroundJob.Enqueue(
                () => Console.WriteLine("BlogList  => {0}", JsonConvert.SerializeObject(_context.blogs.ToList(), Formatting.Indented)));
            var result = string.IsNullOrWhiteSpace(jobId) ? "Fail." : "Success.";
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult BlogById(int id)
        {
            var jobId = BackgroundJob.Enqueue(
              () => Console.WriteLine("BlogById => {0}", JsonConvert.SerializeObject(_context.blogs.FirstOrDefault(x => x.Blog_Id == id), Formatting.Indented)));
            var result = string.IsNullOrWhiteSpace(jobId) ? "Fail." : "Success.";
            return Ok(result);
        }

        [HttpPost]
        public IActionResult BlogCreate(BlogViewModel requestModel)
        {
            var model = new BlogModel
            {
                Blog_Title = requestModel.BlogTitle,
                Blog_Author = requestModel.BlogAuthor,
                Blog_Content = requestModel.BlogContent
            };
            RecurringJob.AddOrUpdate(
               "BlogCreate",
               () => H_BlogCreate(model),
               Cron.Minutely);
            return Ok(model);
        }

        [HttpPost]
        [Route("create-job/{expression}")]
        public IActionResult BlogCreate(string expression, BlogViewModel requestModel)
        {
            var model = new BlogModel
            {
                Blog_Title = requestModel.BlogTitle,
                Blog_Author = requestModel.BlogAuthor,
                Blog_Content = requestModel.BlogContent
            };
            //RecurringJob.AddOrUpdate(
            //   "BlogCreate",
            //   () => H_BlogCreate(model),
            //   Cron.Minutely);

            RecurringJob.AddOrUpdate(
               "BlogCreate",
               () => H_BlogCreate(model),
               expression);
            return Ok(model);
        }

        [HttpPut("{id}")]
        [Route("update-job/{expression}")]
        public IActionResult BlogUpdate(string expression,int id, BlogViewModel requestModel)
        {
            var model = new BlogModel
            {
                Blog_Title = requestModel.BlogTitle,
                Blog_Author = requestModel.BlogAuthor,
                Blog_Content = requestModel.BlogContent
            };
            RecurringJob.AddOrUpdate(
               "BlogUpdate", () => H_BlogUpdate(id, model),
               expression);
            return Ok(model);
        }

        [HttpPut("{id}")]
        public IActionResult BlogUpdate(int id, BlogViewModel requestModel)
        {
            var model = new BlogModel
            {
                Blog_Title = requestModel.BlogTitle,
                Blog_Author = requestModel.BlogAuthor,
                Blog_Content = requestModel.BlogContent
            };
            RecurringJob.AddOrUpdate(
               "BlogUpdate", () => H_BlogUpdate(id, model),
               Cron.Minutely);
            return Ok(model);
        }

        [HttpPatch("{id}")]
        [Route("patch-job/{expression}")]
        public IActionResult BlogPatch(string expression, int id, BlogViewModel requestModel)
        {
            RecurringJob.AddOrUpdate(
               "BlogPatch", () => H_BlogPatch(id, requestModel),
               expression);
            return Ok(requestModel);
        }

        [HttpPatch("{id}")]
        public IActionResult BlogPatch(int id, BlogViewModel requestModel)
        {
            RecurringJob.AddOrUpdate(
               "BlogPatch", () => H_BlogPatch(id, requestModel),
               Cron.Minutely);
            return Ok(requestModel);
        }

        [HttpDelete("{id}")]
        public IActionResult BlogDelete(int id)
        {
            var jobId = BackgroundJob.Enqueue(() => H_BlogDelete(id));
            var result = string.IsNullOrWhiteSpace(jobId) ? "Delete Fail." : "Delete Success.";
            return Ok(result);
        }

        [HttpPost]
        [Route("remove-job/{id}")]
        public IActionResult RemoveJob(string id)
        {
            RecurringJob.RemoveIfExists(id);
            return Ok();
        }

        [NonAction]
        public async Task H_BlogCreate(BlogModel model)
        {
            await _context.blogs.AddAsync(model);
            var result = await _context.SaveChangesAsync();
            Console.WriteLine(result > 0 ? "Saving Successful." : "Saving Fail.");
        }

        [NonAction]
        public async Task H_BlogUpdate(int id, BlogModel model)
        {
            var item = await _context.blogs.FirstOrDefaultAsync(x => x.Blog_Id == id);
            if (item is null)
            {
                Console.WriteLine("Update Data Not Found.");
                return;
            }

            item.Blog_Title = model.Blog_Title;
            item.Blog_Author = model.Blog_Author;
            item.Blog_Content = model.Blog_Content;

            var result = await _context.SaveChangesAsync();
            Console.WriteLine(result > 0 ? "Update Successful." : "Update Fail.");
        }

        [NonAction]
        public async Task<string> H_BlogPatch(int id, BlogViewModel requestModel)
        {
            string message = string.Empty;
            var item = await _context.blogs.FirstOrDefaultAsync(x => x.Blog_Id == id);
            if (item is null)
            {
                message = "Patch Data Not Found.";
                Console.WriteLine(message);
                return message;
            }

            if (!string.IsNullOrEmpty(requestModel.BlogAuthor))
            {
                item.Blog_Author = requestModel.BlogAuthor;
            }
            if (!string.IsNullOrEmpty(requestModel.BlogContent))
            {
                item.Blog_Content = requestModel.BlogContent;
            }
            if (!string.IsNullOrEmpty(requestModel.BlogTitle))
            {
                item.Blog_Title = requestModel.BlogTitle;
            }

            var result = await _context.SaveChangesAsync();
            message = result > 0 ? "Patch Successful." : "Patch Fail.";
            Console.WriteLine(message);
            return message; 
        }

        [NonAction]
        public async Task<string> H_BlogDelete(int id)
        {
            string message = string.Empty;
            var item = await _context.blogs.FirstOrDefaultAsync(x => x.Blog_Id == id);
            if (item is null)
            {
                message = "Data Not Found.";
                Console.WriteLine(message);
                return;
            }

            _context.blogs.Remove(item);
            var result = await _context.SaveChangesAsync();
            message = result > 0 ? "Delete Successful." : "Delete Fail.";
            Console.WriteLine(message);
            return message;
        }
    }
}
