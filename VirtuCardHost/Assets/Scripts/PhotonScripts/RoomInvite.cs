using System;
using System.Collections.Generic;

namespace PhotonScripts
{
    public class RoomInvite
    {
        public string HostName = "";
        // Add all the players you wish to compare against here
        public List<string> TargetUsers = new List<string>();
        public string RoomCode = "";
        public string GameName = "";

        public static RoomInvite CreateInvite(List<string> users)
        {
            RoomInvite invite = new RoomInvite();
            invite.TargetUsers = users;
            invite.HostName = HostData.UserProfile.Username;
            invite.GameName = HostData.GetGame().GetGameName();
            invite.RoomCode = HostData.GetJoinCode();
            return invite;
        }

        // Room invites are going to be Dictionary<string, object> objects
        // This is a supported custom type, which makes it easier cause we
        // avoid doing serialization stuff
        public Dictionary<string, object> GetDictInvite()
        {
            Dictionary<string, object> request = new Dictionary<string, object>();
            request.Add("HostName", HostName);
            request.Add("TargetUsers", TargetUsers.ToArray());
            request.Add("RoomCode", RoomCode);
            request.Add("GameName", GameName);
            return request;
        }

        public static RoomInvite InviteFromDict(object o)
        {
            if (o.GetType() == typeof(Dictionary<string, object>))
            {
                Dictionary<string, object> dict = o as Dictionary<string, object>;
                RoomInvite invite = new RoomInvite();
                if (dict != null)
                {
                    invite.HostName = dict["HostName"] as string;
                    invite.RoomCode = dict["RoomCode"] as string;
                    invite.TargetUsers = new List<string>(dict["TargetUsers"] as string[] ?? Array.Empty<string>());
                    invite.GameName = dict["GameName"] as string;
                }
                else
                {
                    return null;
                }

                return invite;
            }

            return null;
        }
    }
}