namespace pmclient.Helpers;

public static class ApiEndpoints
{
    public const string BaseAddress = "http://localhost:8080/api/v1";

    public static class Authentication
    {
        private const string Base = "/auth";

        public const string Login = Base + "/login";
        public const string Register = Base + "/register";
        public const string Token = Base + "/refresh";
    }

    public static class Users
    {
        private const string Base = "/users";

        public const string GetCurrentUser = Base + "/me";
        public const string UpdateCurrentUser = Base + "/me";
        public const string DeleteCurrentUser = Base + "/me";
    }

    public static class Cards
    {
        private const string Base = "/cards";

        public const string GetCardsByUser = Base;
        public const string CreateCard = Base;
        public const string UpdateCard = Base + "/{id}";
        public const string DeleteCard = Base + "/{id}";
    }

    public static class Groups
    {
        private const string Base = "/groups";

        public const string GetGroupsByUser = Base + "/current";
        public const string CreateGroup = Base + "/create";
        public const string UpdateGroup = Base + "/update";
    }
}