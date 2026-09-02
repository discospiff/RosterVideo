using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RosterVideo.Models;
using Xunit;

namespace RosterVideo.Tests
{
    public class ModelValidationTests
    {
        [Fact]
        public void RosterEntry_Validation_Fails_When_Required_Missing()
        {
            var entry = new RosterEntry();
            var results = new List<ValidationResult>();
            var context = new ValidationContext(entry);

            var valid = Validator.TryValidateObject(entry, context, results, validateAllProperties: true);

            Assert.False(valid);
            Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
            Assert.Contains(results, r => r.MemberNames.Contains("LastName"));
            Assert.Contains(results, r => r.MemberNames.Contains("Shortcut"));
        }
    }
}
