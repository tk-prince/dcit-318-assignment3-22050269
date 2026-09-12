using System;
using System.Collections.Generic;
using System.IO;

// Student Class

public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    // Assign grade based on score
    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100)
        {
            return "A";
        }
        else if (Score >= 70 && Score <= 79)
        {
            return "B";
        }
        else if (Score >= 60 && Score <= 69)
        {
            return "C";
        }
        else if (Score >= 50 && Score <= 59)
        {
            return "D";
        }
        else
        {
            return "F";
        }
    }
}


// InvalidScoreFormatException

public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message)
        : base(message)
    {
    }
}

// MissingFieldException

public class MissingFieldException : Exception
{
    public MissingFieldException(string message)
        : base(message)
    {
    }
}


// StudentResultProcessor

public class StudentResultProcessor
{
    // Read students from input file
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        List<Student> students = new List<Student>();

        // using ensures that StreamReader is properly closed
        using (StreamReader reader = new StreamReader(inputFilePath))
        {
            string? line;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;

                // Skip completely empty lines
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // Split the line using comma
                string[] fields = line.Split(',');

                // Validate number of fields
                if (fields.Length != 3)
                {
                    throw new MissingFieldException(
                        $"Line {lineNumber}: " +
                        "A student record must contain ID, Full Name, and Score."
                    );
                }

                // Remove unnecessary spaces
                string idText = fields[0].Trim();
                string fullName = fields[1].Trim();
                string scoreText = fields[2].Trim();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(idText) ||
                    string.IsNullOrWhiteSpace(fullName) ||
                    string.IsNullOrWhiteSpace(scoreText))
                {
                    throw new MissingFieldException(
                        $"Line {lineNumber}: One or more required fields are missing."
                    );
                }

                // Convert student ID
                int id;

                if (!int.TryParse(idText, out id))
                {
                    throw new InvalidScoreFormatException(
                        $"Line {lineNumber}: Student ID '{idText}' is invalid."
                    );
                }

                // Convert score
                int score;

                if (!int.TryParse(scoreText, out score))
                {
                    throw new InvalidScoreFormatException(
                        $"Line {lineNumber}: Score '{scoreText}' is not a valid integer."
                    );
                }

                // Validate score range
                if (score < 0 || score > 100)
                {
                    throw new InvalidScoreFormatException(
                        $"Line {lineNumber}: Score must be between 0 and 100."
                    );
                }

                // Create Student object
                Student student = new Student(
                    id,
                    fullName,
                    score
                );

                // Add valid student to List
                students.Add(student);
            }
        }

        return students;
    }


    // Write results to output file
    public void WriteReportToFile(
        List<Student> students,
        string outputFilePath)
    {
        // using ensures that StreamWriter is properly closed
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            foreach (Student student in students)
            {
                writer.WriteLine(
                    $"{student.FullName} " +
                    $"(ID: {student.Id}): " +
                    $"Score = {student.Score}, " +
                    $"Grade = {student.GetGrade()}"
                );
            }
        }
    }
}


// Main Application

public class Program
{
    public static void Main()
    {
        // Input and output file paths
        string inputFilePath = "students.txt";
        string outputFilePath = "student_report.txt";

        try
        {
            // Create processor
            StudentResultProcessor processor =
                new StudentResultProcessor();

            // Read students from input file
            List<Student> students =
                processor.ReadStudentsFromFile(inputFilePath);

            Console.WriteLine(
                $"{students.Count} student record(s) read successfully."
            );

            // Write report to output file
            processor.WriteReportToFile(
                students,
                outputFilePath
            );

            Console.WriteLine(
                $"Report successfully written to {outputFilePath}"
            );
        }

        // Input file does not exist
        catch (FileNotFoundException)
        {
            Console.WriteLine(
                "Error: The input file could not be found."
            );
        }

        // Invalid score format
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine(
                $"Invalid Score Error: {ex.Message}"
            );
        }

        // Missing field
        catch (MissingFieldException ex)
        {
            Console.WriteLine(
                $"Missing Field Error: {ex.Message}"
            );
        }

        // Any other unexpected error
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Unexpected Error: {ex.Message}"
            );
        }
    }
}
