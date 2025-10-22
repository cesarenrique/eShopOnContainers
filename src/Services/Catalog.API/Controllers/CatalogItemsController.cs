using Catalog.API.Data;
using Catalog.API.Model;
using Catalog.API.Model.CatalogItemDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogItemsController : ControllerBase
    {
        private readonly CatalogContext _context;

        public CatalogItemsController(CatalogContext context)
        {
            _context = context;
        }


        // GET: api/CatalogItems
        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<CatalogItem>>> GetCatalogItems()
        {
            return await _context.CatalogItems.ToListAsync();
        }*/

        /*
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsDTO<CatalogItemDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetCatalogItems([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var itemsOnPage = await _context.CatalogItems
                // Join con las tablas CatalogBrand y CatalogType
                .Include(item => item.CatalogBrand)
                .Include(item => item.CatalogType)
                .Select(item => new CatalogItemDTO()
                {
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    CatalogType = item.CatalogType.Type,
                    CatalogBrand = item.CatalogBrand.Brand,
                    AvailableStock = item.AvailableStock,
                    RestockThreshold = item.RestockThreshold,
                    MaxStockThreshold = item.MaxStockThreshold,
                    OnReorder = item.OnReorder
                })
                .OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            var totalItems = await _context.CatalogItems.LongCountAsync();

            var model = new PaginatedItemsDTO<CatalogItemDTO>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }*/
        // GET: api/Items
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsDTO<CatalogItemDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetCatalogItems([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var itemsOnPage = await _context.CatalogItems
                // Join table CatalogBrand and CatalogType
                .Include(item => item.CatalogBrand)
                .Include(item => item.CatalogType)
                .Select(item => new CatalogItemDTO()
                {
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    CatalogType = item.CatalogType.Type,
                    CatalogBrand = item.CatalogBrand.Brand,
                    AvailableStock = item.AvailableStock,
                    RestockThreshold = item.RestockThreshold,
                    MaxStockThreshold = item.MaxStockThreshold,
                    OnReorder = item.OnReorder
                })
                .OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            var totalItems = await _context.CatalogItems.LongCountAsync();

            var model = new PaginatedItemsDTO<CatalogItemDTO>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        // GET: api/CatalogItems/5
        /*[HttpGet("{id}")]
        public async Task<ActionResult<CatalogItem>> GetCatalogItem(int id)
        {
            var catalogItem = await _context.CatalogItems.FindAsync(id);

            if (catalogItem == null)
            {
                return NotFound();
            }

            return catalogItem;
        }*/
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CatalogItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CatalogItem>> GetCatalogItem(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var catalogItem = await _context.CatalogItems.FindAsync(id);

            if (catalogItem == null)
            {
                return NotFound();
            }

            return catalogItem;
        }

        // PUT: api/CatalogItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*[HttpPut("{id}")]
        public async Task<IActionResult> PutCatalogItem(int id, CatalogItem catalogItem)
        {
            if (id != catalogItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(catalogItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatalogItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }*/
        [HttpPut]
        [ProducesResponseType(typeof(CatalogItemDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<CatalogItemDTO>> PutCatalogItem(CatalogItemDTO itemDTO)
        {
            bool priceChanged = false;

            if (itemDTO == null)
                return BadRequest();

            var item = await _context.CatalogItems
                .Include(i => i.CatalogBrand)
                .Include(i => i.CatalogType)
                .FirstOrDefaultAsync(i => i.Name == itemDTO.Name);

            if (item == null)
                return NotFound($"Item {itemDTO.Name} does not exist in the catalog");

            if (item.Price != itemDTO.Price)
                priceChanged = true;

            // Actualización del tipo de catálogo
            if (item.CatalogType.Type != itemDTO.CatalogType)
            {
                var type = await _context.CatalogTypes
                    .FirstOrDefaultAsync(t => t.Type == itemDTO.CatalogType);

                if (type == null)
                    item.CatalogType = new CatalogType { Type = itemDTO.CatalogType };
                else
                    item.CatalogType = type;
            }

            // Actualización de la marca de catálogo
            if (item.CatalogBrand.Brand != itemDTO.CatalogBrand)
            {
                var brand = await _context.CatalogBrands
                    .FirstOrDefaultAsync(b => b.Brand == itemDTO.CatalogBrand);

                if (brand == null)
                    item.CatalogBrand = new CatalogBrand { Brand = itemDTO.CatalogBrand };
                else
                    item.CatalogBrand = brand;
            }

            // Se recomienda usar AutoMapper para evitar esta asignación manual
            item.MaxStockThreshold = itemDTO.MaxStockThreshold;
            item.OnReorder = itemDTO.OnReorder;
            item.Price = itemDTO.Price;
            item.Description = itemDTO.Description;
            item.RestockThreshold = itemDTO.RestockThreshold;
            item.AvailableStock = itemDTO.AvailableStock;

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return itemDTO;
        }

        // POST: api/CatalogItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*[HttpPost]
        public async Task<ActionResult<CatalogItem>> PostCatalogItem(CatalogItem catalogItem)
        {
            _context.CatalogItems.Add(catalogItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCatalogItem", new { id = catalogItem.Id }, catalogItem);
        }*/
        // POST: api/Items
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CatalogItemDTO>> PostCatalogItem(CatalogItemDTO catalogItemDTO)
        {
            // Verifica si el producto ya existe por nombre (debe ser único)
            CatalogItem catalogItem = await _context.CatalogItems
                .FirstOrDefaultAsync(ci => ci.Name == catalogItemDTO.Name);

            if (catalogItem != null)
                return BadRequest("Product Name must be unique");

            // Crea una nueva instancia de CatalogItem
            catalogItem = new CatalogItem
            {
                Name = catalogItemDTO.Name,
                Description = catalogItemDTO.Description,
                Price = catalogItemDTO.Price,
                AvailableStock = catalogItemDTO.AvailableStock,
                RestockThreshold = catalogItemDTO.RestockThreshold,
                MaxStockThreshold = catalogItemDTO.MaxStockThreshold,
                OnReorder = catalogItemDTO.OnReorder
            };

            // Verifica o crea el tipo de catálogo
            CatalogType catalogType = await _context.CatalogTypes
                .FirstOrDefaultAsync(ct => ct.Type == catalogItemDTO.CatalogType);

            if (catalogType == null)
                catalogType = new CatalogType { Type = catalogItemDTO.CatalogType };

            // Verifica o crea la marca de catálogo
            CatalogBrand catalogBrand = await _context.CatalogBrands
                .FirstOrDefaultAsync(cb => cb.Brand == catalogItemDTO.CatalogBrand);

            if (catalogBrand == null)
                catalogBrand = new CatalogBrand { Brand = catalogItemDTO.CatalogBrand };

            // Asigna las relaciones
            catalogItem.CatalogType = catalogType;
            catalogItem.CatalogBrand = catalogBrand;

            // Guarda en la base de datos
            _context.CatalogItems.Add(catalogItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCatalogItem", new { id = catalogItem.Id }, catalogItemDTO);
        }

        // DELETE: api/CatalogItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatalogItem(int id)
        {
            var catalogItem = await _context.CatalogItems.FindAsync(id);
            if (catalogItem == null)
            {
                return NotFound();
            }

            _context.CatalogItems.Remove(catalogItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CatalogItemExists(int id)
        {
            return _context.CatalogItems.Any(e => e.Id == id);
        }
    }
}
