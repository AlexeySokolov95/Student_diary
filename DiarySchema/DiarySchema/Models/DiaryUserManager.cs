using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace DiarySchema.Models
{
    public class DiaryUserManager : UserManager<DiaryUser>
    {
        public DiaryUserManager(IUserStore<DiaryUser> store) 
            : base(store) 
    {
        }
        public static DiaryUserManager Create(IdentityFactoryOptions<DiaryUserManager> options,
                                                IOwinContext context)
        {
            var db = context.Get<DiaryContext>();
            DiaryUserManager manager = new DiaryUserManager(new UserStore<DiaryUser>(db));
            return manager;
        }
    }
}