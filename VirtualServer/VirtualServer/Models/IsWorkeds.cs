using System.ComponentModel.DataAnnotations;

namespace VirtualServer.Models
{
    //Модель для статуса True or False, в БД одна запись
    public class IsWorkeds
    {
        [Key]
        public int Id { get; set; } = 1;
        public bool Worked { get; set; } = false;
    }
}