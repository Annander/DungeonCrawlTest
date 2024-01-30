namespace Gamification
{
    public abstract class ExperienceListener
    {
        private readonly ExperienceLevel experienceLevel;
        
        public ExperienceListener(ExperienceLevel experienceLevel)
        {
            this.experienceLevel = experienceLevel;
        }

        public virtual void Activate()
        {
            experienceLevel.OnLevelUp += OnLevelUp;
        }
        
        public virtual void Deactivate()
        {
            experienceLevel.OnLevelUp -= OnLevelUp;
        }

        protected virtual void OnLevelUp(int level) {}
    }
}