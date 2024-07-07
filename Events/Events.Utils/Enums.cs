using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Utils
{
    public class Enums
    {
        public enum EventStatus
        {
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

        public enum AccountStatus
        {
            Active,
            Inactive,
            Banned
        }

        public enum Gender
        {
            Male,
            Female,
            Others
        }

        public enum OrderStatus
        {
            Failed,
            Success
        }

        public enum PaymentStatus {
            Failed, 
            Success
        }
        
        public enum IsCheckin
        {
            No,
            Yes
        }
    }
}
