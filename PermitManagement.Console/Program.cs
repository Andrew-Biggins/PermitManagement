using PermitManagement.Core.Entities;
using PermitManagement.Presentation;
using System.Globalization;
using static PermitManagement.Presentation.Shared;

using var http = new HttpClient { BaseAddress = new Uri("https://localhost:7158") };
var api = new PermitApiClient(http);

while (true)
{
    Console.WriteLine("\nPermit Management CLI");
    Console.WriteLine("1. Add Permit");
    Console.WriteLine("2. Check Permit");
    Console.WriteLine("3. List Active Permits");
    Console.WriteLine("0. Exit");
    Console.Write("> ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            var vehicle = ReadVehicle();
            var zone = ReadZone();
            var start = ReadDate("Start date");
            var end = ReadDate("End date");

            var permit = new Permit(vehicle, zone, start, end);
            await api.AddPermitAsync(permit);
            Console.WriteLine("Permit added successfully.");
            break;

        case "2":
            vehicle = ReadVehicle();
            zone = ReadZone();
            var valid = await api.CheckPermitAsync(vehicle.Registration, zone.Name);
            Console.WriteLine(valid ? "Valid permit" : "No valid permit");
            break;

        case "3":
            Console.Write("Zone: ");
            zone = ReadZone();
            var permits = await api.GetActivePermitsAsync(zone.Name);
            foreach (var p in permits)
                Console.WriteLine($"{p.Vehicle.Registration} valid {p.StartDate:d} → {p.EndDate:d}");
            break;

        case "0":
            return;
    }
}

static DateTime ReadDate(string prompt)
{
    while (true)
    {
        const string dateFormat = "yyyy-MM-dd";
        Console.Write($"{prompt} ({dateFormat}): ");
        var input = Console.ReadLine();

        if (DateTime.TryParseExact(
                input,
                dateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
        {
            return date;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Invalid date format. Please use ({dateFormat}).");
        Console.ResetColor();
    }
}

static Zone ReadZone()
{
    while (true)
    {
        Console.Write("Zone (A–K): ");
        var input = Console.ReadLine()?.Trim().ToUpperInvariant();

        if (!string.IsNullOrEmpty(input) && input.Length == 1 && input[0] is >= 'A' and <= 'K')
            return new Zone(input);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Invalid zone. Enter a single letter from A to K.");
        Console.ResetColor();
    }
}

static Vehicle ReadVehicle()
{
    while (true)
    {
        Console.Write("Vehicle registration: ");
        var input = Console.ReadLine()?.Trim().ToUpperInvariant();

        if (!string.IsNullOrEmpty(input) && RegPattern.IsMatch(input))
            return new Vehicle(input);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(InvalidRegistrationMessage);
        Console.ResetColor();
    }
}
