using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mud.Data;
using Mud.Data.Commands;
using Mud.Data.Configuration;
using Mud.Data.Enums;
using Mud.DataAccess.Repositories;

namespace Mud.Server
{
    public class CharacterCreationProccessor
    {
        public RaceRepo RaceRepository;

        public CharacterCreationProccessor(AppSettings appSettings)
        {
            RaceRepository = new RaceRepo(appSettings);
        }

        public void ProcessCharacter(User user, List<string> inputData)
        {
            var baseCommand = inputData[0];
            switch (user.Character.CurrentState)
            {
                case CharacterState.AddName:
                    var name = char.ToUpper(baseCommand[0]) + baseCommand.Substring(1).ToLower();

                    if (!CommandProccessor.CheckUserLogin(user, name))
                    {
                        return;
                    }

                    user.Connection.SendData($"{name}. Is that the name by which you wish to be known?");
                    user.Character.Name = name;
                    break;
                case CharacterState.ConfirmName:
                    if (StringConstants.AffirmativeReply.Contains(baseCommand.ToLower()))
                    {
                        user.Connection.SendData("Ok, what name do you wish to be known by?");
                        user.Character.CurrentState = CharacterState.AddName;
                        return;
                    }
                    user.Character.CurrentState = CharacterState.AddPassword;
                    user.Connection.SendData($"Welcome {user.Character.Name}. Please enter a password: ");
                    break;
                case CharacterState.AddPassword:
                    var password = CommandProccessor.HashPassword(baseCommand);
                    user.Character.Password = password;
                    user.Character.CurrentState = CharacterState.ConfirmPassword;
                    user.Connection.SendData("Please confirm your password: ");
                    break;
                case CharacterState.ConfirmPassword:
                    var confirmPassword = CommandProccessor.HashPassword(baseCommand);
                    if (confirmPassword == user.Character.Password)
                    {
                        user.Character.CurrentState = CharacterState.AddGender;
                        user.Connection.SendData("Is this avatar (m)ale or (f)emale?");
                        return;
                    }
                    user.Character.CurrentState = CharacterState.AddPassword;
                    user.Connection.SendData("The passwords did not match! Please enter a password: ");
                    break;
                case CharacterState.AddGender:
                    var genderReply = baseCommand.ToLower();
                    user.Character.Gender = (genderReply == "male" || genderReply == "m")
                        ? Gender.Male
                        : Gender.Female;
                    user.Character.CurrentState = CharacterState.AddRace;
                    var races = RaceRepository.GetAllRaces();
                    user.Connection.SendData("Please choose your race: ");
                    races.ForEach(r => user.Connection.SendData($"\t{r.Name}"));
                    user.Connection.SendData("(You can use 'help raceName' to get a description of particular race)");
                    break;
                case CharacterState.AddRace:
                    races = RaceRepository.GetAllRaces();
                    if (baseCommand.ToLower() == "help")
                    {
                        if (inputData.Count == 1)
                        {
                            user.Connection.SendData("You are in the middle of selecting your avatar's race. Please choose a race: ");
                            races.ForEach(r => user.Connection.SendData(r.Name));
                            user.Connection.SendData("(You can use 'help raceName' to get a description of particular race)");
                            return;
                        }
                        if (races.Any(r =>
                            string.Equals(inputData[1], r.Name, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            var raceDescription = races.First(r =>
                                    string.Equals(inputData[1], r.Name, StringComparison.CurrentCultureIgnoreCase))
                                    .Description;
                            user.Connection.SendData(raceDescription);

                            user.Connection.SendData("Please choose your race: ");
                            races.ForEach(r => user.Connection.SendData($"\t{r.Name}"));
                            user.Connection.SendData("(You can use 'help raceName' to get a description of particular race)");
                            return;
                        }
                    }
                    if (races.Any(r =>
                        string.Equals(baseCommand, r.Name, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        user.Character.Race = baseCommand;
                        user.Character.CurrentState = CharacterState.ConfirmRace;
                        user.Connection.SendData($"{user.Character.Race}. Is that the correct race?");
                        return;
                    }
                    user.Connection.SendData($"{inputData[0]} is not a valid race! Please choose your race: ");
                    races.ForEach(r => user.Connection.SendData($"\t{r.Name}"));
                    user.Connection.SendData("(You can use 'help raceName' to get a description of particular race)");
                    break;
                case CharacterState.ConfirmRace:
                    races = RaceRepository.GetAllRaces();
                    if (StringConstants.AffirmativeReply.Contains(baseCommand.ToLower()))
                    {
                        user.Connection.SendData("Ok then.\nPlease choose your race: ");
                        races.ForEach(r => user.Connection.SendData($"\t{r.Name}"));
                        user.Connection.SendData("(You can use 'help raceName' to get a description of particular race)");
                        user.Character.CurrentState = CharacterState.AddRace;
                        return;
                    }
                    user.Character.CurrentState = CharacterState.AddHeight;
                    var race = races.First(r => r.Name == user.Character.Race);
                    var baseHeight = race.MinHeight;
                    var heightInc = (race.MaxHeight - baseHeight) / 6;
                    user.Connection.SendData($"{user.Character.Gender}s of the {race.Name} are between {baseHeight} and {baseHeight + heightInc * 6} ");
                    user.Connection.SendData($"1.)\tVery Short ({baseHeight})");
                    user.Connection.SendData($"2.)\tShort ({baseHeight + heightInc})");
                    user.Connection.SendData($"3.)\tAverage Short ({baseHeight + heightInc * 2})");
                    user.Connection.SendData($"4.)\tAverage ({baseHeight + heightInc * 3})");
                    user.Connection.SendData($"5.)\tAverage Tall ({baseHeight + heightInc * 4})");
                    user.Connection.SendData($"6.)\tTall ({baseHeight + heightInc * 5})");
                    user.Connection.SendData($"7.)\tVery Tall ({baseHeight + heightInc * 6})");
                    user.Connection.SendData("Choose your height:");
                    break;
                case CharacterState.AddHeight:
                    int.TryParse(baseCommand, out var heightChoice);
                    races = RaceRepository.GetAllRaces();
                    race = races.First(r => r.Name == user.Character.Race);
                    if (heightChoice > 7 || heightChoice < 1)
                    {
                        baseHeight = race.MinHeight;
                        heightInc = (race.MaxHeight - baseHeight) / 6;
                        user.Connection.SendData($"\"{baseCommand}\" is not a valid choice!");
                        user.Connection.SendData($"{user.Character.Gender}s of the {race.Name} are between {baseHeight} and {baseHeight + heightInc * 6}.");
                        user.Connection.SendData($"1.)\tVery Short ({baseHeight})");
                        user.Connection.SendData($"2.)\tShort ({baseHeight + heightInc})");
                        user.Connection.SendData($"3.)\tAverage Short ({baseHeight + heightInc * 2})");
                        user.Connection.SendData($"4.)\tAverage ({baseHeight + heightInc * 3})");
                        user.Connection.SendData($"5.)\tAverage Tall ({baseHeight + heightInc * 4})");
                        user.Connection.SendData($"6.)\tTall ({baseHeight + heightInc * 5})");
                        user.Connection.SendData($"7.)\tVery Tall ({baseHeight + heightInc * 6})");
                        user.Connection.SendData("Choose your height:");
                        return;
                    }
                    user.Character.CurrentState = CharacterState.AddWeight;
                    var baseWeight = race.MinWeight;
                    var weightInc = (race.MaxWeight - baseWeight) / 6;
                    user.Connection.SendData($"{user.Character.Gender}s of the {race.Name} are between {baseWeight} lbs and {baseWeight + weightInc * 6} lbs.");
                    user.Connection.SendData($"1.)\t {baseWeight} lbs");
                    user.Connection.SendData($"2.)\t {baseWeight + weightInc} lbs");
                    user.Connection.SendData($"3.)\t {baseWeight + weightInc * 2} lbs");
                    user.Connection.SendData($"4.)\t {baseWeight + weightInc * 3} lbs");
                    user.Connection.SendData($"5.)\t {baseWeight + weightInc * 4} lbs");
                    user.Connection.SendData($"6.)\t {baseWeight + weightInc * 5} lbs");
                    user.Connection.SendData($"7.)\t {baseWeight + weightInc * 6} lbs");
                    user.Connection.SendData("Choose your weight:");
                    break;
                case CharacterState.AddWeight:
                    int.TryParse(baseCommand, out var weightChoice);
                    races = RaceRepository.GetAllRaces();
                    race = races.First(r => r.Name == user.Character.Race);
                    if (weightChoice > 7 || weightChoice < 1)
                    {
                        baseWeight = race.MinWeight;
                        weightInc = (race.MaxWeight - baseWeight) / 6;
                        user.Connection.SendData($"{user.Character.Gender}s of the {race.Name} are between {baseWeight} lbs and {baseWeight + weightInc * 6} lbs.");
                        user.Connection.SendData($"1.)\t {baseWeight} lbs");
                        user.Connection.SendData($"2.)\t {baseWeight + weightInc} lbs");
                        user.Connection.SendData($"3.)\t {baseWeight + weightInc * 2} lbs");
                        user.Connection.SendData($"4.)\t {baseWeight + weightInc * 3} lbs");
                        user.Connection.SendData($"5.)\t {baseWeight + weightInc * 4} lbs");
                        user.Connection.SendData($"6.)\t {baseWeight + weightInc * 5} lbs");
                        user.Connection.SendData($"7.)\t {baseWeight + weightInc * 6} lbs");
                        user.Connection.SendData("Choose your weight:");
                        return;
                    }
                    user.Character.CurrentState = CharacterState.ChooseStats;

                    break;
                case CharacterState.ChooseStats:
                    break;
                case CharacterState.AdjustStats:
                    break;
                case CharacterState.ConfirmStats:
                    break;
                case CharacterState.ConfirmCharacterCreation:
                    break;
                default:
                    user.Character.CommandsAvailiable.Remove(Commands.CreateCharacter);
                    break;
            }
        }
    }
}
