//Entity qui contient les informations de l'état de toutes les voitures et de la balle
using System;
using System.Collections.Generic;

namespace RedUtils
{
    public class StateEntity
    {
        public Car[] blueTeam { get; set; }
        public Car[] redTeam { get; set; }
        public Ball Ball { get; set; }

        public StateEntity(string replayFolder, integer frame)
        {
            JSON jsonFrame = this.ParseJSON(replayFolder, frame);
            this.verifyFrame(jsonFrame);
            this.BuildTeam(jsonFrame);      
        }
        private JSON ParseJSON(string replayFolder, integer frame){
            string frameFile = replayFolder + "/parsed_data_f" + frame + ".json";
            try
            {
                string json = System.IO.File.ReadAllText(frameFile);
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("File not found: " + frameFile);
            }
            return JSON.Parse(json);
        }
        private bool verifyFrame(JSON jsonFrame){
            // Vérification du nombre de joueurs
            int nbPlayers = jsonFrame["players"].Count;
            if (!VerifyPlayerCount(nbPlayers)) return false;

            // Vérification de la balle
            if (!VerifyBall(jsonFrame["ball"])) return false;

            // Vérification de chaque joueur
            foreach (JSON player in jsonFrame["players"])
            {
                if (!VerifyPlayer(player)) return false;
            }

            return true;
        }

        private void BuildTeam(JSON jsonFrame){

            foreach (JSON player in jsonFrame["players"])
            {
                Car car = new Car();
                car.Location = new Vec3(player["position"]);
                car.Velocity = new Vec3(player["velocity"]);
                car.Rotation = new Vec3(player["quaternion"]);
                if (car.IsOrange)
                {
                    redTeam.Add(car);
                }
                else
                {
                    blueTeam.Add(car);
                }
            }

            this.blueTeam = blueTeam.ToArray();
            this.redTeam = redTeam.ToArray();
            this.Ball = new Ball(jsonFrame["ball"]);
        }

        // Vérifie que le nombre de joueurs est pair et entre 2 et 6
        private bool VerifyPlayerCount(int nbPlayers){
            if (nbPlayers % 2 != 0 || nbPlayers < 2 || nbPlayers > 6)
            {
                Console.WriteLine("Invalid number of players: " + nbPlayers);
                return false;
            }
            return true;
        }

// Vérifie que la balle a une position et une vitesse
        private bool VerifyBall(JSON ball) {
            if (ball == null){
                Console.WriteLine("Invalid frame: no ball data");
                return false;
            }

            if (!VerifyFieldExists(ball, "position", "ball position") ||
                !VerifyFieldExists(ball, "velocity", "ball velocity"))
            {
                return false;
            }

            return true;
        }

        // Vérifie qu'un joueur a les champs nécessaires
        private bool VerifyPlayer(JSON player){
            string[] requiredFields = { "position", "velocity", "quaternion", "is_orange", "boost" };

            foreach (string field in requiredFields)
            {
                if (!VerifyFieldExists(player, field, $"player {field}"))
                {
                    return false;
                }
            }

            return true;
        }

// Méthode générique pour vérifier si un champ existe dans l'objet JSON
        private bool VerifyFieldExists(JSON jsonObject, string fieldName, string errorMessage)
        {
            if (jsonObject[fieldName] == null)
            {
                Console.WriteLine($"Invalid frame: missing {errorMessage}");
                return false;
            }
            return true;
        }

            }
        }