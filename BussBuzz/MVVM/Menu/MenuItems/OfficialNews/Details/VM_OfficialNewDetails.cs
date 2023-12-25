using BussBuzz.Base;
using BussBuzz.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzBuzz.MVVM.Menu.MenuItems.OfficialNews.Details
{
    public partial class VM_OfficialNewDetails: ObservableObject, VM_Base
    {
        private M_OfficialNewDetails model;
        public Task InitializeAsync(object News)
        {
            try
            {
                if (News is VM_OfficialNew)
                {
                    // Инициализация детали новостей из БД
                    model = DatabaseService.Instance.GetDetailsNew(News as VM_OfficialNew);
                    Title = model.Title;
                    Description = model.Description;
                    Date = model.Date;
                    Image = model.Image;
                }
            }
            catch { }
            return Task.CompletedTask;
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty<string>(ref title, value); }
        }
        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty<string>(ref description, value); }
        }

        private string date;
        public string Date
        {
            get { return date; }
            set { SetProperty<string>(ref date, value); }
        }
        private string image;
        public string Image
        {
            get { return image; }
            set { SetProperty<string>(ref image, value); }
        }
    }
}
