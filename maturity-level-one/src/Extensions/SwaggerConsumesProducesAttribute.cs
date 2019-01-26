using System;

namespace Codit.LevelOne.Extensions
{
    /// <summary>
    /// SwaggerConsumesProducesAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class SwaggerConsumesProducesAttribute : Attribute
    {
        /// <summary>
        /// SwaggerProducesMediaTypeAttribute
        /// </summary>
        public SwaggerConsumesProducesAttribute()
        {
        }
        /// <summary>
        /// Request Content Type
        /// </summary>
        public string Consumes { get; set; }
        /// <summary>
        /// Response Content Type
        /// </summary>
        public string Produces { get; set; }

        /// <summary>
        /// Remove all other Response Content Types
        /// </summary>
        public bool Clear { get; set; }
    }
}