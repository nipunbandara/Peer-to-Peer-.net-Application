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
    public class clientsTablesController : ApiController
    {
        private jobdbEntities2 db = new jobdbEntities2();

        // GET: api/clientsTables
        public IQueryable<clientsTable> GetclientsTables()
        {
            return db.clientsTables;
        }

        // GET: api/clientsTables/5
        [ResponseType(typeof(clientsTable))]
        public IHttpActionResult GetclientsTable(int id)
        {
            clientsTable clientsTable = db.clientsTables.Find(id);
            if (clientsTable == null)
            {
                return NotFound();
            }

            return Ok(clientsTable);
        }

        // PUT: api/clientsTables/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutclientsTable(int id, clientsTable clientsTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != clientsTable.Id)
            {
                return BadRequest();
            }

            db.Entry(clientsTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!clientsTableExists(id))
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

        // POST: api/clientsTables
        [ResponseType(typeof(clientsTable))]
        public IHttpActionResult PostclientsTable(clientsTable clientsTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.clientsTables.Add(clientsTable);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (clientsTableExists(clientsTable.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = clientsTable.Id }, clientsTable);
        }

        // DELETE: api/clientsTables/5
        [ResponseType(typeof(clientsTable))]
        public IHttpActionResult DeleteclientsTable(int id)
        {
            clientsTable clientsTable = db.clientsTables.Find(id);
            if (clientsTable == null)
            {
                return NotFound();
            }

            db.clientsTables.Remove(clientsTable);
            db.SaveChanges();

            return Ok(clientsTable);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool clientsTableExists(int id)
        {
            return db.clientsTables.Count(e => e.Id == id) > 0;
        }
    }
}