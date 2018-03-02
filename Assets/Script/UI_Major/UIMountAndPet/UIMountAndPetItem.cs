using UnityEngine;
using System.Collections;

public class UIMountAndPetItem : GUISingleItemList
{
    public GUISingleButton icon;
    public GUISingleSprite black;
    public GUISingleSprite pitchOn;
    public GUISingleSprite lockSprite;
    public GUISingleLabel iconState;
    private UIMountNode mountNode;
    private UIPetNode petNode;
    protected override void InitItem()
    {
       icon.onClick = OnIconClick;
       
    }
    public override void Info(object obj)
    {
        if (MountAndPetNodeData.Instance().ShowType)
        {
            if (obj!= null)
            {
                mountNode = (UIMountNode)obj;
                //判断是否有坐骑 在没有坐骑的情况下取表里第一个坐骑显示和被选中的状态
                icon.spriteName = mountNode.icon_name;


                // 先判断是否拥有
                //再判断是否使用
                if (MountAndPetNodeData.Instance().IsHaveThisMount(mountNode.mount_id))
                {
                    if (MountAndPetNodeData.Instance().currentMountID != mountNode.mount_id)
                    {
                        black.gameObject.SetActive(false);
                        lockSprite.gameObject.SetActive(false);
                        iconState.gameObject.SetActive(false);
                    }
                    else
                    {
                        black.gameObject.SetActive(false);
                        lockSprite.gameObject.SetActive(false);
                        iconState.gameObject.SetActive(true);
                    }
                }
                else
                {
                    black.gameObject.SetActive(true);
                    lockSprite.gameObject.SetActive(true);
                    iconState.gameObject.SetActive(false);
                }

            }
        }
        else
        {
            if (obj != null)
            {
                petNode = (UIPetNode)obj;
                //判断是否有宠物 在没有宠物的情况下取表里第一个坐骑显示和被选中的状态
                icon.spriteName = petNode.icon_name;
                if (MountAndPetNodeData.Instance().IsHaveThisPet(petNode.pet_id))
                {
                    if (MountAndPetNodeData.Instance().currentPetID != petNode.pet_id)
                    {
                        black.gameObject.SetActive(false);
                        lockSprite.gameObject.SetActive(false);
                        iconState.gameObject.SetActive(false);
                    }
                    else
                    {
                        black.gameObject.SetActive(false);
                        lockSprite.gameObject.SetActive(false);
                        iconState.gameObject.SetActive(true);
                    }
                }
                else
                {
                    black.gameObject.SetActive(true);
                    lockSprite.gameObject.SetActive(true);
                    iconState.gameObject.SetActive(false);
                }
            }
        }
    }
    private void OnIconClick()
    {
        MountAndPetNodeData.Instance().seletIndex = index;
        if (MountAndPetNodeData.Instance().ShowType)//true坐骑/false宠物
        {
            if (mountNode!=null)
            {
                UIMountAndPet.Instance.SetInfo(mountNode, MountAndPet.Mount);
                

            }
            // UIMountAndPet.Instance().InsHero(mountNode.icon_name);
        }
        else
        {
            if (petNode!=null)
            {
                UIMountAndPet.Instance.SetInfo(petNode, MountAndPet.Pet);
            }
        }
    }
    public void Update()
    {
        if (index == MountAndPetNodeData.Instance().seletIndex)
        {
            pitchOn.gameObject.SetActive(true);
        }
        else
        {
            pitchOn.gameObject.SetActive(false);
        }
    }
}