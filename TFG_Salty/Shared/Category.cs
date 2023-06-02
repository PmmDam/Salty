using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFG_Salty.Shared
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

        //Estas flags controlarán el estado de una categoría sin tener que borrarla de la base de datos
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
