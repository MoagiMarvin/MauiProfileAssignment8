using MauiProfileAssignment.ViewModels;

namespace MauiProfileAssignment.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = new ProfileViewModel();
    }
}