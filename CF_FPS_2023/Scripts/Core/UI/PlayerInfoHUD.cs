using Assets.Resolution.Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerInfoHUD : MonoBehaviour
{


    #region WeaponPanel--Data
    public GameObject WeaponPanel;
    

    public UIGrid InventoryPanel;//runtime
    private UIWidget InventoryPanelWidget;
    private List<UISprite> weaponCells;
    public UIGrid throwItemGrid;
    private List<UISprite> throwItemCells;
    public Color unUsedStatus;
    public Color UsedStatus;
    public int cellheight;
    public int cellspace;
    public float openInventoryPanelDuration = 1;
    public float inventoryPanelShowDuration = 2;
    public float hideInventoryPanelDuration = 1;
    public Sequence inventoryStatusAnimation;
    //Current Weapon
    private int maxAmmoCount;
    public UILabel currentAmmoLabel;
    public UILabel MaxAmmoLabel;
    public UIGrid ammoEntityGrid;
    private UISprite extraAmmoUi;
    private int tid_extraAmmoHideTask=-1;
    //卸载弹夹UI变化的时间
    public float unEquipClip;
    //换弹UI变化的时间
    public float reloadTime;
    private IEnumerator reloadAnimationCor;
    private bool reducedDefaultAmmo=false;
    private int showAmmoEntityUICount = 5;
    public Color ammoWillEmptyLabelColor;
    public float ammoWillEmptyLabelScale;
    public float ammoLabelColorBreathDuration=0.5f;
    public float ammoLabelScaleBreathDuration=0.5f;
    private Sequence ammoLabelTween;
    private Color ammoLabelCommonColor;
    #endregion

    #region RoleStatus--Data
    public UISprite hpBar;
    public UILabel hvLabel;

    public UISprite defendBar;
    public UILabel dvLabel;
    public int MaxHPValue { get; set; }
    public int MaxDefendValue { get; set; }    
    #endregion

    public void Awake()
    {
        int childCount = InventoryPanel.transform.childCount;
        if (weaponCells==null)
        {
            weaponCells = new List<UISprite>(childCount);
        }
        for (int i = 0; i < childCount; i++)
        {
            weaponCells.Add(InventoryPanel.transform.GetChild(i).GetComponent<UISprite>());
        }

        childCount=throwItemGrid.transform.childCount;
        if (throwItemCells == null)
        {
            throwItemCells = new List<UISprite>(childCount);
        }
        for (int i = 0; i < childCount; i++)
        {
            throwItemCells.Add(throwItemGrid.transform.GetChild(i).GetComponent<UISprite>());
        }
        InventoryPanel.cellHeight = cellheight + cellspace;
        InventoryPanel.Reposition();
        throwItemGrid.Reposition();
        extraAmmoUi = ammoEntityGrid.transform.GetChild(ammoEntityGrid.transform.childCount-1).GetComponent<UISprite>();
        ammoLabelCommonColor = currentAmmoLabel.color;
        InventoryPanelWidget = InventoryPanel.GetComponent<UIWidget>();
    }

    #region WeaponHUD
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index">[0,2]</param>
    public void SwitchWeapon(int index)
    {
        //WeaponType weaponType = weaponDataConfig.weaponType;
        //int index = weaponType-WeaponType.Primarily;
        InventoryPanelWidget.gameObject.SetActive(true);
        InventoryPanelWidget.alpha = 0;
        if (inventoryStatusAnimation!=null)
        {
            inventoryStatusAnimation.Restart();
        }
        else
        {
            inventoryStatusAnimation = DOTween.Sequence();
            inventoryStatusAnimation.Append(DOTween.To(() => InventoryPanelWidget.alpha, (v) => { InventoryPanelWidget.alpha = v; }, 1, openInventoryPanelDuration));
            inventoryStatusAnimation.Append(DOTween.To(() => InventoryPanelWidget.alpha, (v) => { InventoryPanelWidget.alpha = v; }, 0, hideInventoryPanelDuration).SetDelay(inventoryPanelShowDuration));
            inventoryStatusAnimation.onComplete = () => { InventoryPanelWidget.gameObject.SetActive(false); };
            inventoryStatusAnimation.SetAutoKill(false);
        }

        if (ammoLabelTween!=null)
        {
            ammoLabelTween.Pause();
        }
        
        currentAmmoLabel.color = ammoLabelCommonColor;
        currentAmmoLabel.transform.localScale = Vector3.one;

        if (index < 0)
        {
            Debug.LogError("武器HUD索引输入有误");
            return;
        }
        if (index < 3)
        {
            UpdateIconColor(weaponCells,index);
            SetAllUnUsed(throwItemCells);
        }
        else if (index < 6)
        {
            SetAllUnUsed(weaponCells);
            UpdateIconColor(throwItemCells,index - 3);
        }
        else
        {
            Debug.LogError("武器HUD索引输入有误");
            return;
        }

    }
    private void SetAllUnUsed(List<UISprite> cells)
    {
        int childCount = cells.Count;
        for (int i = 0; i < childCount; i++)
        {
            UISprite ui_sprite = cells[i];
            if (ui_sprite.gameObject.activeInHierarchy == false)
            {
                continue;
            }
            ui_sprite.color = unUsedStatus;
        }
    }
    private void UpdateIconColor(List<UISprite> cells,int localIndex)
    {
        int childCount = cells.Count;
        if (childCount <= localIndex)
        {
            Debug.LogError("超出索引");
            return;
        }
        for (int i = 0; i < childCount; i++)
        {
            UISprite ui_sprite = cells[i];
            if (ui_sprite.gameObject.activeInHierarchy==false)
            {
                continue;
            }
            if (localIndex == i)
            {
                ui_sprite.color = UsedStatus;
            }
            else
            {
                ui_sprite.color = unUsedStatus;
            }
        }
        
    }
    
    public void UpdateWeaponHUD(int[] haveWeapon)
    {
        foreach (var item in weaponCells)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in throwItemCells)
        {
            item.gameObject.SetActive(false);
        }
        bool isHaveThrowItem = false;
        foreach (var item in haveWeapon)
        {
            int index = item;
            if (index < 0)
            {
                Debug.LogError("武器HUD索引输入有误");
                continue;
            }
            if (index < 3)
            {
                weaponCells[index].gameObject.SetActive(true);
            }
            else if (index < 6)
            {
                isHaveThrowItem = true;
                throwItemCells[index - 3].gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("武器HUD索引输入有误");
                continue;
            }
        }
        if (isHaveThrowItem)
        {
            weaponCells[3].gameObject.SetActive(true);
        }
        InventoryPanel.Reposition();
        throwItemGrid.Reposition();
    }
    public void UpdateWeaponIcon(int index,string icon_name)
    {
        if (index<0)
        {
            Debug.LogError("武器HUD索引输入有误");
            return;
        }
        if (index<3)
        {
            UISprite sprite=weaponCells[index];
            sprite.spriteName = icon_name;
            Vector2 rect = GetUISpriteRect(sprite);
            sprite.keepAspectRatio = UIWidget.AspectRatioSource.Free;
            int multiply = (int)rect.y / cellheight;
            int width = (int)rect.x / multiply;
            sprite.width = width;
            sprite.height = cellheight;
            sprite.SetDirty();
        }
        else if (index<6)
        {
            throwItemCells[index-3].spriteName = icon_name;
        }
        else
        {
            Debug.LogError("武器HUD索引输入有误");
            return;
        }
    }

    public void SetMaxAmmo(int maxAmmoCount)
    {
        this.maxAmmoCount = maxAmmoCount;
        MaxAmmoLabel.text = "/"+maxAmmoCount.ToString();
    }
    public void UpdateAmmo(int ammoCount,bool isAutoShowCells=false)
    {
        int childCount = ammoEntityGrid.transform.childCount;
        if (isAutoShowCells)
        {
            if (ammoCount >= showAmmoEntityUICount)
            {
                reducedDefaultAmmo = false;
                if (ammoLabelTween != null)
                {
                    ammoLabelTween.Pause();
                }
                currentAmmoLabel.color = ammoLabelCommonColor;
                currentAmmoLabel.transform.localScale = Vector3.one;
            }
            else
            {
                if (ammoLabelTween != null)
                {
                    ammoLabelTween.Play();
                }
            }
            for (int i = 0; i < childCount - 1; i++)
            {
                ammoEntityGrid.transform.GetChild(i).gameObject.SetActive(i < ammoCount);
            }
            ammoEntityGrid.Reposition();
        }
        


        if (reloadAnimationCor != null)
        {
            StopCoroutine(reloadAnimationCor);
            reloadAnimationCor = null;
        }
        currentAmmoLabel.text = ammoCount.ToString();
    }
    public void UpdateAmmoWhenReduce(int currentAmmoCount)
    {
        UpdateAmmo(currentAmmoCount);
        //Effect
        if (currentAmmoCount >= showAmmoEntityUICount)
        {
            if (tid_extraAmmoHideTask == -1)
            {
                extraAmmoUi.gameObject.SetActive(true);
                ammoEntityGrid.pivot = UIWidget.Pivot.Center;
                ammoEntityGrid.Reposition();
                tid_extraAmmoHideTask = TimeSystem.Instance.AddTimeTask(100, () =>
                {
                    ammoEntityGrid.pivot = UIWidget.Pivot.Right;
                    extraAmmoUi.gameObject.SetActive(false);
                    ammoEntityGrid.Reposition();
                    TimeSystem.Instance.AddTimeTask(50,
                       ()=> { tid_extraAmmoHideTask = -1;} , PETime.PETimeUnit.MillSeconds);
                }, PETime.PETimeUnit.MillSeconds);
            }
            
        }
        else
        {
            Transform bulletTrans = ammoEntityGrid.transform.GetChild(currentAmmoCount);
            UIWidget bulletSprite = bulletTrans.GetComponent<UIWidget>();
            DOTween.To(() => bulletSprite.alpha, (v) => { bulletSprite.alpha = v; }, 0, 0.2f).onComplete = () => { 
                ammoEntityGrid.pivot = UIWidget.Pivot.Right;
                bulletSprite.alpha = 1; bulletTrans.gameObject.SetActive(false);ammoEntityGrid.Reposition();};
            reducedDefaultAmmo = true;
            if (currentAmmoCount == 0)
            {
                currentAmmoLabel.color = ammoWillEmptyLabelColor;
                currentAmmoLabel.transform.localScale = Vector3.one;
            }
            else
            {
                if (ammoLabelTween != null)
                {
                    ammoLabelTween.Play();
                }
                else
                {
                    ammoLabelTween = DOTween.Sequence();
                    ammoLabelTween.Append(DOTween.To(() => currentAmmoLabel.color, (targetColor) => { currentAmmoLabel.color = targetColor; }, ammoWillEmptyLabelColor, ammoLabelColorBreathDuration));
                    ammoLabelTween.Join(DOTween.To(() => currentAmmoLabel.transform.localScale.x, (targetScale) => { currentAmmoLabel.transform.localScale = new Vector3(targetScale, targetScale, currentAmmoLabel.transform.localScale.z); }, ammoWillEmptyLabelScale, ammoLabelScaleBreathDuration));
                    ammoLabelTween.SetLoops(-1,LoopType.Yoyo);
                    ammoLabelTween.SetAutoKill(false);
                }
            }
        }
        
    }
    public void ReLoad()
    {
        UpdateAmmo(0);
        if (reloadAnimationCor!=null)
        {
            StopCoroutine(reloadAnimationCor);
            reloadAnimationCor = null;
        }
        reloadAnimationCor = StartReloadUiAnimation();
        StartCoroutine(reloadAnimationCor);
        
    }
    public IEnumerator StartReloadUiAnimation()
    {
        Transform ammoEntityGridTrans = ammoEntityGrid.transform;
        int childCount = ammoEntityGridTrans.childCount;
        float intervalTime = unEquipClip / (childCount - 1);
        ammoEntityGridTrans.GetChild(childCount - 1).gameObject.SetActive(false);
        for (int i = childCount - 2; i >= 0; --i)
        {
            ammoEntityGridTrans.GetChild(i).gameObject.SetActive(false);
            yield return new WaitForSeconds(intervalTime);
        }
        intervalTime = reloadTime / (childCount - 1);
        for (int i = 0; i < childCount-1; ++i)
        {
            yield return new WaitForSeconds(intervalTime);
            ammoEntityGridTrans.GetChild(i).gameObject.SetActive(true);
            ammoEntityGrid.Reposition();
        }
    }
    public void ShowAmmoCellUI()
    {
        Transform ammoEntityGridTrans = ammoEntityGrid.transform;
        int childCount = ammoEntityGridTrans.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            ammoEntityGridTrans.GetChild(i).gameObject.SetActive(true);
        }
        ammoEntityGrid.Reposition();
        reducedDefaultAmmo = false;
    }
    public void HideAmmoCellUI()
    {
        Transform ammoEntityGridTrans = ammoEntityGrid.transform;
        int childCount = ammoEntityGridTrans.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            ammoEntityGridTrans.GetChild(i).gameObject.SetActive(false);
        }
        reducedDefaultAmmo = true;
    }
#endregion

#region HPStatus

    public void UpdateHPStatusUI(int hp)
    {
        int value = Mathf.Clamp(hp,0,MaxHPValue);
        float progress = value * 1.0f / MaxHPValue;
        hpBar.fillAmount = progress;
        hvLabel.text = value.ToString();
    }
    public void UpdateDefendStatusUI(int defend)
    {
       int value = Mathf.Clamp(defend,0,MaxDefendValue);
        float progress = value * 1.0f / MaxHPValue;
        hpBar.fillAmount = progress;
        hvLabel.text = value.ToString();
    }
#endregion

    public Vector2 GetUISpriteRect(UISprite sprite)
    {
        var spriteData = sprite.GetAtlasSprite();
        return new Vector2( (spriteData.width+spriteData.paddingLeft+spriteData.paddingRight), (spriteData.height + spriteData.paddingBottom + spriteData.paddingTop));
    }
}

