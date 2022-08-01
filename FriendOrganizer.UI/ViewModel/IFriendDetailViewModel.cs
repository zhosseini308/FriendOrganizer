using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.ViewModel
{
    public interface IFriendDetailViewModel
    {
        bool HasChanges { get; }
        Task LoadAsync(int? friendId);
    }
}