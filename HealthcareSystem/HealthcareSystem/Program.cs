using System;
using System.Collections.Generic;
using System.Linq;

// Generic Repository for Entity Management
public class Repository<T>
{
    private List<T> items = new List<T>();

    // Add an item to the repository
    public void Add(T item)
    {
        items.Add(item);
    }

    // Get all items
    public List<T> GetAll()
    {
        return new List<T>(items);
    }

    // Get the first item matching a condition
    public T? GetById(Func<T, bool> predicate)
    {
        return items.FirstOrDefault(predicate);
    }

    // Remove an item matching a condition
    public bool Remove(Func<T, bool> predicate)
    {
        T? item = items.FirstOrDefault(predicate);

        if (item != null)
        {
            return items.Remove(item);
        }

        return false;
    }
}

// b. Patient class
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }
}

// Prescription class
public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(
        int id,
        int patientId,
        string medicationName,
        DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }
}

// g. Healthcare System Application
public class HealthSystemApp
{
    // Repository for patients
    private Repository<Patient> _patientRepo =
        new Repository<Patient>();

    // Repository for prescriptions
    private Repository<Prescription> _prescriptionRepo =
        new Repository<Prescription>();

    // Dictionary to group prescriptions by Patient ID
    private Dictionary<int, List<Prescription>> _prescriptionMap =
        new Dictionary<int, List<Prescription>>();

    // Seed patients and prescriptions
    public void SeedData()
    {
        // Add patients
        _patientRepo.Add(
            new Patient(1, "Kwame Mensah", 25, "Male")
        );

        _patientRepo.Add(
            new Patient(2, "Ama Owusu", 30, "Female")
        );

        _patientRepo.Add(
            new Patient(3, "Kofi Asante", 40, "Male")
        );

        // Add prescriptions
        _prescriptionRepo.Add(
            new Prescription(
                101,
                1,
                "Paracetamol",
                DateTime.Now.AddDays(-5)
            )
        );

        _prescriptionRepo.Add(
            new Prescription(
                102,
                1,
                "Amoxicillin",
                DateTime.Now.AddDays(-3)
            )
        );

        _prescriptionRepo.Add(
            new Prescription(
                103,
                2,
                "Ibuprofen",
                DateTime.Now.AddDays(-2)
            )
        );

        _prescriptionRepo.Add(
            new Prescription(
                104,
                2,
                "Vitamin C",
                DateTime.Now.AddDays(-1)
            )
        );

        _prescriptionRepo.Add(
            new Prescription(
                105,
                3,
                "Cetirizine",
                DateTime.Now
            )
        );
    }

    // Build dictionary by grouping prescriptions by PatientId
    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();

        foreach (Prescription prescription in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] =
                    new List<Prescription>();
            }

            _prescriptionMap[prescription.PatientId]
                .Add(prescription);
        }
    }

    // Retrieve prescriptions for a specific patient
    public List<Prescription> GetPrescriptionsByPatientId(
        int patientId)
    {
        if (_prescriptionMap.ContainsKey(patientId))
        {
            return _prescriptionMap[patientId];
        }

        return new List<Prescription>();
    }

    // Print all patients
    public void PrintAllPatients()
    {
        Console.WriteLine("===== ALL PATIENTS =====");

        foreach (Patient patient in _patientRepo.GetAll())
        {
            Console.WriteLine(
                $"ID: {patient.Id} | " +
                $"Name: {patient.Name} | " +
                $"Age: {patient.Age} | " +
                $"Gender: {patient.Gender}"
            );
        }

        Console.WriteLine();
    }

    // Print prescriptions for a specific patient
    public void PrintPrescriptionsForPatient(int id)
    {
        // Find the patient
        Patient? patient =
            _patientRepo.GetById(p => p.Id == id);

        if (patient == null)
        {
            Console.WriteLine("Patient not found.");
            return;
        }

        Console.WriteLine(
            $"===== PRESCRIPTIONS FOR {patient.Name.ToUpper()} ====="
        );

        List<Prescription> prescriptions =
            GetPrescriptionsByPatientId(id);

        if (prescriptions.Count == 0)
        {
            Console.WriteLine("No prescriptions found.");
            return;
        }

        foreach (Prescription prescription in prescriptions)
        {
            Console.WriteLine(
                $"Prescription ID: {prescription.Id}"
            );

            Console.WriteLine(
                $"Medication: {prescription.MedicationName}"
            );

            Console.WriteLine(
                $"Date Issued: {prescription.DateIssued:dd/MM/yyyy}"
            );

            Console.WriteLine();
        }
    }
}

// Main Application
public class Program
{
    public static void Main()
    {
        // i. Instantiate HealthSystemApp
        HealthSystemApp app = new HealthSystemApp();

        // ii. Seed patient and prescription data
        app.SeedData();

        // iii. Build prescription dictionary
        app.BuildPrescriptionMap();

        // iv. Print all patients
        app.PrintAllPatients();

        // v. Select Patient ID 1
        int selectedPatientId = 1;

        // Display prescriptions for selected patient
        app.PrintPrescriptionsForPatient(selectedPatientId);
    }
}
