using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net.Http;
using SvgNet;   // мб через нее можно конвертировать svg в png

namespace Weather
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        class WeatherInfo
        {
            public string now { get; set; }             // юникс тайм
            public Fact fact{ get; set; }               // фактические значения
            public Forecasts[] forecasts { get; set; }  // прогнозы
        }
        class Fact
        {
            public short temp { get; set; }             // температура
            public string icon { get; set; }            // код иконки
        }
        class Forecasts
        {
            public string date { get; set; }            // дата прогноза 
            public Parts parts { get; set; }            // части суток
        }
        class Parts
        {
            public Day_short day_short { get; set; }    // прогноз на день
            public Night_short night_short { get; set; }// прогноз на ночь
        }
        class Day_short
        {
            public short temp { get; set; }            // максимальная темпа за день
        }
        class Night_short
        {
            public short temp { get; set; }            // минимальная темпа за ночь
        }
        private DateTime stringToDateTime(string date)
        {
            int year = Int32.Parse(date.Substring(0,4));    // получаем год
            int month = Int32.Parse(date.Substring(5,2));   // месяц
            int day = Int32.Parse(date.Substring(8,2));     // и день из json структуры
            return new DateTime(year, month, day);
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();                       // создаем клиента
            HttpRequestMessage request = new HttpRequestMessage();      // создаем запрос для клиента

            request.RequestUri = new Uri("https://api.weather.yandex.ru/v1/" +              // ссылка на апишку
                "forecast?" +       // тестовый комплект услуг от Я.Погоды
                "lat=48.48272&" +   // широта
                "lon=135.08379&" +  // долгота
                "limit=3&" +        // лимит прогноза
                "extra=false");     // доп. инфа           
            request.Method = HttpMethod.Get;                                                // хз че эт
            request.Headers.Add("X-Yandex-API-Key", "dcaa56be-ef4e-458e-bec6-3390de3f977b");// имя заголовка и его значение (ключ из ЛК)

            HttpResponseMessage response = await client.SendAsync(request); // кидаем запрос и ждем ответ
            
            if (response.StatusCode == System.Net.HttpStatusCode.OK)        // чекаем код статуса
            {
                checkBox1.Checked = true;                                   // подтверждаем соединение
                HttpContent responseContent = response.Content;             // получаем контент из ответа
                var json = await responseContent.ReadAsStringAsync();       // считываем контент в json
                var obj = JsonConvert.DeserializeObject<WeatherInfo>(json); // ковентируем json по маске класса WeatherInfo
                
                label1.Text = obj.fact.temp.ToString() + "°";                                           // сохраняем градусы в лейбл и добавляем символ градусов
                //string imageLocation = "https://yastatic.net/weather/i/icons/blueye/color/svg/.svg";  // попытка сохранить svg в pictureBox
                //imageLocation = imageLocation.Insert(54, obj.fact.icon);                              // вставляем код изображения погоды в ссылку
                //pictureBox1.ImageLocation = imageLocation;                                            // осталось сконвертировать svg в png

                //richTextBox1.Text = JsonConvert.DeserializeObject(json).ToString();

                string[] days = new string[7] { "Воскресенье", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота"};

                // записываем прогноз на три дня вперед
                label3.Text = days[(int)(stringToDateTime(obj.forecasts[0].date).DayOfWeek)];
                label4.Text = obj.forecasts[0].parts.day_short.temp.ToString() + "°";
                label5.Text = obj.forecasts[0].parts.night_short.temp.ToString() + "°";
         
                label6.Text = days[(int)(stringToDateTime(obj.forecasts[1].date).DayOfWeek)];
                label7.Text = obj.forecasts[1].parts.day_short.temp.ToString() + "°";
                label8.Text = obj.forecasts[1].parts.night_short.temp.ToString() + "°";

                label9.Text = days[(int)(stringToDateTime(obj.forecasts[2].date).DayOfWeek)];
                label10.Text = obj.forecasts[2].parts.day_short.temp.ToString() + "°";
                label11.Text = obj.forecasts[2].parts.night_short.temp.ToString() + "°";
            }
            else
            {
                checkBox1.Checked = false;
                MessageBox.Show("Нет соединения");
            }
        }
    }
}
