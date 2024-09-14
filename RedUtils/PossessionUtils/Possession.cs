using System;
using System.Collections.Generic;
using RedUtils.Math;

namespace RedUtils
{
	/// <summary>A bunch of various helpful math and physics Possession check</summary>
	public enum PossessionStatus
    {
        MyPossession,
        TeammatePossession,
        OpponentPossession,
        OpponentPossessionButClosest
    }

	public static class Possession{
		 /// <summary>
        /// Détermine la possession de la balle.
        /// </summary>
        /// <param name="myCar">Le joueur actuel (toi)</param>
        /// <param name="teamMates">Liste des équipiers</param>
        /// <param name="opponents">Liste des adversaires</param>
        /// <param name="ball">Position et mouvement de la balle</param>
        /// <returns>Le statut de la possession</returns>
		public static PossessionStatus HasPossession(Car myCar, Car[] teamMates, Car[] opponents, Ball ball)
		{
			return car.Location.Dist(ball.Location) < threshold;
		}

		 public static float GetTimeToReachBall(Car car, Ball ball)
        {
            Vec3 relativePosition = ball.location - car.Location; // Position relative de la balle par rapport à la voiture
            Vec3 relativeVelocity = ball.velocity - car.Velocity; // Vitesse relative entre la voiture et la balle

            // Angle entre la direction de la voiture et la direction vers la balle
            float angleToBall = AngleBetween(car.Velocity, relativePosition);
            float currentSpeed = car.Velocity.Length();

            // Si la voiture est déjà à la vitesse max
            if (currentSpeed >= Car.MaxSpeed)
            {
                return TimeToReachMovingTarget(relativePosition, relativeVelocity, Car.MaxSpeed);
            }

            // Sinon on calcule en prenant en compte l'accélération
            float timeToMaxSpeed = (Car.MaxSpeed - currentSpeed) / Car.BoostAccel;
            float distanceDuringAcceleration = currentSpeed * timeToMaxSpeed + 0.5f * Car.BoostAccel * timeToMaxSpeed * timeToMaxSpeed;

            // Prédiction en fonction de la vitesse relative de la balle et de la voiture
            float predictedTimeToReach = TimeToReachMovingTarget(relativePosition, relativeVelocity, MaxCarSpeed);

            return predictedTimeToReach;
        }

        /// <summary>
        /// Calcule le temps d'interception entre la voiture et la balle en mouvement.
        /// </summary>
        /// <param name="relativePosition">Position relative de la balle par rapport à la voiture</param>
        /// <param name="relativeVelocity">Vitesse relative de la balle par rapport à la voiture</param>
        /// <param name="carSpeed">Vitesse de la voiture</param>
        /// <returns>Le temps en secondes pour intercepter la balle</returns>
        public static float TimeToReachBall(Car car)
        {
            float timeToReach = float.MaxValue;

        // Parcourt les prédictions de la balle (slices) et trouve le premier slice atteignable
            BallSlice reachableSlice = Ball.Prediction.Find(slice => 
            {
                // Calculer le temps pour ce slice
                timeToReach = getTimeToReachBallSlice(slice, car);
                return timeToReach != float.MaxValue; // Si le slice est atteignable, arrêter la recherche
            });

            // Si un slice atteignable a été trouvé, retourner le temps calculé
            if (reachableSlice != null)
            {
                return timeToReach; // Le temps a déjà été calculé
            }

        // Si aucun slice atteignable, retourner une valeur indicative
            return float.MaxValue;

        }
        public static float TimeToReachTarget(Car car, Vec3 targetPosition)
        {
            Vec3 relativePosition = targetPosition - car.Location;  // Calcul de la position relative entre la voiture et la cible
            float distanceToTarget = relativePosition.Length();      // Distance à parcourir
            float currentSpeed = car.Velocity.Length();              // Vitesse actuelle de la voiture

            // Si la voiture est déjà à sa vitesse maximale, calcul direct
            if (currentSpeed >= Car.MaxSpeed)
            {
                return distanceToTarget / Car.MaxSpeed;
            }

            // Calcul du temps nécessaire pour atteindre la vitesse maximale
            float timeToMaxSpeed = (Car.MaxSpeed - currentSpeed) / Car.BoostAccel;

            // Distance parcourue pendant l'accélération
            float distanceDuringAcceleration = currentSpeed * timeToMaxSpeed + 0.5f *  Car.BoostAccel * timeToMaxSpeed * timeToMaxSpeed;

            if (distanceDuringAcceleration >= distanceToTarget)
            {
                // La voiture atteindra la position cible avant d'atteindre la vitesse maximale
                return (float)(-currentSpeed + MathF.Sqrt(currentSpeed * currentSpeed + 2 *  Car.BoostAccel * distanceToTarget)) /  Car.BoostAccel;
            }
            else
            {
                // La voiture atteindra la vitesse maximale avant d'atteindre la position cible
                float remainingDistance = distanceToTarget - distanceDuringAcceleration;
                float timeAtMaxSpeed = remainingDistance / Car.MaxSpeed;

                // Temps total = temps pour atteindre la vitesse max + temps pour parcourir la distance restante à vitesse max
                return timeToMaxSpeed + timeAtMaxSpeed;
            }
        }
        public static float getTimeToReachBallSlice(BallSlice slice, Car car){
            float timeRemaining = slice.Time - Game.Time;
            if(timeRemaining < 0){
                return float.MaxValue;
            }
            float timeToReach = TimeToReachTarget(car, slice.Location);
            if (timeToReach < timeRemaining){
                return timeToReach;
            };
            return float.MaxValue;
        }
            
    }
        
}
