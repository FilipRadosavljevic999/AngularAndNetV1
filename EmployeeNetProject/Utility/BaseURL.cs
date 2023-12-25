namespace EmployeeNetProject.Utility
{
    public class BaseURL
    {
        public static string Url { get=> GetUrl();  }
        private static string GetUrl()
        {
            string filePath = "Url.txt";
            string[] lines = System.IO.File.ReadAllLines(filePath);
            return lines[0];
        }

    }
}
