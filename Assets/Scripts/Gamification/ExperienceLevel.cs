namespace Gamification
{
    public class ExperienceLevel
    {
        public event LeveledUp OnLevelUp;
        public delegate void LeveledUp(int level);
        
        // This is the basic balancing tool, used to calculate values for XP rewards
        private readonly int baseXp;

        // Total XP is used to calculate your current level on return
        private int totalXp;

        // Current experience level
        private int currentLevel;

        // Current experience, since last level up
        private int currentXp;

        // Temporarily stores xp that you just gained
        private int xpGain;

        public ExperienceLevel(int baseXp, int totalXp) 
        {
            this.baseXp = baseXp;
            this.totalXp = totalXp;

            CalcCurrentLevel();
        }

        public bool AddXp(int xp) 
        {
            xpGain += xp;
            return CheckLevelUp();
        }

        public int CurrentLevel => currentLevel;
        public int CurrentXp => currentXp;
        public int NextLevel => TargetXp(currentLevel);
        public float Percentile => (float)currentXp / NextLevel;

        private bool CheckLevelUp() 
        {
            var didLevelUp = false;

            if (xpGain > 0) 
            {
                int calc = currentXp + xpGain;

                if (calc >= TargetXp(currentLevel)) 
                {
                    while (calc >= TargetXp(currentLevel)) 
                    {
                        currentXp = calc - TargetXp(currentLevel);
                        ++currentLevel;

                        OnLevelUp?.Invoke(currentLevel);
                    }

                    didLevelUp = true;
                } 
                else 
                {
                    currentXp += xpGain;
                }

                totalXp += xpGain;
                xpGain = 0;
            }

            return didLevelUp;
        }

        // Basic level target XP formula, using the BaseXP value, which is also used as basis in rewards
        private int TargetXp(int level) 
        {
            return baseXp * level * (level + 1);
        }

        // This makes sure that leveling up is deterministic and means that only level cap and total XP must be stored
        private void CalcCurrentLevel() 
        {
            var calc = totalXp;
            var currentCalc = calc;

            while (calc >= TargetXp(currentLevel)) 
            {
                currentCalc -= TargetXp(currentLevel);
                ++currentLevel;
            }

            currentXp = currentCalc;
        }
    }
}