using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer
{
    /// <summary>
    /// The Object for returning data to the UI
    /// </summary>
    public class ReturnObject
    {
        /// <summary>
        /// Status of request
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Message to the user
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Object that contain the response
        /// </summary>
        public object returnObject { get; set; }

    }
}
