namespace CertificationsQuiz.Shared
{
    public class Question : PostQuestion
    {
        public string Id { get; set; }
    }

    public class PostQuestion
    {
        public string Body { get; set; }
        public string Answer { get; set; }
        public string[] IncorrectChoices { get; set; }

        public string QuizId { get; set; }
    }

    public class SubmitQuestion : PostQuestion
    {
        public string id { get; set; }
        public string Type = "Question";
    }
}