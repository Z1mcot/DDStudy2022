using DDStudy2022.Api.Models.Users;

namespace DDStudy2022.Api.Models.Comments
{
    public class CommentModel
    {
        // Пока что у нас два одинаковых класса, это будет продолжаться до тех пор
        // пока я не смогу нормально пристроить сюда UserAvatarModel
        // Всё так и задуманно
        public Guid AuthorId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime PublishDate { get; set; }
    }
}
