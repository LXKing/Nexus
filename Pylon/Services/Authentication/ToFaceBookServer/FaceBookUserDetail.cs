﻿using Newtonsoft.Json;

namespace Aiursoft.Pylon.Services.Authentication.ToFaceBookServer
{
    public class FaceBookUserDetail : IUserDetail
    {
        [JsonProperty("sub")]
        public string Id { get; set; }

        [JsonProperty("picture")]
        public string AvatarUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("bio")]
        public string Bio { get; set; } = "";
    }
}
