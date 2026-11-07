using Microsoft.Data.Sqlite;
using PostService.CommonTypes;
using PostService.Models;

namespace PostService.DataAccess;

public class PostingRepository : IPostingRepository
{
    private readonly string _connectionString;
    private const string Columns = "Id, [From], [To], Content, DeliveryType, Weight, Width, Height, Depth, [Value], Price, Description, CreatedAt";

    public PostingRepository(string connectionString = "Data Source=PostingDb.db;")
    {
        _connectionString = connectionString;
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        try { connection.Open(); return connection; }
        catch { connection.Dispose(); throw; }
    }

    public void CreateDb()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Postings (
                Id INTEGER PRIMARY KEY,
                [From] TEXT NOT NULL,
                [To] TEXT NOT NULL,
                Content TEXT NOT NULL,
                DeliveryType INTEGER NOT NULL,
                Weight REAL NOT NULL,
                Width REAL NOT NULL,
                Height REAL NOT NULL,
                Depth REAL NOT NULL,
                [Value] REAL NULL,
                Price REAL NOT NULL,
                Description TEXT NULL,
                CreatedAt INTEGER NOT NULL);
            """;
        command.ExecuteNonQuery();
    }

    public List<Posting> GetList()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT {Columns} FROM Postings ORDER BY Id;";
        using var reader = command.ExecuteReader();
        var result = new List<Posting>();
        while (reader.Read()) result.Add(ReadPosting(reader));
        return result;
    }

    public Posting? GetById(int postingId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT {Columns} FROM Postings WHERE Id = @Id;";
        command.Parameters.AddWithValue("@Id", postingId);
        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadPosting(reader) : null;
    }

    public int Create(Posting posting)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Postings ([From], [To], Content, DeliveryType, Weight,
                Width, Height, Depth, [Value], Price, Description, CreatedAt)
            VALUES (@From, @To, @Content, @DeliveryType, @Weight,
                @Width, @Height, @Depth, @Value, @Price, @Description, @CreatedAt)
            RETURNING Id;
            """;
        AddParameters(command, posting);
        command.Parameters.AddWithValue("@CreatedAt", new DateTimeOffset(posting.CreatedAt).ToUnixTimeSeconds());
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public int Update(Posting posting)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        // Дата створення залишається незмінною.
        command.CommandText = """
            UPDATE Postings SET [From] = @From, [To] = @To, Content = @Content,
                DeliveryType = @DeliveryType, Weight = @Weight, Width = @Width,
                Height = @Height, Depth = @Depth, [Value] = @Value,
                Price = @Price, Description = @Description
            WHERE Id = @Id;
            """;
        AddParameters(command, posting);
        command.Parameters.AddWithValue("@Id", posting.Id);
        return command.ExecuteNonQuery();
    }

    public int Delete(int postingId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Postings WHERE Id = @Id;";
        command.Parameters.AddWithValue("@Id", postingId);
        return command.ExecuteNonQuery();
    }

    private static void AddParameters(SqliteCommand command, Posting posting)
    {
        command.Parameters.AddWithValue("@From", posting.From);
        command.Parameters.AddWithValue("@To", posting.To);
        command.Parameters.AddWithValue("@Content", posting.Content);
        command.Parameters.AddWithValue("@DeliveryType", (int)posting.DeliveryType);
        command.Parameters.AddWithValue("@Weight", posting.Weight);
        command.Parameters.AddWithValue("@Width", posting.Width);
        command.Parameters.AddWithValue("@Height", posting.Height);
        command.Parameters.AddWithValue("@Depth", posting.Depth);
        command.Parameters.AddWithValue("@Value", (object?)posting.Value ?? DBNull.Value);
        command.Parameters.AddWithValue("@Price", posting.Price);
        command.Parameters.AddWithValue("@Description", (object?)posting.Description ?? DBNull.Value);
    }

    private static Posting ReadPosting(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        From = reader.GetString(reader.GetOrdinal("From")),
        To = reader.GetString(reader.GetOrdinal("To")),
        Content = reader.GetString(reader.GetOrdinal("Content")),
        DeliveryType = (DeliveryType)reader.GetInt32(reader.GetOrdinal("DeliveryType")),
        Weight = reader.GetFloat(reader.GetOrdinal("Weight")),
        Width = reader.GetFloat(reader.GetOrdinal("Width")),
        Height = reader.GetFloat(reader.GetOrdinal("Height")),
        Depth = reader.GetFloat(reader.GetOrdinal("Depth")),
        Value = reader.IsDBNull(reader.GetOrdinal("Value")) ? null : reader.GetFloat(reader.GetOrdinal("Value")),
        Price = reader.GetFloat(reader.GetOrdinal("Price")),
        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
        CreatedAt = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(reader.GetOrdinal("CreatedAt"))).UtcDateTime
    };
}
