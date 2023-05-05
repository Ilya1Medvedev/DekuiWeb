namespace BakWeb.Extensions
{
    public static class CodeGenerator
    {
        public static int RandomCode(int length)
        {
            var random = new Random();
            char[] code = new char[length];

            for (int i = 0; i < length; i++)
            {
                int randomDigit = random.Next(0, 10);

                code[i] = (char)('0' + randomDigit);
            }

            return int.Parse(new string(code));
        }
    }
}
