using SQLLiteTest;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        SQL_BasicTest.SQL_ExecuteBasicTest();
        SQL_BasicTest.ExecuteDapperTest();
    }
}