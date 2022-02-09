﻿namespace CertificationsQuiz.Shared
{
    public class Quiz
    {
        public string Id { get; set; }
        public string Certification { get; set; }
        public string Section { get; set; }
        public string Subsection { get; set; }
        public string Topic { get; set; }
    }

    public class PostQuiz
    {
        public string Certification { get; set; }
        public string Section { get; set; }
        public string Subsection { get; set; }
        public string Topic { get; set; }
    }

    public class SubmitQuiz
    {
        public string id { get; set; }
        public string Certification { get; set; }
        public string Section { get; set; }
        public string Subsection { get; set; }
        public string Topic { get; set; }
        public string Type = "Quiz";
    }
}
