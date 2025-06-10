using System;
using System.ComponentModel.DataAnnotations;

namespace CookHouse.Models
{
    public class BlockedIpLog
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string IP { get; set; }
        public DateTime BlockedAt { get; set; } = DateTime.Now;
        public int RequestCount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}