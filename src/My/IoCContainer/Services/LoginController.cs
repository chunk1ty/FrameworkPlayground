namespace DiContainer.Services
{
    public class LoginController
    {
        private readonly IRepository _repository;
        private readonly IEmailSender _emailSender;

        public LoginController(IRepository repository, IEmailSender emailSender)
        {
            _repository = repository;
            _emailSender = emailSender;
        }
    }
}