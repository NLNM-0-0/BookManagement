using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement{
    public interface INavigationService {
        void Navigate();
        void Navigate(object o);
        void NoBackNavigate();
        void NoBackNavigate(object o);

        Type GetViewModel();
    }
}
