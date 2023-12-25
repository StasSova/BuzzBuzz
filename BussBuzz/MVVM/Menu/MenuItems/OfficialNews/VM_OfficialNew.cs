using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzBuzz.MVVM.Menu.MenuItems.OfficialNews
{
    public partial class VM_OfficialNew : ObservableObject
    {
        private VM_OfficialNew() { }
        public VM_OfficialNew(M_OfficialNew model) 
        {
            this.Model = model;
        }
        private M_OfficialNew model;
        public M_OfficialNew Model
        {
            get { return model; }
            set { SetProperty<M_OfficialNew>(ref model, value); }
        }

        public string Title
        {
            get { return Model.Title; }
            set { SetProperty<string>(ref Model.Title, value); }
        }
        public string Description
        {
            get { return Model.Description; }
            set { SetProperty<string>(ref Model.Description, value); }
        }
        public string Date
        {
            get { return Model.Date; }
            set { SetProperty<string>(ref Model.Date, value); }
        }
        public string Image
        {
            get { return Model.Image; }
            set { SetProperty<string>(ref Model.Image, value); }
        }
    }
}
