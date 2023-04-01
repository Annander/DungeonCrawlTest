public class ExperienceLevel {
    // This is the basic balancing tool, used to calculate values for XP rewards
    private int baseXP;

    // Total XP is used to calculate your current level on return
    private int totalXP;

    // Current experience level
    private int currentLevel;

    // Current experience, since last level up
    private int currentXP;

    // Temporarily stores xp that you just gained
    private int xpGain;

    public ExperienceLevel(int baseXP, int totalXP) {
        this.baseXP = baseXP;
        this.totalXP = totalXP;

        CalcCurrentLevel();
    }

    public bool AddXP(int xp) {
        xpGain += xp;
        return CheckLevelUp();
    }

    public int CurrentLevel {
        get { return currentLevel; }
    }

    public int CurrentXP {
        get { return currentXP; }
    }

    public int NextLevel {
        get { return TargetXP(currentLevel); }
    }

    public float Percentile {
        get { return (float)currentXP / NextLevel; }
    }

    private bool CheckLevelUp() {
        bool didLevelUp = false;

        if (xpGain > 0) {
            int xpcalc = currentXP + xpGain;

            if (xpcalc >= TargetXP(currentLevel)) {
                while (xpcalc >= TargetXP(currentLevel)) {
                    currentXP = xpcalc - TargetXP(currentLevel);
                    ++currentLevel;
                }

                didLevelUp = true;
            } else {
                currentXP += xpGain;
            }

            totalXP += xpGain;
            xpGain = 0;
        }

        return didLevelUp;
    }

    // Basic level target XP formula, using the BaseXP value, which is also used as basis in rewards
    private int TargetXP(int level) {
        return baseXP * level * (level + 1);
    }

    // This makes sure that leveling up is deterministic and means that only level cap and total XP must be stored
    private void CalcCurrentLevel() {
        int xpcalc = totalXP;
        int currentCalc = xpcalc;

        while (xpcalc >= TargetXP(currentLevel)) {
            currentCalc -= TargetXP(currentLevel);
            ++currentLevel;
        }

        currentXP = currentCalc;
    }
}