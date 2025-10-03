using Barangay.Tools;

partial class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting NCD String Decryption...\n");
        
        NCDStringDecryptor.DecryptStrings();
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
