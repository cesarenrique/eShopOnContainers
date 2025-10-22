using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalog.API.Data;
using Catalog.API.Model;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogTypesController : ControllerBase
    {
        private readonly CatalogContext _context;

        public CatalogTypesController(CatalogContext context)
        {
            _context = context;
        }

        // GET: api/CatalogTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CatalogType>>> GetCatalogTypes()
        {
            return await _context.CatalogTypes.ToListAsync();
        }

        // GET: api/CatalogTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CatalogType>> GetCatalogType(int id)
        {
            var catalogType = await _context.CatalogTypes.FindAsync(id);

            if (catalogType == null)
            {
                return NotFound();
            }

            return catalogType;
        }

        // PUT: api/CatalogTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCatalogType(int id, CatalogType catalogType)
        {
            if (id != catalogType.Id)
            {
                return BadRequest();
            }

            _context.Entry(catalogType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatalogTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CatalogTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CatalogType>> PostCatalogType(CatalogType catalogType)
        {
            _context.CatalogTypes.Add(catalogType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCatalogType", new { id = catalogType.Id }, catalogType);
        }

        // DELETE: api/CatalogTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatalogType(int id)
        {
            var catalogType = await _context.CatalogTypes.FindAsync(id);
            if (catalogType == null)
            {
                return NotFound();
            }

            _context.CatalogTypes.Remove(catalogType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CatalogTypeExists(int id)
        {
            return _context.CatalogTypes.Any(e => e.Id == id);
        }
    }
}
