using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;

namespace PierogiesBot.Discord.Infrastructure;

internal class DependencyInjectionJobFactory : PropertySettingJobFactory
{
    private readonly IServiceProvider _serviceProvider;

    public DependencyInjectionJobFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        var job = _serviceProvider.GetRequiredService(bundle.JobDetail.JobType);
        SetObjectProperties(job, bundle.JobDetail.JobDataMap);
        return (IJob) job;
    }
}