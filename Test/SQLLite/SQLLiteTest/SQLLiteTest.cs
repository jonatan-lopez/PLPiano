using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace SQLLiteTest
{
/// <summary>
/// https://zetcode.com/csharp/sqlite/
/// </summary>
    public class SQL_BasicTest
    {
    
        public static void SQL_ExecuteBasicTest()
        {
            string cs = @"URI=file:test.db";

            using var con = new SQLiteConnection(cs);
            con.Open();

            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = "DROP TABLE IF EXISTS cars";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE cars(id INTEGER PRIMARY KEY,
                name TEXT, price INT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Audi',52642)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Mercedes',57127)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Skoda',9000)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Volvo',29000)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Bentley',350000)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Citroen',21000)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Hummer',41400)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Volkswagen',21600)";
            cmd.ExecuteNonQuery();

            using var cmdQuery = new SQLiteCommand(con);
            cmdQuery.CommandText = "SELECT * FROM cars";
            var dr = cmdQuery.ExecuteReader(System.Data.CommandBehavior.Default);
            while (dr.Read())
            {
                Console.WriteLine($"{dr.GetInt32(0)} {dr.GetString(1)} {dr.GetInt32(2)}");
            }
            
            Console.WriteLine("Table cars created");
        }

        public static void ExecuteDapperTest()
        {
            
            string cs = @"URI=file:test.db";
            string sql = "SELECT  * FROM Cars";
            using var con = new SQLiteConnection(cs);
            con.Open();
            var cars = con.Query<Car>(sql).ToList();
        }
    }
}
