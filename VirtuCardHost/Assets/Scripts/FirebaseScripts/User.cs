using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FirebaseScripts
{
    public class User
    {
        private string username;
        private string name;
        private string avatar;
        private string email;
        private string _userId;

        //Should be as strings
        private List<string> friends;

        public User(string username, string email, string userId)
        {
            this.username = username;
            this.email = email;
            this._userId = userId;
            this.friends = new List<string>();
        }

        public User(string json)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            this.username = (string) dict["username"];
            this.email = (string) dict["email"];
            this.name = (string) dict["name"];
            this.avatar = (string) dict["avatar"];
            this._userId = (string) dict["userid"];
            JArray array = (JArray) dict["friends"];
            this.friends = array.ToObject<List<string>>();
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Username
        {
            get => username;
            set => username = value;
        }


        public string Avatar
        {
            get => avatar;
            set => avatar = value;
        }

        public string Email
        {
            get => email;
            set => email = value;
        }

        public string UserId
        {
            get => _userId;
            set => _userId = value;
        }

        public List<string> Friends
        {
            get => friends;
            set => friends = value;
        }

        public override string ToString()
        {
            return $"username: {username}, " +
                   $"userid: {_userId}, " +
                   $"avatar: {avatar}, " +
                   $"name: {name}, " +
                   $"email: {email}, " +
                   $"friends: {friends.ToArray()}";
        }
    }
}