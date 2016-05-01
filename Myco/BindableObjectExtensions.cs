using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Myco
{
    public static class BindableObjectExtensions
    {
        #region Fields

        private static MethodInfo _setInheritedBindingContext = null;

        #endregion Fields

        #region Methods

        public static void SetInheritedBindingContextReflect(this BindableObject bindable, object value)
        {
            if (_setInheritedBindingContext == null)
            {
                _setInheritedBindingContext = typeof(BindableObject).GetRuntimeMethods().First(x => x.Name == "SetInheritedBindingContext");
            }

            _setInheritedBindingContext.Invoke(null, new object[] { bindable, value });
        }

        #endregion Methods
    }
}