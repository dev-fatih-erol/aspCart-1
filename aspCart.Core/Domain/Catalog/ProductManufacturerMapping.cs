using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspCart.Core.Domain.Catalog
{
    public class ProductManufacturerMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ManufacturerId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
    }
}