namespace ClosirisTest.DTOs
{
    public class FileModel
    {
        public int Id { get; set; }
        public string FolderName { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        public DateTime CreationDate { get; set; }
        public string FileImage { get; set; }
        public string FileSize { get; set; }
    }
}