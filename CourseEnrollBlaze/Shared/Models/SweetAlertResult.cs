namespace CourseEnrollBlaze.Shared.Models
{
    public class SweetAlertResult
    {
        public bool IsConfirmed { get; set; }
        public bool IsDenied { get; set; }
        public bool IsDismissed { get; set; }
        public object? Value { get; set; }
    }

}
