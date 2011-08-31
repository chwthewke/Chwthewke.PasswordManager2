using System;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public interface IFuzzyDateFormatter
    {
        string Format(DateTime dateTime);
    }
}