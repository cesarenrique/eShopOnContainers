using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Model
{
    public class CatalogType
    {
        [Key] // This is an annotation to mark this property as Primary Key of CatalogType
        public int Id { get; set; }

        [Required]
        [StringLength(100)] // This is the maximum length accepted for the name of the Type
        public string Type { get; set; }
    }
}