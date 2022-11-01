using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        // Содержимое поста (картинка) описывается путём(-ями) до файла(-ов)
        public string[] Content { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }
        // Тут имеются ввиду лайки
        // public uint ReactionsCounter { get; set; }

        // Мб даже комментарии
    }
}
