using System;
using StaffPortal.Models;

namespace StaffPortal.Business
{
    public class Alert
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public ApplicationUser ForUser { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}