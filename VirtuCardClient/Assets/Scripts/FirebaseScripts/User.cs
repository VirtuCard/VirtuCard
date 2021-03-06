﻿using System.Collections.Generic;
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
        private bool isAnonymous;

        //Should be as strings
        private List<string> friends;

        public User(string username, string email, string userId)
        {
            this.username = username;
            this.email = email;
            this._userId = userId;
            this.avatar = "";
            this.name = "";
            this.friends = new List<string>();
            this.isAnonymous = false;
        }


        public User(string username, string name, string userId, bool isAnonymous)
        {
            this.username = username;
            this.email = "";
            this._userId = userId;
            this.avatar = "";
            this.name = name;
            this.friends = new List<string>();
            this.isAnonymous = isAnonymous;
        }


        public User(string json)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            this.username = (string) dict["Username"];
            this.email = (string) dict["Email"];
            this.name = (string) dict["Name"];
            this.avatar = (string) dict["Avatar"];
            this._userId = (string) dict["UserId"];
            this.isAnonymous = (bool) dict["IsAnonymous"];
            if (dict.ContainsKey("Friends"))
            {
                JArray array = (JArray) dict["Friends"];
                if (array == null)
                {
                    this.friends = new List<string>();
                }
                else
                {
                    this.friends = array.ToObject<List<string>>();
                }
            }
            else
            {
                this.friends = new List<string>();
            }
            
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

        public bool IsAnonymous
        {
            get => isAnonymous;
            set => isAnonymous = value;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}