using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet
{
    /// <summary>
    /// Hidden
    /// - The Drone network is unaware of this resource.
    /// Known
    /// - The Network is aware of this resource.
    /// Reserved
    /// - A Drone is navigating to pickup this resource.
    /// Depleted
    /// - The resource has been used, and is ready to be reset.
    /// </summary>
    public enum ResourcesState { Hidden, Known, Reserved, Depleted }

    public class Resource : MonoBehaviour
    {
        public ResourcesState state = ResourcesState.Hidden;
    }
}
