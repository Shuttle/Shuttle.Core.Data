using System;

namespace Shuttle.Core.Data
{
    public class DatabaseContextFactoryOptions
    {
        public string DefaultConnectionStringName { get; set; }
        public TimeSpan DefaultCreateTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}