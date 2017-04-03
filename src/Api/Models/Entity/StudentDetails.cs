using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity
{
    public class StudentDetails
    {
        // ReSharper disable once UnusedMember.Global
        public StudentDetails() { }

        public StudentDetails(User user, StudentFormModel model)
        {
            UserId = user.Id;
            Discipline = model.Discipline;
            CurrentDegree = model.CurrentDegree;
            CurrentSemester = model.CurrentSemester;
        }

        [Required, ForeignKey("UserId")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public string Discipline { get; set; }

        public string CurrentDegree { get; set; }

        public short CurrentSemester { get; set; }
    }

    public class StudentDetailsMap
    {
        public StudentDetailsMap(EntityTypeBuilder<StudentDetails> entityBuilder)
        {
            entityBuilder.HasKey(d => new { d.UserId });
            entityBuilder.HasOne(d => d.User).WithOne(u => u.StudentDetails).OnDelete(DeleteBehavior.Cascade);

        }
    }
}
