using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace QueueApp
{
    class Program
    {
        // решил использовать потокобезопасную очередь для работы паралельно с 3 мя окнами(потоками)
        static ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
        // услуги
        static int[] serviceTimes = { 5, 7, 10, 15 };
        // окна обслуживания 
        static int[] windows = { 1, 2, 3 };
        // начальный билет
        static int numberOfTicket = 1;

        static void Main(string[] args)
        {
            // Запускаем потоки для каждого окна
            foreach (var window in windows)
            {
                new Thread(() => ServeCustomers(window)).Start();
            }

            // Добавляем талоны в очередь выбирая наименьший из доступных в очереди(не в процессе обслуживания)
            for (int i = 0; i < 8 * 60 / serviceTimes.Min(); i++)
            {
                foreach (var serviceTime in serviceTimes)
                {
                    queue.Enqueue(serviceTime);
                }
            }

            // Ждем завершения работы всех окон
            while (!queue.IsEmpty)
            {
                Thread.Sleep(100);
            }

            Console.WriteLine("Очередь пуста. Работа завершена.");
            Console.ReadLine();
        }

        static void ServeCustomers(int window)
        {
            while (true)
            {
                // Забираем с очереди услугу
                if (queue.TryDequeue(out int serviceTime))
                {
                    
                    Console.WriteLine($"Окно {window}, талон {numberOfTicket++}: Обслуживание клиента на {serviceTime} минут. \n" +
                                      $"Начало обработки талона {DateTime.Now} \n" +
                                      $"Окончание обработки талона {DateTime.Now.AddMinutes(serviceTime)}");
                    Console.WriteLine();
                    Thread.Sleep(serviceTime); // Здесь надо минуты в миллисекунды * 60000, НО ДЛЯ СИМУЛЯЦИИ не умножаю
                    
                }
            }
        }
    }

}