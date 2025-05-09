namespace pmclient.Helpers;

public static class ApiEndpoints
{
    public const string BaseAddress = "http://localhost:8080/api/v1";

    public static class Authentication
    {
        private const string Base = "/auth";
        private const string TwoFa = "/2fa";

        public const string Login = Base + "/login";
        public const string Register = Base + "/register";
        public const string Token = Base + "/refresh";
        public const string EnableTwoFa = Base + TwoFa + "/enable";
        public const string VerifyTwoFa = Base + TwoFa + "/verify";
        public const string DisableTwoFa = Base + TwoFa + "/disable";
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

        public const string GetGroupsByUser = Base;
        public const string CreateGroup = Base;
        public const string UpdateGroup = Base + "/{id}";
        public const string DeleteGroup = Base + "/{id}";
    }
}