using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualServer.Models
{
    //Модель для данных создания и удаления сервера
    public class VirtualServers
    {
        [Key]
        public int VirtualServerId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime? RemoveDateTime { get; set; }
    }
}