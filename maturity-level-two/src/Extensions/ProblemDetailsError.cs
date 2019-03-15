using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;

namespace Codit.LevelTwo.Extensions
{
    public class ProblemDetailsError : ProblemDetails
    {
        public ProblemDetailsError(int statusCode, string detail=""): base()
        {
            var errorType = statusCode.Between(500, 599, true) ? "server-error" :
                (statusCode.Between(400, 499, true) ? "client-error" : "unknown");

            this.Status = statusCode;
            this.Title = ReasonPhrases.GetReasonPhrase(statusCode);
            if (!String.IsNullOrEmpty(detail)) { this.Detail = detail; }
            this.Instance = $"urn:codit:{errorType}:{Guid.NewGuid()}";
        }
        public ProblemDetailsError() : base()
        { }

    }
}
