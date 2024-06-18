using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs.Request
{
    public class EventActionRequestDTO
    {
        public string Action { get; set; }
        public string FieldName { get; set; }
        public string SearchName { get; set; } 
        public string Orderby { get; set; }    
    }
}
