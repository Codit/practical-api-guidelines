using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codit.LevelTwo
{
    public static class Constants
    {
        public static class Messages
        {
            public const string ProblemDetailsDetail = "Please refer to the errors property for additional details.";
        }

        public static class OpenApi
        {
            public const string Title = "Codito Car API";
            public const string Description = "Codito Car models and customizations";
            public const string TermsOfService = "N/A";
            public const string ContactName = "API at Codit";
            public const string ContactEmail = "support@codit.eu";
            public const string ContactUrl = "https://www.codit.eu";
        }

        public static class RouteNames
        {
            // names of controller routes
            public static class v1
            {
                public const string GetCars = "Cars_GetCars";
                public const string GetCar = "Cars_GetCar";
                public const string GetCustomizations = "Customizations_GetCustomizations";
                public const string GetCustomization = "Customizations_GetCustomization";
                public const string CreateCustomization = "Customization_Create";
                public const string UpdateCustomizationIncremental = "Customizations_UpdateIncremental";
                public const string SellCustomization = "Customizations_Sell";
                public const string DeleteCustomization = "Customizations_DeleteCustomization";
            }
        }
    }
}
