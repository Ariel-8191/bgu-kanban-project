using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Frontend
{
    /// <summary>
    /// A base class for all view models, providing an implementation of
    /// <see cref="INotifyPropertyChanged"/> so that data-bound views are notified when
    /// a property value changes.
    /// </summary>
    public abstract class Notifiable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the given property.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that changed. When called from within a property setter,
        /// this is supplied automatically by the compiler.
        /// </param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
