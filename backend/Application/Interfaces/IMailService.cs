using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMailService
    {
        //Task SendEmailAsync(string receptor, string subject, string body);
        Task SendEmailAsync(string receptor, string subject, string body);
        // Task<bool> SendEmailAsync(EmailData emailConfig);
        Task SendEmailAsync(EmailData emailConfig);
    }
}
