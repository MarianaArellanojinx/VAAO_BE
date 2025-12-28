using System.ComponentModel.DataAnnotations.Schema;

namespace VAAO_BE.Entities
{



    [Table("Users")]
    public class Users
    {
        public int IdUser { get; set; }

        public string UserName { get; set; } = null!;

        public string UserPassword { get; set; } = null!;

        public bool IsActive { get; set; }

        public int Rol { get; set; }
    }
}
