namespace Shared
{
    public class UserContext : IUser
    {
        private Brand _userBrand;
        private GeneralPlatForm _generalPlatForm;
        
        public Brand GetUserBrand() => _userBrand;

        public GeneralPlatForm GetUserPlatForm() => _generalPlatForm;
        
        public void SetBrand(Brand brand) => _userBrand = brand;
        
        public void SetPlatform(GeneralPlatForm generalPlatForm) => _generalPlatForm = generalPlatForm;
    }
}