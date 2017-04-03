using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.User
{
    public class StudentFormModel
    {
        [Required]
        public string Discipline { get; set; }

        [Required]
        public string CurrentDegree { get; set; }

        [Required]
        public short CurrentSemester { get; set; }
    }
}
