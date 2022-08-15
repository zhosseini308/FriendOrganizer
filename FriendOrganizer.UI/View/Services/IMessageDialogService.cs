using System.Threading.Tasks;
using static FriendOrganizer.UI.View.Services.MessageDialogService;

namespace FriendOrganizer.UI.View.Services
{
    public interface IMessageDialogService
    {
        Task<MessageDialogResult> ShowOkCancelDialogAsync(string text, string title);
        Task ShowInfoDialogAsync(string text);
    }
}