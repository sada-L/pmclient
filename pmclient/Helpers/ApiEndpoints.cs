namespace pmclient.Helpers;

public static class ApiEndpoints
{
   public const string BaseAddress = "http://localhost:8080/api/v1";

   public static class Authentication
   {
      private const string Base = "/users";
      
      public const string Login = Base + "/login";
      public const string Register = Base + "/create";
   }

   public static class Users
   {
      private const string Base = "/users";
      
      public const string GetCurrentUser = Base + "/current";
   }
   
   public static class Cards
   {
      private const string Base = "/cards";
      
      public const string GetCardsByUser = Base + "/current";
   }
}