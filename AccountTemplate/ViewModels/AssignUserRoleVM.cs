namespace AccountTemplate.ViewModels
{
    public class AssignUserRoleVM
    {
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string SelectedRole { get; set; }

    }
}
