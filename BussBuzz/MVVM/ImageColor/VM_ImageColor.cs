using BussBuzz.Base;
using BussBuzz.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.MVVM.ImageColor
{
    public partial class VM_ImageColor: ObservableObject, VM_Base
    {
        public VM_ImageColor() 
        {
            this.ImagePath = VM_CurrentUser.Instance.Image;
            this.Color = ColorConverterService.Instance.ConvertStringToColor(VM_CurrentUser.Instance.Color);
            InitImagesList();
        }

        private string imagePath;
        public string ImagePath
        {
            get { return imagePath; }
            set { SetProperty<string>(ref imagePath, value); }
        }

        private Color color;
        public Color Color
        {
            get => color;
            set => SetProperty<Color>(ref color, value);
        }

        private int currentIndex;

        private List<string> imagesList;

        private void InitImagesList()
        {
            this.imagesList = ImageService.Instance.GetImagesNameForAvatarAsync();
            currentIndex = this.imagesList.IndexOf(ImagePath);
            currentIndex = currentIndex >= 0 ? currentIndex : 0;
        }
        [RelayCommand]
        private Task NextImage()
        {
            currentIndex = currentIndex == this.imagesList.Count - 1 ? 0 : currentIndex + 1;
            ImagePath = imagesList[currentIndex];
            return Task.CompletedTask;
        }
        [RelayCommand]
        private Task PrevImage()
        {
            currentIndex = currentIndex == 0 ? imagesList.Count - 1 : currentIndex - 1;
            ImagePath = imagesList[currentIndex];
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task Confirm()
        {
            if (ImagePath != VM_CurrentUser.Instance.Image)
                EventService.RaiseImChangedPhotoEventAsync(ImagePath);
            if (Color != ColorConverterService.Instance.ConvertStringToColor(VM_CurrentUser.Instance.Color))
                EventService.RaiseImChangedColorEventAsync(Color.ToHex());
            Back();
            return Task.CompletedTask;
        }

        [RelayCommand]
        private void Back()
        {
            _ = NavigationService.Instance.NavigateBackAsync();
        }
    }
}
