using System;
using System.Collections.Generic;

public class RoomInvite
{
    public string HostName = "";
    // Add all the players you wish to compare against here
    public List<string> TargetUsers = new List<string>();
    public string RoomCode = "";
    public string GameName = "";


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