// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Stance.Core;
using Stance.Core.Constants;
using Stance.Core.Contracts;
using Stance.Core.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.Constants;
using Stance.Web.Pages.Auth;
using Xunit;

namespace Stance.Tests.Web.Pages.Auth
{
    public class ValidateDeviceMfaTests
    {
        [Fact]
        public void OnGet_GivenNoUserIsAuthenticated_ExpectNoMfaTypesSet()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);

            var page = new ValidateDeviceMfa(currentAuthenticatedUserProvider.Object, mediator.Object);

            page.OnGet();

            Assert.False(page.HasApp);
        }

        [Fact]
        public void OnGet_GivenUserIsAuthenticated_ExpectMfaTypesToBeSet()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(
                    new UnauthenticatedUser(
                        TestVariables.UserId, MfaProvider.App | MfaProvider.Device) as ISystemUser));

            var page = new ValidateDeviceMfa(currentAuthenticatedUserProvider.Object, mediator.Object);

            page.OnGet();

            Assert.True(page.HasApp);
        }

        [Fact]
        public void OnGet_GivenUserIsAuthenticatedButIsNotRightType_ExpectNoMfaTypesSet()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new Mock<ISystemUser>().Object));

            var page = new ValidateDeviceMfa(currentAuthenticatedUserProvider.Object, mediator.Object);

            page.OnGet();

            Assert.False(page.HasApp);
        }

        [Fact]
        public async Task OnPostAppMfaAsync_GivenCommandFails_ExpectPrgErrorStateAndRedirectToSamePage()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AppMfaRequestedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var page = new ValidateDeviceMfa(currentAuthenticatedUserProvider.Object, mediator.Object);

            Assert.IsType<RedirectToPageResult>(await page.OnPostAppMfaAsync());
        }

        [Fact]
        public async Task OnPostAppMfaAsync_GivenCommandSucceeds_ExpectRedirectAppMfaPage()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AppMfaRequestedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var page = new ValidateDeviceMfa(currentAuthenticatedUserProvider.Object, mediator.Object);

            var result = Assert.IsType<RedirectToPageResult>(await page.OnPostAppMfaAsync());
            Assert.Equal(PageLocations.AuthAppMfa, result.PageName);
        }

        [Fact]
        public async Task OnPostEmailMfaAsync_GivenCommandFails_ExpectPrgErrorStateAndRedirectToSamePage()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EmailMfaRequestedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var page = new ValidateDeviceMfa(currentAuthenticatedUserProvider.Object, mediator.Object);

            Assert.IsType<RedirectToPageResult>(await page.OnPostEmailMfaAsync());
        }

        [Fact]
        public async Task OnPostEmailMfaAsync_GivenCommandSucceeds_ExpectRedirectAppMfaPage()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EmailMfaRequestedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var page = new ValidateDeviceMfa(currentAuthenticatedUserProvider.Object, mediator.Object);

            var result = Assert.IsType<RedirectToPageResult>(await page.OnPostEmailMfaAsync());
            Assert.Equal(PageLocations.AuthEmailMfa, result.PageName);
        }
    }
}