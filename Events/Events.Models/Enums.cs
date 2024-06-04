using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models
{
    public class Enums
    {
        public enum EventStatus
        {
            Planning,
            Pending,
            Ongoing,
            Completed,
            Rejected
        }

        public enum CollaboratorStatus
        {
            Registered,
            Approved,
            Completed,
            Rejected
        }
    }
}
