using System.Collections.Generic;
using AccountTemplate.Models;

namespace AccountTemplate.ViewModels
{
    public class ProfileBranchVM
    {
        public Profile Profile { get; set; }
        public string UserId { get; set; }

        public List<Branch> Branches { get; set; }
        public List<ProfileBranch> AssignedBranches { get; set; }
        public int[] SelectedBranchIds { get; set; }
    }

}