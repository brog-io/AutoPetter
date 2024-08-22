using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;

namespace AutoPetMod
{
    public class ModConfig
    {
        public bool PetAnimals { get; set; } = true;
        public bool PetPets { get; set; } = true;
    }

    public class ModEntry : Mod
    {
        private ModConfig? _config;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            if (_config == null)
            {
                _config = new ModConfig();
                helper.WriteConfig(_config);
            }
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (_config?.PetAnimals ?? true)
                PetAllAnimals();
            if (_config?.PetPets ?? true)
                PetAllPets();
        }

        private void PetAllAnimals()
        {
            Farm farm = Game1.getFarm();
            foreach (Building building in farm.buildings)
            {
                if (building.indoors.Value is AnimalHouse animalHouse)
                {
                    foreach (FarmAnimal animal in animalHouse.animals.Values)
                    {
                        if (!animal.wasPet.Value)
                        {
                            animal.pet(Game1.player);
                        }
                    }
                }
            }
        }

        private void PetAllPets()
        {
            foreach (NPC character in Utility.getAllCharacters())
            {
                if (character is Pet pet && !pet.lastPetDay.Equals(Game1.Date))
                {
                    pet.checkAction(Game1.player, Game1.currentLocation);
                }
            }
        }
    }
}