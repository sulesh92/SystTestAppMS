using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Data;

namespace TestAppSysTech
{
    public class CommonTools
    {
        public static async void ShowMessageAsync(string message)
        {
            var messageDialog = new MessageDialog(message);
            await messageDialog.ShowAsync();
        }


        public static Person FindPersonById(int personId)
        {
            Person requestdPerson = new Person();

            using (DataModelContext context = new DataModelContext())
            {
                List<Group> groups = context.Groups.ToList();
                List<Subordinate> subordinates = context.Subordinates.ToList();
                List<Salary> salaries = context.Salaries.ToList();

                requestdPerson = context.Persons.Find(personId);
            }

            return requestdPerson;
        }

    }

    /// <summary>
    /// Возвращает дату в указанном формате. Необходимо для настройки
    /// отоборажения даты на страницах из БД
    /// </summary>
    public class DateTimeOffsetToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTimeOffset sourceTime = (DateTimeOffset)value;
            DateTime targetTime = sourceTime.DateTime;
            return targetTime.ToString("dd.MM.yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            DateTimeOffset resultTime = DateTime.Parse(value.ToString());
            return resultTime;
        }


    }




}
