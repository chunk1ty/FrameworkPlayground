using DiContainer.Core;
using DiContainer.Services;

namespace DiContainer
{
    class Program
    {
        static void Main(string[] args)
        {
            var mySimpleContainer = new MySimpleContainer();
            mySimpleContainer.Register<IRepository, Repository>(new Repository());

            mySimpleContainer.Resolve<IRepository>();
            
            
            var myAdvancedContainer = new MyAdvancedContainer();
            myAdvancedContainer.RegisterTransient<IEmailSender, SmtpEmailSender>();
            myAdvancedContainer.RegisterSingleton<IRepository, Repository>();
            myAdvancedContainer.RegisterTransient<Repository>(new Repository());
            myAdvancedContainer.RegisterTransient<LoginController>();
            // myAdvancedContainer.GetInstance(typeof (LoginController));

            // myAdvancedContainer.RegisterSingleton<IRepository, Repository>();
           //  myAdvancedContainer.RegisterSingleton(new Repository());

            var hash1 = myAdvancedContainer.GetService<Repository>().GetHashCode();
            var hash2 = myAdvancedContainer.GetService<IRepository>().GetHashCode();

            var loginController = myAdvancedContainer.GetService<LoginController>();
        }
    }
}