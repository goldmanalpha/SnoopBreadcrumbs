using System.ComponentModel;
using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace ViewModels
{
    using System.Windows;

    /// <summary>
    /// Base class for ViewModels containing some utility
    /// methods
    /// </summary>
    public abstract class ViewModelBase : DependencyObject, INotifyPropertyChanged
    {
        /// <summary>
        /// Checks whether the values for the property changed
        /// or not, sets the new value and raises a propertychanged
        /// event if it has.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        protected bool CheckPropertyChanged<T>(
            string propertyName,
            ref T oldValue,
            ref T newValue)
        {
            if (propertyName == null)
            {
                throw new ArgumentException("propertyName");
            }

            if (!Equals(oldValue, newValue))
            {
                oldValue = newValue;

                Dispatcher.CurrentDispatcher.Invoke(new Action(
                    () => RaisePropertyChanged(propertyName)));
                return true;
            }

            return false;
        }


        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="name">The parameter name</param>
        protected virtual void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
