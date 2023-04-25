namespace ChatGptCommitMessageGenerator.Models
{
    public struct GitChange
    {
        public string Change { get; set; }
        public int TokenCount { get; set; }
        public int GroupNumber { get; set; }
    }
}