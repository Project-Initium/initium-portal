using FluentValidation;
using Initium.Portal.Core.Settings;
using Initium.Portal.Core.Validation;

namespace Initium.Portal.Tests.Core.Validation
{
    public class PasswordValidatorTests
    {


        public class DummyModel
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
        }

        public class InvalidDummyModelValidator : AbstractValidator<DummyModel>
        {
            public InvalidDummyModelValidator(SecuritySettings securitySettings)
            {
                this.RuleFor(x => x.Prop1)
                    .SetValidator(new PasswordValidator(securitySettings));
            }
        }
    }
    
    
}