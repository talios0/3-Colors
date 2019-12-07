using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StateReciever {
    private static States state;

    public static States GetState() {
        return state;
    }

    public static void SetState(States _state) {
        state = _state;
    }
}
