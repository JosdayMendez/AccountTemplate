using AccountTemplate.Models;

namespace AccountTemplate.ViewModels
{
    public class UserBranchVM
    {
        public AppUser User { get; set; }
        public string UserId { get; set; }

        public List<Branch> Branches { get; set; }
        public List<UserBranch> AssignedBranches { get; set; }
        public int[] SelectedBranchIds { get; set; }
    }
}
