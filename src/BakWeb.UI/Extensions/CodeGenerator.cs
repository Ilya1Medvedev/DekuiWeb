namespace BakWeb.Extensions
{
    public static class CodeGenerator
    {
        static string RandomCode(int length)
        {
            var random = new Random();
            char[] code = new char[length];

            for (int i = 0; i < length; i++)
            {
                // Generate a random digit (0-9)
                int randomDigit = random.Next(0, 10);

                code[i] = (char)('0' + randomDigit);
            }

            return new string(code);
        }
    }
}
