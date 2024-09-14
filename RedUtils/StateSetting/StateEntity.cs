//Entity qui contient les informations de l'état de toutes les voitures et de la balle
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RedUtils.Math;

namespace RedUtils.StateSetting
{
    public class StateEntity
    {
        public Car[] blueTeam { get; set; }
        public Car[] orangeTeam { get; set; }
        public Ball Ball { get; set; }

        public StateEntity(string replayFolder, int frame)
        {
            JsonDocument jsonFrame = this.ParseJSON(replayFolder, frame);
            JsonElement root = jsonFrame.RootElement;
            this.verifyFrame(root);
            this.BuildTeam(root);      
        }
        private JsonDocument ParseJSON(string replayFolder, int frame){
            string frameFile = replayFolder + "/parsed_data_f" + frame + ".json";
            JsonDocument json = null;
            try
            {
                string jsonData = File.ReadAllText(frameFile);
                json = JsonDocument.Parse(jsonData);
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("File not found: " + frameFile);
            }
            return json;
        }
        private bool verifyFrame(JsonElement jsonFrame){
            // Accéder à la racine du document JSON

            // Vérification du nombre de joueurs
            if (jsonFrame.TryGetProperty("players", out JsonElement players))
            {
                int nbPlayers = players.GetArrayLength();
                if (!VerifyPlayerCount(nbPlayers)) return false;

                // Vérification de la balle
                if (!jsonFrame.TryGetProperty("ball", out JsonElement ball) || !VerifyBall(ball))
                {
                    return false;
                }

                // Vérification de chaque joueur
                foreach (JsonElement player in players.EnumerateArray())
                {
                    if (!VerifyPlayer(player)) return false;
                }
            }
            else
            {
                Console.WriteLine("Missing players data.");
                return false;
            }

            return true;

        }

        private void BuildTeam(JsonElement jsonFrame){
            // Vérifier si la propriété "players" existe dans jsonFrame
            if (jsonFrame.TryGetProperty("players", out JsonElement players))
            {
                foreach (JsonElement player in players.EnumerateArray())
                {
                    Car car = new Car();

                    // Récupérer et convertir la position en Vec3
                    if (player.TryGetProperty("position", out JsonElement position))
                    {
                        car.Location = new Vec3(
                            position.GetProperty("x").GetSingle(),
                            position.GetProperty("y").GetSingle(),
                            position.GetProperty("z").GetSingle()
                        );
                    }

                    // Récupérer et convertir la vitesse en Vec3
                    if (player.TryGetProperty("velocity", out JsonElement velocity))
                    {
                        car.Velocity = new Vec3(
                            velocity.GetProperty("x").GetSingle(),
                            velocity.GetProperty("y").GetSingle(),
                            velocity.GetProperty("z").GetSingle()
                        );
                    }

                    // Récupérer et convertir la rotation (quaternion) en Vec3 (ou autrement si nécessaire)
                    if (player.TryGetProperty("quaternion", out JsonElement quaternion))
                    {
                        car.Rotation = new Vec3(
                            quaternion.GetProperty("x").GetSingle(),
                            quaternion.GetProperty("y").GetSingle(),
                            quaternion.GetProperty("z").GetSingle()
                        );
                    }

                    // Vérifier si le joueur est dans l'équipe orange
                    if (player.TryGetProperty("is_orange", out JsonElement isOrange) && isOrange.GetBoolean())
                    {
                        this.orangeTeam.Append(car); // Ajouter à l'équipe orange
                    }
                    else
                    {
                        this.blueTeam.Append(car); // Ajouter à l'équipe bleue
                    }
                }
            }
            else
            {
                Console.WriteLine("No players found in the frame data.");
            }
}

        private void BuildBall(JsonElement jsonFrame)
{
    // Vérification et extraction de l'élément "ball"
    if (jsonFrame.TryGetProperty("ball", out JsonElement ballElement))
    {
        // Extraire la position de la balle
        if (ballElement.TryGetProperty("position", out JsonElement positionElement))
        {
            Vec3 position = new Vec3(
                positionElement.GetProperty("x").GetSingle(),
                positionElement.GetProperty("y").GetSingle(),
                positionElement.GetProperty("z").GetSingle()
            );

            // Extraire la vitesse de la balle
            if (ballElement.TryGetProperty("velocity", out JsonElement velocityElement))
            {
                Vec3 velocity = new Vec3(
                    velocityElement.GetProperty("x").GetSingle(),
                    velocityElement.GetProperty("y").GetSingle(),
                    velocityElement.GetProperty("z").GetSingle()
                );

                // Créer un nouvel objet Ball avec la position et la vitesse
                this.Ball = new Ball(position, velocity);
            }
            else
            {
                Console.WriteLine("Missing 'velocity' field for the ball.");
            }
        }
        else
        {
            Console.WriteLine("Missing 'position' field for the ball.");
        }
    }
    else
    {
        Console.WriteLine("Missing 'ball' field in the JSON frame.");
    }
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
        private bool VerifyBall(JsonElement ball) {
            if (ball.ValueKind == JsonValueKind.Null){
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
        private bool VerifyPlayer(JsonElement player){
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
        private bool VerifyFieldExists(JsonElement jsonObject, string fieldName, string errorMessage)
        {
            if (!jsonObject.TryGetProperty(fieldName, out _))
            {
                Console.WriteLine($"Invalid frame: missing {errorMessage}");
                return false;
            }
            return true;
        }

            }
        }