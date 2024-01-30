namespace Gamification
{
    [System.Serializable]
    public class Container
    {
        private ExperienceLevel experienceSystem;
        private ExperienceListener[] experienceListeners;

        public Container(ExperienceListener[] experienceListeners)
        {
            this.experienceListeners = experienceListeners;
            
            foreach (var listener in experienceListeners)
            {
                listener.Activate();
            }
        }
    }
}