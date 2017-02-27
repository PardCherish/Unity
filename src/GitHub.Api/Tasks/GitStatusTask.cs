using System;

namespace GitHub.Unity
{
    class GitStatusTask : GitTask
    {
        private readonly ITaskResultDispatcher<GitStatus> resultDispatcher;
        private readonly StatusOutputProcessor processor;
        private GitStatus gitStatus;

        public GitStatusTask(IEnvironment environment, IProcessManager processManager, ITaskResultDispatcher<GitStatus> resultDispatcher,
                IGitObjectFactory gitObjectFactory)
            : base(environment, processManager)
        {
            this.resultDispatcher = resultDispatcher;
            processor = new StatusOutputProcessor(gitObjectFactory);
        }

        protected override ProcessOutputManager HookupOutput(IProcess process)
        {
            processor.OnStatus += status => {
                gitStatus = status;
//                Logger.Debug("GOT STATUS {0}", gitStatus);
                resultDispatcher.ReportSuccess(gitStatus);
            };
            return new ProcessOutputManager(process, processor);
        }

        protected override void RaiseOnSuccess()
        {
            //Logger.Debug("RAISING STATUS {0}", gitStatus);
        }

        public override bool Blocking { get { return false; } }
        public override TaskQueueSetting Queued { get { return TaskQueueSetting.QueueSingle; } }
        public override bool Critical { get { return false; } }
        public override bool Cached { get { return false; } }
        public override string Label { get { return "git status"; } }

        protected override string ProcessArguments
        {
            get { return "status -b -u --porcelain"; }
        }
    }
}