using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualServer.Models
{
    //Модель для хранения временных интервалов, когда сервера работают
    public class Counters
    {
        [Key]
        public int Id { get; set; }
        public DateTime start { get; set; }
        public DateTime? end { get; set; }
    }
}