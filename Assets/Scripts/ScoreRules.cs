using UnityEngine;

public static class ScoreRules
{
    public static int GetHitScore(Role targetRole)
    {
        switch (targetRole)
        {
            case Role.Sheriff: return +2;
            case Role.Badguy: return +1;
            case Role.Player: return +2;
            case Role.Innocent: return -2;
            default: return 0;
        }
    }

    public static int GetHitByPenalty(Role myRole)
    {
        switch (myRole)
        {
            case Role.Sheriff: return -2;
            case Role.Badguy: return -1;
            case Role.Player: return -2;
            case Role.Innocent: return 0; // not listed → assume 0
            default: return 0;
        }
    }

    public static int GetKillScore(Role targetRole)
    {
        switch (targetRole)
        {
            case Role.Sheriff: return +50;
            case Role.Badguy: return +3;
            case Role.Innocent: return -10;
            case Role.Player: return 0; // Players can't be killed
            default: return 0;
        }
    }
}
