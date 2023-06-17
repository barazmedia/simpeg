using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simpeg.Models
{
    public class UpdateUsersInRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}