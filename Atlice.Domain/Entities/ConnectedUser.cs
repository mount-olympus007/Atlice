using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Entities
{
    public class ConnectedUser
    {
        [Key]
        public Guid ConnectionId { get; set; } = Guid.NewGuid();
        public string? UserID { get; set; }
        public string? UserName { get; set; }
        public bool Admin { get; set; }
        public bool Mobile { get; set; }
        public DateTime DateConnected { get; set; }
    }
}
