namespace TutorStudent.Application.Contracts
{
    public class ResponseDto
    {
        public ResponseDto(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}