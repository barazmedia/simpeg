using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simpeg.Models
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Users { get; set; }
    }
}