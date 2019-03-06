using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiarySchema.ViewModel
{
    public class StudentRegisterViewModel
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string CardNumber { get; set; }
        public int GroupId { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        
    }
}