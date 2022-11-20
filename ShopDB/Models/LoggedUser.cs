using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopDB.Models
{
    public enum AcessLevel : byte
    {
        User,
        Admin
    }

    internal class LoggedUser
    {
        public uint Id { get; set; }
        public AcessLevel AcessLevel { get; set; }
        public string Username { get; set; }
        public string Login { get; set; }
    }
}
