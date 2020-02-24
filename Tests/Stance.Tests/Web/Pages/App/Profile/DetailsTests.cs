using System.Threading.Tasks;
using Xunit;

namespace Stance.Tests.Web.Pages.App.Profile
{
    public class DetailsTests
    {
        [Fact]
        public async Task OnGetAsync_GiveNullPageModelAndNoProfile_ExpectNotFoundResult()
        {

        }

        [Fact]
        public async Task OnGetAsync_GiveNullPageModelAndProfile_ExpectPageResultAndPageModelDataSet()
        {
            
        }

        [Fact]
        public async Task OnGetAsync_GivePageModelIsNotNull_ExpectPageResultAndPageModelDataSetWithoutDataCall()
        {

        }
    }
}