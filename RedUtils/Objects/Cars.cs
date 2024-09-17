﻿using System;
using System.Collections.Generic;
using rlbot.flat;
using RedUtils.Math;
using RedUtils.StateSetting;

namespace RedUtils
{
	/// <summary>Contains static properties on the cars currently in the game</summary>
	public static class Cars
	{
		public static int frame = 0;
		/// <summary>How many cars are currently in the game</summary>
		public static int Count => AllCars.Count;
		/// <summary>All the cars in the game, including cars that are respawning</summary>
		public static List<Car> AllCars { get; private set; }
		/// <summary>All the cars in the game, NOT including cars that are respawning</summary>
		public static List<Car> AllLivingCars { get { return AllCars.FindAll(car => !car.IsDemolished); } }
		/// <summary>All cars on the blue team, including cars that are respawning</summary>
		public static List<Car> BlueCars { get { return AllCars.FindAll(car => car.Team == 0); } }
		/// <summary>All cars on the blue team, NOT including cars that are respawning</summary>
		public static List<Car> LivingBlueCars { get { return AllCars.FindAll(car => !car.IsDemolished && car.Team == 0); } }
		/// <summary>All cars on the orange team, including cars that are respawning</summary>
		public static List<Car> OrangeCars { get { return AllCars.FindAll(car => car.Team == 1); } }
		/// <summary>All cars on the orange team, NOT including cars that are respawning</summary>
		public static List<Car> LivingOrangeCars { get { return AllCars.FindAll(car => !car.IsDemolished && car.Team == 1); } }

		/// <summary>Initliazes the list of cars with data from the packet</summary>
		public static void Initialize(GameTickPacket packet)
		{
			AllCars = new List<Car>();

			for (int i = 0; i < packet.PlayersLength; i++)
			{
				AllCars.Add(new Car(i, packet.Players(i).Value));
			}
		}

		/// <summary>Updates the list of cars with data from the packet</summary>
		public static void Update(GameTickPacket packet)
		{
			foreach (Car car in AllCars)
			{
				car.Update(packet.Players(car.Index).Value);
			}
		}

		public static void StateSet(StateEntity state, int frame)
		{
			//permet de ne pas le relancer pour chaque bot
			if (Cars.frame == frame)
			{
				return;
			}
			Cars.frame = frame;
			AllCars = new List<Car>();
			//le for avec un enumerate
			for (int i = 0; i < state.blueTeam.Length; i++)
			{
				Car player = state.blueTeam[i];
				BlueCars[i].UpdateStateSetter(
					player.Location,
				 	player.Velocity,
				 	player.Rotation,
				   	player.Orientation,
				    player.Boost
					);
			}
			for (int i = 0; i < state.orangeTeam.Length; i++)
			{
				Car player = state.orangeTeam[i];
				OrangeCars[i].UpdateStateSetter(
					player.Location,
				 	player.Velocity,
				 	player.Rotation,
				   	player.Orientation,
				    player.Boost
					);
			}
		}
	}
}
