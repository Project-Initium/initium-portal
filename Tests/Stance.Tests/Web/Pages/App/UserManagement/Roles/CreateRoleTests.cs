using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Stance.Tests.Web.Pages.App.UserManagement.Roles
{
    public class CreateRoleTests
    {
        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResultToSelfAndNoCommandExecuted()
        {

        }

        [Fact]
        public async Task OnPostAsync_GivenValidModelStateAndSuccessfulResult_ExpectRedirectToPageResultToViewPageAndIdSet()
        {

        }

        [Fact]
        public async Task OnPostAsync_GivenValidModelStateAndFailedResult_ExpectRedirectToPageResultToSelfAndPrgSet()
        {

        }

        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
        }

        [Fact]
        public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
        {
            
        }

        [Fact]
        public void Validate_GivenNameIsNull_ExpectValidationFailure()
        {
            
        }
    }
}
