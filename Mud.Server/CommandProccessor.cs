using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Mud.Data;
using Mud.Data.Character;
using Mud.Data.Commands;

namespace Mud.Server
{
    public static class CommandProccessor
    {
        public static string Secret = "";

        public static void HandleUserPassword(User user, string data)
        {
            var password = HashPassword(data);
            if (password != user.Character.Password)
            {
                user.Connection.SendData("The password you entered does not match.\nEnter your password:");
                return;
            }
            user.Character.CommandsAvailiable.Remove(Commands.EnterPassword);
            LoadWorld(user);
        }

        private static void LoadWorld(User user)
        {
            user.Connection.SendData("You are logged in to the world.");
        }

        public static bool CheckUserLogin(User user, string name)
        {
            if (IsNameGood(name))
            {
                return true;
            }
            user.Connection.SendData($"{name} is not a valid name.\nEnter the name you wish to be know by:");
            return false;
        }

        public static void HandleUserLogin(User user, Character character, string name)
        { 
            if (character == null)
            {
                user.Character.Name = name;
                user.Character.CommandsAvailiable = new List<Commands> { Commands.ConfirmPassword };
                user.Connection.SendData($"{name}. Is that the name by which you wish to be known?");
                return;
            }
            user.Character = character;
            user.Character.CommandsAvailiable.Add(Commands.EnterPassword);
            user.Connection.SendData($"Welcome {user.Character.Name}.\nEnter you password:");
        }

        private static bool IsNameGood(string data)
        {
            if (Regex.IsMatch(data, @"^[a-zA-Z]+$"))
            {
                if (!((IList) Enum.GetNames(typeof(Commands))).Contains(data))
                {
                    return true;
                }
            }
            return false;
        }

        private static string HashPassword(string text)
        {
            using (var hasher = SHA512.Create())
            {
                var textWithSaltBytes = Encoding.UTF8.GetBytes(string.Concat(text, Secret));
                var hashedBytes = hasher.ComputeHash(textWithSaltBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
