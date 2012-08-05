using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrumbLib
{
    public class AssemblyHelper
    {


        /// <summary>
        /// all framework elements can have a tag and since they're ubiquitous
        /// they will suffice
        /// </summary>
        public IEnumerable<Type> GetFrameworkElements()
        {
            var list = new List<Type>();

            var assembly = typeof(System.Windows.Controls.TextBlock).Assembly; // etc
            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType != null)
                {
                    bool isFE = type.IsSubclassOf(typeof(System.Windows.FrameworkElement));

                    if (isFE)
                    {
                        list.Add(type);
                    }
                }
            }

            list = list.OrderBy(f => f.Name).ToList();

            return list;
        }
    }
}
