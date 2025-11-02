using PermitManagement.Core.Entities;
using PermitManagement.Infrastructure;
using PermitManagement.Shared;

namespace PermitManagement.Api;

public static class SeedData
{
    public static void Initialize(PermitDbContext context)
    {
        if (context.Permits.Any()) return;

        var today = DateTime.Today;

        var zones = Enum.GetNames(typeof(ZoneName));

        var random = new Random(42);
        var permits = new List<Permit>();

        foreach (var zone in zones)
        {
            for (int i = 0; i < 4; i++)
            {
                var startOffset = random.Next(-20, 0);   
                var duration = random.Next(3, 100);       

                var start = today.AddDays(startOffset);
                var end = start.AddDays(duration);

                // Generate a semi-realistic UK-style reg like "AB12ABC"
                var reg = GenerateRegistration(random);

                permits.Add(new Permit(
                    new Vehicle(reg),
                    new Zone(zone),
                    start,
                    end));
            }
        }

        context.Permits.AddRange(permits);

        context.SaveChanges();
    }

    private static string GenerateRegistration(Random random)
    {
        string RandomLetters(int count)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string([.. Enumerable.Range(0, count).Select(_ => letters[random.Next(letters.Length)])]);
        }

        string RandomDigits(int count) => new([.. Enumerable.Range(0, count).Select(_ => (char)('0' + random.Next(10)))]);

        // Format: AA##AAA
        return $"{RandomLetters(2)}{RandomDigits(2)}{RandomLetters(3)}";
    }
}