using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HeroSelection : MonoBehaviour
{
    [System.Serializable]
    public class HeroButton
    {
        public Button button;
        public string heroName;
    }

    public HeroButton[] heroButtons;

    void Start()
    {
        foreach (var hero in heroButtons)
        {
            hero.button.onClick.AddListener(() => SelectHero(hero));
        }
    }

    void SelectHero(HeroButton hero)
    {
        // 保存选中的英雄名称
        GeneralDataManager.SelectedHeroName = hero.heroName;
        Debug.Log($"已选择英雄: {hero.heroName}");

 
    }


}