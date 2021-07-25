using System;

namespace synosscamera.core.Tests.Helpers
{
    public class CustomOneAttribute : Attribute
    {
        public string Info { get; private set; }
        public CustomOneAttribute(string info)
        {
            Info = info;
        }
    }

    public class CustomTwoAttribute : Attribute
    {
        public string Info { get; private set; }
        public CustomTwoAttribute(string info)
        {
            Info = info;
        }
    }
}
