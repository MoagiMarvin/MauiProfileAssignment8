using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using System.Collections.Generic;
using MauiProfileAssignment.Models;

namespace MauiProfileAssignment.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _surname;
        private string _emailAddress;
        private string _bio;
        private ImageSource _profileImage;
        private readonly string _fileName = "profiles.json";
        private readonly string _filePath;
        private List<Profile> _existingProfiles;

        public event PropertyChangedEventHandler PropertyChanged;

        public ProfileViewModel()
        {
            _filePath = Path.Combine(FileSystem.AppDataDirectory, _fileName);
            SaveCommand = new Command(async () => await SaveProfileAsync());
            PickImageCommand = new Command(async () => await PickImageAsync());
            ClearCommand = new Command(ClearFields);
            _existingProfiles = LoadExistingProfiles();
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Surname
        {
            get => _surname;
            set
            {
                if (_surname != value)
                {
                    _surname = value;
                    OnPropertyChanged();
                }
            }
        }

        public string EmailAddress
        {
            get => _emailAddress;
            set
            {
                if (_emailAddress != value)
                {
                    _emailAddress = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _emailError;

        public string EmailError
        {
            get => _emailError;
            set
            {
                if (_emailError != value)
                {
                    _emailError = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Bio
        {
            get => _bio;
            set
            {
                if (_bio != value)
                {
                    _bio = value;
                    OnPropertyChanged();
                }
            }
        }

        public ImageSource ProfileImage
        {
            get => _profileImage;
            set
            {
                if (_profileImage != value)
                {
                    _profileImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand PickImageCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }

        private List<Profile> LoadExistingProfiles()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var jsonString = File.ReadAllText(_filePath);
                    return JsonSerializer.Deserialize<List<Profile>>(jsonString) ?? new List<Profile>();
                }
            }
            catch (Exception)
            {
                // Handle error silently and return empty list
            }
            return new List<Profile>();
        }

        private async Task PickImageAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select Profile Picture"
                });

                if (result != null)
                {
                    var stream = await result.OpenReadAsync();
                    ProfileImage = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to pick image", "OK");
            }
        }

        private void ClearFields()
        {
            Name = string.Empty;
            Surname = string.Empty;
            EmailAddress = string.Empty;
            Bio = string.Empty;
            ProfileImage = null;
        }

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                EmailError = "Email is required";
                return;
            }

            if (!email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                EmailAddress = email + "@gmail.com";
            }
        }

        private async Task SaveProfileAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(EmailAddress))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Name and Email are required", "OK");
                    return;
                }

                // Ensure email ends with @gmail.com
                if (!EmailAddress.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    EmailAddress += "@gmail.com";
                }

                // Check if user exists
                if (_existingProfiles.Any(p => p.EmailAddress.Equals(EmailAddress, StringComparison.OrdinalIgnoreCase)))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "User already exists", "OK");
                    return;
                }

                var profile = new Profile
                {
                    Name = Name,
                    Surname = Surname,
                    EmailAddress = EmailAddress,
                    Bio = Bio,
                    ImagePath = await SaveImageToFile()
                };

                _existingProfiles.Add(profile);
                var jsonString = JsonSerializer.Serialize(_existingProfiles);
                File.WriteAllText(_filePath, jsonString);

                await Application.Current.MainPage.DisplayAlert("Success", "Profile saved successfully", "OK");
                ClearFields();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to save profile", "OK");
            }
        }

        private async Task<string> SaveImageToFile()
        {
            if (ProfileImage == null) return string.Empty;

            try
            {
                var fileName = $"{Guid.NewGuid()}.jpg";
                var imagePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                // Implementation depends on how you're handling the image
                // This is a placeholder for the actual image saving logic
                // You'll need to adapt this based on how you're storing the image

                return fileName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
