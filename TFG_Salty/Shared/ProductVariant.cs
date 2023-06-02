using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TFG_Salty.Shared
{  /// <summary>
   /// Con esta clase gestionaremos las diferentes variaciones que puede haber de un mismo producto. Por ejemplo, un libro que está en formato fisico o digital.
   /// La clave primaria de esta clase estará compuesta por el Id del product y el Id del tipo de producto
   /// </summary>
    public class ProductVariant
    {
        [JsonIgnore]
        public Product? Product { get; set; }
        public int ProductId { get; set; }
        public ProductType? ProductType { get; set; }
        public int ProductTypeId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalPrice { get; set; }

        //Flags para el panel de administracion
        public bool Visible { get; set; } = true;
        public bool Deleted { get; set; } = false;

        //Estas flags no son necesarias en la base de datos para el panel de administración.
        //es más fácil tenerlo aquí centralizado
        [NotMapped]
        public bool Editing { get; set; } = false;
        [NotMapped]
        public bool IsNew { get; set; } = false;

    }
}
