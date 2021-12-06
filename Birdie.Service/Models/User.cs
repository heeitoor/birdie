namespace Birdie.Service.Models
{
    public class UserSignUpModel : UserLoginModel
    {
        public virtual string PasswordConfirm { get; set; }
    }

    public class UserLoginModel
    {
        public virtual string UserName { get; set; }

        public virtual string Password { get; set; }
    }
}
