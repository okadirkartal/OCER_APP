using System.ComponentModel.DataAnnotations;

namespace Application.Core.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }

        public string UserName { get; set; }
        
        public string Password { get; set; }
    }
}