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
        private ModConfig _config = null!;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // Register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => _config = new ModConfig(),
                save: () => Helper.WriteConfig(_config)
            );

            // Add some config options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Pet Animals",
                tooltip: () => "Whether to automatically pet farm animals",
                getValue: () => _config.PetAnimals,
                setValue: value => _config.PetAnimals = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Pet Pets",
                tooltip: () => "Whether to automatically pet your pets (cats/dogs)",
                getValue: () => _config.PetPets,
                setValue: value => _config.PetPets = value
            );
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (_config.PetAnimals)
                PetAllAnimals();
            if (_config.PetPets)
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