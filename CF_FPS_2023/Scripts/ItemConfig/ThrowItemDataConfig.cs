using Assets.Resolution.Scripts.Weapon;
using UnityEngine;

[CreateAssetMenu(fileName = "New_ThrowItemSO", menuName = "CreateWeaponSO/CreateThrowItemSO")]
public class ThrowItemDataConfig : WeaponDataConfig
{
    public ThrowItemType throwItemType;
    public GameObject ThrowItemEntity;//投掷物实体
    public override void OnEnable()
    {
        cellIndex = weaponType - WeaponType.Primarily + throwItemType-ThrowItemType.Explosion;
    }
}
