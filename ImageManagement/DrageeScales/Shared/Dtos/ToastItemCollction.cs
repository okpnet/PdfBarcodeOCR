using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Dtos
{
    public class ToastItemCollction: ObservableCollection<ToastItem>
    {
        public new async void Add(ToastItem item)
        {
            item.RemoveAction = (t) => this.Remove(t);
            base.Add(item);
            item.ShowToast();
        } 
    }
}
