using UnityEngine;

public enum Role
{
    Sheriff,
    Badguy,
    Player,
    Innocent
}

public class RoleHandler : MonoBehaviour
{
    public Role role;
}
