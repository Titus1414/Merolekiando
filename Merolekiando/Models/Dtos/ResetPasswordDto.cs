namespace Merolekando.Models.Dtos
{
    public class ResetPasswordDto
    {
        public int Uid { get; set; }
        public string OldPass { get; set; }
        public string NewPass { get; set; }
        public string SamePass { get; set; }
    }
}
