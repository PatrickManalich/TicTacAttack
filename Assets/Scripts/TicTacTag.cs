using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Public enum class for globally shared enums. This is useful for addinging multiple tags to a tic tac. */
public class TicTacTag : MonoBehaviour {

    public enum Flavor {
        Orange,
        Mint,
        PeachAndPassionFruit,
        Spearmint
    }

    public enum PathRoute {
        Melee,
        Ranged
    }

    public enum PathDistance {
        None,
        Close,
        Mid,
        Far
    }
}
