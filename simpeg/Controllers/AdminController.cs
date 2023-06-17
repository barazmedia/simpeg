using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using simpeg.Areas.Identity.Data;
using simpeg.Models;

namespace simpeg.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUsers> _userManager;

        public AdminController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUsers> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ListRole()
        {
            var role = _roleManager.Roles;
            return View(role);
        }
        public IActionResult CreateRole() 
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = model.RoleName
                };
                IdentityResult result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    ViewData["pesan"] = $"<span class='alert alert-success'>Role berhasil dibuat </span>";
                    return View();
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewData["PesanError"]=$"<span class='alert alert-danger'>Role dengan id {id} tidak ditemukan</span>";
                return View("NotFound");
            }
            else
                {
                    var model = new EditRoleViewModel{
                    Id = role.Id,
                    Name = role.Name
                };

                //menampilkan user dalam role 
                foreach(var user in _userManager.Users)
                {
                    if(await _userManager.IsInRoleAsync(user,role.Name))
                    {
                        model.Users.Add(user.FirstName+" "+user.LastName);
                    }
                }
                return View(model);  

           }                  
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if(role == null)
            {
                ViewData["PesanError"]=$"<span class='alert alert-danger'>Role dengan id {model.Id} tidak ditemukan</span>";
                return View("NotFound");
            }
            else
            {
                role.Name = model.Name;
                var result = await _roleManager.UpdateAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListRole");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description);
                    }
                return View(model);  
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                ViewData["PesanError"]=$"<span class='alert alert-danger'>Role dengan id {id} tidak ditemukan</span>";
                return View("NotFound");
            }
            else
            {
                var result = await _roleManager.DeleteAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListRole");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description);
                    }
                    return View("ListRole");
                }
            }
        }
        public async Task<IActionResult> UpdateUsersInRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            ViewData["roleid"] = id;
            if(role == null)
            {
                ViewData["PesanError"]=$"<span class='alert alert-danger'>Role dengan id {id} tidak ditemukan</span>";
                return View("NotFound");
            }
            else
            {
                var model = new List<UpdateUsersInRoleViewModel>();
                foreach(var user in _userManager.Users)
                {
                    var updateUsersInRoleViewModel = new UpdateUsersInRoleViewModel{
                        UserId = user.Id,
                        UserName = user.FirstName+" "+user.LastName
                    };
                     //cek apakah user ada dalam role
                    if( await _userManager.IsInRoleAsync(user,role.Name))
                    {
                        updateUsersInRoleViewModel.IsSelected = true;
                    }
                    else
                    {
                        updateUsersInRoleViewModel.IsSelected = false;
                    }
                    model.Add(updateUsersInRoleViewModel);
                }
            return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUsersInRole(List<UpdateUsersInRoleViewModel> model, string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            for (int i = 0; i<model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;

                //jika selected terselect dan user tidak ada dalam role berarti di tambah......
                if(model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user,role.Name);
                }
                else if(!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                //lakukan pengecekan 
                if(result.Succeeded)
                {
                    if(i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole",new {Id = id});
                    }
                }
            }
            return RedirectToAction("EditRole", new {Id = id});

        }
    }
}
