using BussBuzz.Base;
using BussBuzz.Services;
using BuzzBuzz.MVVM.Menu.MenuItems.OfficialNews.Details;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzBuzz.MVVM.Menu.MenuItems.OfficialNews
{
    public partial class VM_OfficialNews : ObservableObject, VM_Base
    {
        public VM_OfficialNews() 
        {
            _ = InitOfficialNews();
        }
        public async Task InitOfficialNews()
        {
            try
            {
                lock (NewsCollection)
                {
                    NewsCollection.Clear();
                }
                List<M_OfficialNew> news = DatabaseService.Instance.GetOfficialNews();
                foreach (M_OfficialNew n in news)
                {
                    await AddNews(n);
                }
            }
            catch (Exception ex) { }
        }
        public ObservableCollection<VM_OfficialNew> NewsCollection { get; set; } = new ();
        public Task AddNews(VM_OfficialNew News)
        {
            try
            {
                addnews(News);
            }
            catch { }
            return Task.CompletedTask;
        }
        public Task AddNews(M_OfficialNew News)
        {
            try
            {
                addnews(new VM_OfficialNew(News));
            }
            catch { }
            return Task.CompletedTask;
        }
        private Task addnews(VM_OfficialNew news)
        {
            lock (NewsCollection)
            {
                NewsCollection.Insert(0, news);
            }
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToDetails(object News)
        {
            try
            {
                NavigationService.Instance.NavigateToAsync<VM_OfficialNewDetails>(News);
            }
            catch { }
            return Task.CompletedTask;
        }
    }
}
