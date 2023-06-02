using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFG_Salty.Shared
{
    public class Product
    {
        //Propiedades
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public List<Image> Images { get; set; } = new List<Image>();

        //Aunque parezca redundante, facilita mucho la vida a la hora de trabajar con EntityFramework
        //tener el objeto nullable y la clave foranea del mismo. Explicaré los casos de usos en la memoria

        public Category? Category { get; set; }

        public int CategoryId { get; set; }

        public bool Featured { get; set; } = false;

        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

        //Flags para el panel de administración
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
