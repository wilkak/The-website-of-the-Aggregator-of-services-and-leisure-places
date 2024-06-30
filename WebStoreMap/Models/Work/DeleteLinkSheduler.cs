using Quartz;
using Quartz.Impl;

namespace WebStoreMap.Models.Work
{
    public static class DeleteLinkSheduler
    {
        public static async void Start()
        {
            IScheduler Scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            
            await Scheduler.Start();

            IJobDetail Job = JobBuilder.Create<DeleteLink>().Build();

            ITrigger Trigger = TriggerBuilder.Create()  // создаем триггер
                .WithIdentity("trigger1", "group1")     // идентифицируем триггер с именем и группой
                .StartNow()                            // запуск сразу после начала выполнения
                .WithSimpleSchedule(x => x          // настраиваем выполнение действия
                    .WithIntervalInHours(24)          // через 24 часа
                    .RepeatForever())                   // бесконечное повторение
                .Build();                               // создаем триггер

            _ = await Scheduler.ScheduleJob(Job, Trigger);        // начинаем выполнение работы
        }
    }
}