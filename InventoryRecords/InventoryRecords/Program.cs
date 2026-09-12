
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Marker Interface

public interface IInventoryEntity
{
    int Id { get; }
}


// Immutable Inventory Record

public record InventoryItem(
    int Id,
    string Name,
    int Quantity,
    DateTime DateAdded
) : IInventoryEntity;


// Generic Inventory Logger

public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log;
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _log = new List<T>();
        _filePath = filePath;
    }


    // Add an item to the log
    public void Add(T item)
    {
        _log.Add(item);
    }


    // Return all items
    public List<T> GetAll()
    {
        return new List<T>(_log);
    }


    // Save all items to a JSON file
    public void SaveToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(
                _log,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }
            );

            using (StreamWriter writer =
                   new StreamWriter(_filePath))
            {
                writer.Write(json);
            }

            Console.WriteLine(
                "Inventory data saved successfully."
            );
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine(
                "Error: Access to the inventory file was denied."
            );
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine(
                "Error: The specified directory was not found."
            );
        }
        catch (IOException ex)
        {
            Console.WriteLine(
                $"File error while saving inventory: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Unexpected error while saving: {ex.Message}"
            );
        }
    }


    // Load inventory data from the JSON file
    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine(
                    "Inventory file does not exist."
                );

                return;
            }

            string json;

            using (StreamReader reader =
                   new StreamReader(_filePath))
            {
                json = reader.ReadToEnd();
            }

            List<T>? loadedItems =
                JsonSerializer.Deserialize<List<T>>(json);

            if (loadedItems != null)
            {
                _log = loadedItems;
            }

            Console.WriteLine(
                "Inventory data loaded successfully."
            );
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine(
                "Error: Access to the inventory file was denied."
            );
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine(
                "Error: Inventory file could not be found."
            );
        }
        catch (JsonException)
        {
            Console.WriteLine(
                "Error: The inventory file contains invalid data."
            );
        }
        catch (IOException ex)
        {
            Console.WriteLine(
                $"File error while loading inventory: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Unexpected error while loading: {ex.Message}"
            );
        }
    }
}


// f. InventoryApp

public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(
            filePath
        );
    }


    // Add sample inventory records
    public void SeedSampleData()
    {
        _logger.Add(
            new InventoryItem(
                101,
                "Laptop",
                10,
                DateTime.Now
            )
        );

        _logger.Add(
            new InventoryItem(
                102,
                "Keyboard",
                25,
                DateTime.Now
            )
        );

        _logger.Add(
            new InventoryItem(
                103,
                "Mouse",
                30,
                DateTime.Now
            )
        );

        _logger.Add(
            new InventoryItem(
                104,
                "Monitor",
                15,
                DateTime.Now
            )
        );

        _logger.Add(
            new InventoryItem(
                105,
                "Printer",
                5,
                DateTime.Now
            )
        );

        Console.WriteLine(
            "Sample inventory data added."
        );
    }


    // Save data
    public void SaveData()
    {
        _logger.SaveToFile();
    }


    // Load data
    public void LoadData()
    {
        _logger.LoadFromFile();
    }


    // Print all loaded inventory items
    public void PrintAllItems()
    {
        Console.WriteLine();
        Console.WriteLine(
            "INVENTORY ITEMS"
        );

        List<InventoryItem> items =
            _logger.GetAll();

        if (items.Count == 0)
        {
            Console.WriteLine(
                "No inventory items found."
            );

            return;
        }

        foreach (InventoryItem item in items)
        {
            Console.WriteLine(
                $"ID: {item.Id} | " +
                $"Name: {item.Name} | " +
                $"Quantity: {item.Quantity} | " +
                $"Date Added: {item.DateAdded:dd/MM/yyyy}"
            );
        }
    }
}

// g. Main Application

public class Program
{
    public static void Main()
    {
        string filePath = "inventory.json";
        // First session

        Console.WriteLine("FIRST SESSION");

        InventoryApp app = new InventoryApp(filePath);

        // Seed sample data
        app.SeedSampleData();

        // Save data to disk
        app.SaveData();


        // Clear memory / simulate a new session

        app = null!;

        Console.WriteLine();
        Console.WriteLine("===== NEW SESSION =====");


        // Second session

        InventoryApp newApp =
            new InventoryApp(filePath);

        // Load data from disk
        newApp.LoadData();

        // Print recovered data
        newApp.PrintAllItems();
    }
}
