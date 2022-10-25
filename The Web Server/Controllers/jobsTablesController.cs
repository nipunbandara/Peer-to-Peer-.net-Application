using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using The_Web_Server.Models;

namespace The_Web_Server.Controllers
{
    public class jobsTablesController : ApiController
    {
        private jobdbEntities3 db = new jobdbEntities3();

        // GET: api/jobsTables
        public IQueryable<jobsTable> GetjobsTables()
        {
            return db.jobsTables;
        }

        // GET: api/jobsTables/5
        [ResponseType(typeof(jobsTable))]
        public IHttpActionResult GetjobsTable(string id)
        {
            List<jobsTable> jobs = db.jobsTables.Where(u => u.clientId == id).ToList();
            if (jobs == null)
            {
                return NotFound();
            }

            return Ok(jobs);
        }
        // PUT: api/jobsTables/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutjobsTable(int id, jobsTable jobsTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != jobsTable.Id)
            {
                return BadRequest();
            }

            db.Entry(jobsTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!jobsTableExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/jobsTables
        [ResponseType(typeof(jobsTable))]
        public IHttpActionResult PostjobsTable(jobsTable jobsTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.jobsTables.Add(jobsTable);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (jobsTableExists(jobsTable.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = jobsTable.Id }, jobsTable);
        }

        // DELETE: api/jobsTables/5
        [ResponseType(typeof(jobsTable))]
        public IHttpActionResult DeletejobsTable(int id)
        {
            jobsTable jobsTable = db.jobsTables.Find(id);
            if (jobsTable == null)
            {
                return NotFound();
            }

            db.jobsTables.Remove(jobsTable);
            db.SaveChanges();

            return Ok(jobsTable);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool jobsTableExists(int id)
        {
            return db.jobsTables.Count(e => e.Id == id) > 0;
        }
    }
}