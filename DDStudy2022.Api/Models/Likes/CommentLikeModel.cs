namespace DDStudy2022.Api.Models.Likes
{
    // Пока бесполезная модель, но может пригодится если мы захотим доставать всех лайкнувших
    public class CommentLikeModel
    {
        public Guid UserId { get; set; }
        public Guid CommentId { get; set; }
    }
}
