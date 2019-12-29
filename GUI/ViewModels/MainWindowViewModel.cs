using Shared;

namespace GUI
{
    public class MainWindowViewModel : NotifyableObject
    {
        public InventoryViewModel InventoryVM { get; }

        public MainWindowViewModel()
        {
            InventoryVM = new InventoryViewModel(this);
        }
    }
}
