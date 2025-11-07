using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.DTOs.Client
{
    public class ClientFilterDTO
    {
        public string? Search { get; set; }
        public string? Email { get; set; }
        public string? LastName { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
