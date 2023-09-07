namespace API.Responses
{
    class AuthResponse
    {
        public string AccessToken { get; set; }

        public AuthResponse(string accessToken)
        {
            AccessToken = accessToken;
        }
    }
}