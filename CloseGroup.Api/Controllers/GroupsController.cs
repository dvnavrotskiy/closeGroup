using Microsoft.AspNetCore.Mvc;

namespace CloseGroup.Api.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly ICloseGroupService closeGroupSvc;

        public GroupsController(ICloseGroupService closeGroupSvc)
        {
            this.closeGroupSvc = closeGroupSvc;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return closeGroupSvc.GroupsInfo();
        }

        [HttpGet("{productName}")]
        public ActionResult<string> Get(string productName)
        {
            return closeGroupSvc.CloseGroupFor(productName);
        }

        [HttpPost("analyze")]
        public ActionResult<string> Analyze()
        {
            closeGroupSvc.AnalyzeGroups();
            return closeGroupSvc.GroupsInfo();
        }
    }
}
