using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace DiarySchema.Models
{
    public class AppDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // создаем две роли
            var role1 = new IdentityRole { Name = "Admin" };
            var role2 = new IdentityRole { Name = "Student" };
            var role3 = new IdentityRole { Name = "Lecturer" };

            // добавляем роли в бд
            roleManager.Create(role1);
            roleManager.Create(role2);
            roleManager.Create(role3);

            // создаем пользователей
            var admin = new ApplicationUser { Email = "admin@yandex.ru", UserName = "admin@yandex.ru" };
            string password1 = "F^t1evnm";

            var result1 = userManager.Create(admin, password1);

            // если создание пользователя прошло успешно
            if (result1.Succeeded)
            {
                // добавляем для пользователя роль
                userManager.AddToRole(admin.Id, role1.Name);
            }
            var lucturer = new ApplicationUser { Email = "lecturer@yandex.ru", UserName = "lecturer@yandex.ru" };
            string password2 = "F^t1evnm";

            var result2 = userManager.Create(lucturer, password2);

            // если создание пользователя прошло успешно
            if (result2.Succeeded)
            {
                // добавляем для пользователя роль
                userManager.AddToRole(lucturer.Id, role3.Name);
            }

            base.Seed(context);
        }
    }
}