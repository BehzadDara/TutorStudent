namespace TutorStudent.Application.Contracts
{
    public class ResponseDto
    {
        public ResponseDto(string message)
        {
            _message = message;
        }

        private string _message;
    }
}