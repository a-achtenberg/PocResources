namespace Shared;

public interface IUser
{
    public Brand GetUserBrand();
    public GeneralPlatForm GetUserPlatForm();
   void SetBrand(Brand brand);
   void SetPlatform(GeneralPlatForm generalPlatForm);
}