using System.IO;
using System.Reflection;

namespace SoundCloudDl.Methods
{
    public class Meta
    {
        public string GetAssemblyPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Remove(0, 6); ;
        }
    }
}
