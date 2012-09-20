using System.Linq;

namespace Binbin.HttpHelper
{
    public class APIParameter
    {
        private string name;
        private string value;

        public APIParameter(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get { return name; }
        }

        public string Value
        {
            get { return value; }
        }
    }
}
