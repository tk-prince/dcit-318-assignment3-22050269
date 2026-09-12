using System;
using System.Collections.Generic;
// Marker Interface for Inventory Items

public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}


// ElectronicItem Class

public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(
        int id,
        string name,
        int quantity,
        string brand,
        int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }
}


// GroceryItem Class

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(
        int id,
        string name,
        int quantity,
        DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }
}


// Custom Exceptions

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message)
        : base(message)
    {
    }
}


public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message)
        : base(message)
    {
    }
}


public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message)
        : base(message)
    {
    }
}


// Generic Inventory Repository

public class InventoryRepository<T> where T : IInventoryItem
{
    // Dictionary uses item ID as the key
    private Dictionary<int, T> _items =
        new Dictionary<int, T>();


    // Add an item
    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException(
                $"An item with ID {item.Id} already exists."
            );
        }

        if (item.Quantity < 0)
        {
            throw new InvalidQuantityException(
                $"Quantity for {item.Name} cannot be negative."
            );
        }

        _items.Add(item.Id, item);
    }


    // Get item by ID
    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out T? item))
        {
            throw new ItemNotFoundException(
                $"Item with ID {id} was not found."
            );
        }

        return item;
    }


    // Remove item
    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
        {
            throw new ItemNotFoundException(
                $"Cannot remove item. ID {id} was not found."
            );
        }

        _items.Remove(id);
    }


    // Get all items
    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }


    // Update quantity
    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException(
                "Quantity cannot be negative."
            );
        }

        T item = GetItemById(id);

        item.Quantity = newQuantity;
    }
}


// WarehouseManager Class

public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics =
        new InventoryRepository<ElectronicItem>();

    private InventoryRepository<GroceryItem> _groceries =
        new InventoryRepository<GroceryItem>();


    // Seed inventory data
    public void SeedData()
    {
        try
        {
            // -------------------------
            // Grocery Items
            // -------------------------

            _groceries.AddItem(
                new GroceryItem(
                    101,
                    "Rice",
                    50,
                    DateTime.Now.AddMonths(12)
                )
            );

            _groceries.AddItem(
                new GroceryItem(
                    102,
                    "Milk",
                    30,
                    DateTime.Now.AddMonths(2)
                )
            );

            _groceries.AddItem(
                new GroceryItem(
                    103,
                    "Bread",
                    20,
                    DateTime.Now.AddDays(7)
                )
            );


            // Electronic Items

            _electronics.AddItem(
                new ElectronicItem(
                    201,
                    "Laptop",
                    10,
                    "Dell",
                    24
                )
            );

            _electronics.AddItem(
                new ElectronicItem(
                    202,
                    "Smartphone",
                    15,
                    "Samsung",
                    12
                )
            );

            _electronics.AddItem(
                new ElectronicItem(
                    203,
                    "Headphones",
                    25,
                    "Sony",
                    6
                )
            );

            Console.WriteLine("Inventory data seeded successfully.");
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Duplicate Error: {ex.Message}");
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Quantity Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error: {ex.Message}");
        }
    }


    // Generic method to print all items
    public void PrintAllItems<T>(
        InventoryRepository<T> repo)
        where T : IInventoryItem
    {
        List<T> items = repo.GetAllItems();

        foreach (T item in items)
        {
            Console.WriteLine(
                $"ID: {item.Id} | " +
                $"Name: {item.Name} | " +
                $"Quantity: {item.Quantity}"
            );

            // Display additional information
            // depending on the product type

            if (item is ElectronicItem electronic)
            {
                Console.WriteLine(
                    $"Brand: {electronic.Brand} | " +
                    $"Warranty: {electronic.WarrantyMonths} months"
                );
            }
            else if (item is GroceryItem grocery)
            {
                Console.WriteLine(
                    $"Expiry Date: {grocery.ExpiryDate:dd/MM/yyyy}"
                );
            }

            Console.WriteLine();
        }
    }


    // Generic method to increase stock
    public void IncreaseStock<T>(
        InventoryRepository<T> repo,
        int id,
        int quantity)
        where T : IInventoryItem
    {
        try
        {
            if (quantity < 0)
            {
                throw new InvalidQuantityException(
                    "Stock increase quantity cannot be negative."
                );
            }

            T item = repo.GetItemById(id);

            int newQuantity = item.Quantity + quantity;

            repo.UpdateQuantity(id, newQuantity);

            Console.WriteLine(
                $"Stock increased successfully. " +
                $"{item.Name} now has {item.Quantity} units."
            );
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"Not Found: {ex.Message}");
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Quantity Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error: {ex.Message}");
        }
    }


    // Generic method to remove an item
    public void RemoveItemById<T>(
        InventoryRepository<T> repo,
        int id)
        where T : IInventoryItem
    {
        try
        {
            T item = repo.GetItemById(id);

            repo.RemoveItem(id);

            Console.WriteLine(
                $"Item '{item.Name}' removed successfully."
            );
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"Not Found: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error: {ex.Message}");
        }
    }


    // Public properties allow Main() to access
    // the two repositories.
    public InventoryRepository<ElectronicItem> Electronics
    {
        get { return _electronics; }
    }

    public InventoryRepository<GroceryItem> Groceries
    {
        get { return _groceries; }
    }
}


// Main Application

public class Program
{
    public static void Main()
    {
        // Instantiate WarehouseManager
        WareHouseManager manager = new WareHouseManager();


        // Seed data
        manager.SeedData();


        // Print all grocery items
        Console.WriteLine();
        Console.WriteLine("========== GROCERY INVENTORY ==========");

        manager.PrintAllItems(manager.Groceries);


        // Print all electronic items
        Console.WriteLine();
        Console.WriteLine("======= ELECTRONICS INVENTORY =========");

        manager.PrintAllItems(manager.Electronics);


        // Test exception handling
        // Try to add a duplicate item

        Console.WriteLine();
        Console.WriteLine("========== TEST DUPLICATE ITEM ==========");

        try
        {
            manager.Groceries.AddItem(
                new GroceryItem(
                    101,
                    "Another Rice",
                    10,
                    DateTime.Now.AddMonths(6)
                )
            );
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine(
                $"Duplicate Item Error: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Unexpected Error: {ex.Message}"
            );
        }


        // Try to remove a non-existent item

        Console.WriteLine();
        Console.WriteLine("====== TEST NON-EXISTENT ITEM ======");

        try
        {
            manager.Electronics.RemoveItem(999);
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine(
                $"Item Not Found Error: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Unexpected Error: {ex.Message}"
            );
        }


        // Try to update with invalid quantity

        Console.WriteLine();
        Console.WriteLine("====== TEST INVALID QUANTITY ======");

        try
        {
            manager.Groceries.UpdateQuantity(
                102,
                -20
            );
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine(
                $"Invalid Quantity Error: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Unexpected Error: {ex.Message}"
            );
        }


        // Additional successful operation

        Console.WriteLine();
        Console.WriteLine("========== INCREASE STOCK ==========");

        manager.IncreaseStock(
            manager.Electronics,
            201,
            5
        );
    }
}
