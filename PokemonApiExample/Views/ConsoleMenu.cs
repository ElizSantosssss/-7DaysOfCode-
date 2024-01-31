using System;
using System.Collections.Generic;
using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;

public class ConsoleMenu
{
    private string playerName;
    private string adoptedPokemon;

    public void Run()
    {
        Console.WriteLine("Qual é o seu nome?");
        playerName = Console.ReadLine();

        while (true)
        {
            DisplayMainMenu();
            string userChoice = Console.ReadLine();

            switch (userChoice.ToLower())
            {
                case "1":
                    AdoptionMenu();
                    break;
                case "2":
                    ViewAdoptedPokemon();
                    break;
                case "3":
                    Console.WriteLine($"Obrigado por jogar, {playerName}!");
                    return;
                default:
                    Console.WriteLine("Escolha inválida. Tente novamente.");
                    break;
            }
        }
    }

    private void DisplayMainMenu()
    {
        Console.WriteLine($"\nMENU, {playerName}! O que você deseja?");
        Console.WriteLine("1. Adotar um mascote virtual");
        Console.WriteLine("2. Ver seus mascotes");
        Console.WriteLine("3. Sair do jogo");
        Console.Write("Escolha uma opção: ");
    }

    private void AdoptionMenu()
    {
        Console.WriteLine("\nADOTAR UM MASCOTE. Escolha uma espécie:");

        List<string> availableSpecies = new List<string>
        {
            "Bulbasaur",
            "Charmander",
            "Squirtle"
        };

        for (int i = 0; i < availableSpecies.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {availableSpecies[i]}");
        }

        Console.WriteLine($"{availableSpecies.Count + 1}. Voltar ao Menu Principal");
        Console.Write("Escolha uma espécie para adoção: ");
        string userChoice = Console.ReadLine();

        if (int.TryParse(userChoice, out int chosenIndex) && chosenIndex >= 1 && chosenIndex <= availableSpecies.Count)
        {
            string selectedSpecies = availableSpecies[chosenIndex - 1];
            DisplayPokemonDetails(selectedSpecies);

            Console.WriteLine("Você deseja:");
            Console.WriteLine("1. Saber mais sobre o Pokémon");
            Console.WriteLine("2. Adotar o Pokémon");
            Console.WriteLine("3. Voltar");
            Console.Write("Escolha uma opção: ");
            string adoptionChoice = Console.ReadLine();

            switch (adoptionChoice.ToLower())
            {
                case "1":
                    // Lógica para mostrar mais detalhes sobre o Pokémon
                    DisplayAdditionalDetails(selectedSpecies);
                    break;
                case "2":
                    AdoptPokemon(selectedSpecies);
                    break;
                case "3":
                    Console.WriteLine("Voltando ao Menu Principal...");
                    break;
                default:
                    Console.WriteLine("Escolha inválida. Voltando ao Menu Principal...");
                    break;
            }
        }
        else if (userChoice == (availableSpecies.Count + 1).ToString())
        {
            Console.WriteLine("Retornando ao Menu Principal...");
        }
        else
        {
            Console.WriteLine("Escolha inválida. Tente novamente.");
        }
    }

    private void DisplayPokemonDetails(string species)
    {
        string apiUrl = $"https://pokeapi.co/api/v2/pokemon/{species.ToLower()}/";
        var client = new RestClient(apiUrl);
        var request = new RestRequest(Method.GET);
        IRestResponse response = client.Execute(request);

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            try
            {
                // Parse do JSON para JsonDocument
                using (JsonDocument doc = JsonDocument.Parse(response.Content))
                {
                    var root = doc.RootElement;

                    // Exibição das informações do Pokémon
                    Console.WriteLine($"\nNome Pokémon: {root.GetProperty("name").GetString() ?? "N/A"}");
                    Console.WriteLine($"Altura: {root.GetProperty("height").GetInt32()} decímetros");
                    Console.WriteLine($"Peso: {root.GetProperty("weight").GetInt32()} hectogramas");

                    Console.WriteLine("Habilidades:");
                    var abilitiesArray = root.GetProperty("abilities").EnumerateArray();
                    foreach (var abilityElement in abilitiesArray)
                    {
                        Console.WriteLine($"- {abilityElement.GetProperty("ability").GetProperty("name").GetString() ?? "N/A"}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar a resposta: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Falha na requisição. Código de Status: {response.StatusCode}");
        }
    }

    private void DisplayAdditionalDetails(string species)
    {
        Console.WriteLine($"Detalhes adicionais sobre {species} serão mostrados em versões futuras.");
    }

    private void AdoptPokemon(string species)
    {
        adoptedPokemon = species;
        Console.WriteLine($"Mascote adotado com sucesso! O ovo está chocando...");
    }

    private void ViewAdoptedPokemon()
    {
        if (string.IsNullOrEmpty(adoptedPokemon))
        {
            Console.WriteLine("Você ainda não adotou nenhum mascote.");
        }
        else
        {
            Console.WriteLine($"Seu mascote adotado: {adoptedPokemon}");
        }
    }
}
