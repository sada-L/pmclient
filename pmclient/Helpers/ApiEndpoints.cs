namespace pmclient.Helpers;

public static class ApiEndpoints
{
   public const string BaseAddress = "https://localhost:44311/";

   public static class Authentication
   {
      private const string Base = "/auth";
      
      public const string Login = Base + "/login";
      public const string Register = Base + "/register";
   }

   public static class Users
   {
      private const string Base = "/users";
      
      public const string GetUserByEmail = Base + "/{email}";
   }
}