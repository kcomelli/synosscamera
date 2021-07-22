using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Configuration
{
    /// <summary>
    /// Parallelize options
    /// </summary>
    public abstract class ParallelizeOptions
    {
        /// <summary>
        /// Enable or disable mulithreaded read/write operations depending on the underlying provider support
        /// </summary>
        public bool EnableMultiThreadedOperations { get; set; } = true;
        /// <summary>
        /// Maximal parallel threads for parallel batch operations
        /// </summary>
        public int MaxParallelThreads { get; set; } = 100;
        /// <summary>
        /// Batch size
        /// </summary>
        public int OperationBatchSize { get; set; } = 10;
    }
}
