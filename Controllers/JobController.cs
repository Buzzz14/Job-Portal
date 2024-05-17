using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers
{
    [Authorize]
    public class JobController : Controller
    {
        JobPortalDbContext db_context = new JobPortalDbContext();

        // GET: JobController
        public ActionResult Index(int CurrentPage = 1, int PageSize = 10)
        {
            //int userId = Convert.ToInt32(HttpContext.User.Claims.ElementAt(1).Value);
            var job_list = db_context.Jobs
                //.Where(j => j.UserId == userId)
                .Select(x => new JobViewModel()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Title = x.Title,
                    OrganizationId = x.OrganizationId,
                    OrganizationName = x.Organization.Name,
                    CreatedBy = x.User.Username
                });

            if (job_list != null)
            {
                var paged_list = job_list.OrderBy(x => x.Id)
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize).ToList();
                var new_list = new PaginatedJobViewModel()
                {
                    Count = job_list.Count(),
                    CurrentPage = CurrentPage,
                    PageSize = PageSize,
                    Data = paged_list
                };
                return View(new_list);
            }
            else
            {
                return View(Enumerable.Empty<PaginatedJobViewModel>());
            }
        }






        // GET: JobController/Create
        public ActionResult Create()
        {
            var org_list = db_context.Organizations.Select(x => new OrganizationViewModel()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToList();
            ViewBag.Organizations = org_list;
            return View();
        }

        // POST: JobController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JobViewModel jvm)
        {
            try
            {
                var entity = new Job()
                {
                    Title = jvm.Title,
                    Description = jvm.Description,
                    UserId = Convert.ToInt32(HttpContext.User.Claims.ElementAt(1).Value),
                    OrganizationId = jvm.OrganizationId,
                };   
                db_context.Jobs.Add(entity);
                db_context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }







        // GET: JobsController/Details/5
        public ActionResult Details(int id)
        {
            var jobViewModel = db_context.Jobs
        .Include(x => x.Organization)
        .Select(x => new JobViewModel
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            OrganizationName = x.Organization.Name
        })
        .FirstOrDefault(x => x.Id == id);

            if (jobViewModel == null)
            {
                return NotFound();
            }

            return View(jobViewModel);
        }






        // GET: JobsController/Edit/5
        public ActionResult Edit(int id)
        {
            var jobViewModel = db_context.Jobs
                .Include(x => x.Organization)
                .Select(x => new JobViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    OrganizationId = x.OrganizationId,
                    OrganizationName = x.Organization.Name
                })
                .FirstOrDefault(x => x.Id == id);

            if (jobViewModel == null)
            {
                return NotFound();
            }

            ViewBag.Organizations = new SelectList(db_context.Organizations, "Id", "Name", jobViewModel.OrganizationId);

            return View(jobViewModel);
        }

        // POST: JobsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JobViewModel jobViewModel)
        {

            var job = db_context.Jobs.Find(jobViewModel.Id);
            job.Title = jobViewModel.Title;
            job.Description = jobViewModel.Description;
            job.OrganizationId = jobViewModel.OrganizationId;

            db_context.Entry(job).State = EntityState.Modified;
            db_context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }






        // GET: JobsController/Delete/5
        public ActionResult Delete(int id)
        {
            var jobViewModel = db_context.Jobs
            .Include(x => x.Organization)
            .Select(x => new JobViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                OrganizationName = x.Organization.Name
            })
            .FirstOrDefault(x => x.Id == id);

            if (jobViewModel == null)
            {
                return NotFound();
            }

            return View(jobViewModel);
        }

        // POST: JobsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, JobViewModel jvm)
        {
            try
            {
                var entity = new Job();
                entity.Id = id;
                entity.Title = jvm.Title;
                entity.Description = jvm.Description;
                entity.OrganizationId = jvm.OrganizationId;

                db_context.Jobs.Remove(entity);
                db_context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}