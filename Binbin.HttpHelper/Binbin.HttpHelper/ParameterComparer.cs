using System.Collections.Generic;

namespace Binbin.HttpHelper
{
    /// <summary>
    /// Comparer class used to perform the sorting of the query parameters
    /// </summary>
    public class ParameterComparer : IComparer<APIParameter>
    {
        public int Compare(APIParameter x, APIParameter y)
        {
            if (x.Name == y.Name)
            {
                return string.Compare(x.Value, y.Value);
            }
            else
            {
                return string.Compare(x.Name, y.Name);
            }
        }
    }
}