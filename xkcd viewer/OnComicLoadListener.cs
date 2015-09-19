using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xkcd_viewer
{
    public interface OnComicLoadListener
    {
        void OnComicLoaded(XkcdJsonObject comicJson);
    }
}
