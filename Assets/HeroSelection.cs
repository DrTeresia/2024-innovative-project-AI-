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
        //public Sprite heroSprite;
    }

    public HeroButton[] heroButtons;
    //public Image selectedHeroImage;

    void Start()
    {
        foreach (var hero in heroButtons)
        {
            hero.button.onClick.AddListener(() => SelectHero(hero));
        }
    }

    void SelectHero(HeroButton hero)
    {
        // ����ѡ�е�Ӣ������
        GeneralDataManager.SelectedHeroName = hero.heroName;
        Debug.Log($"��ѡ��Ӣ��: {hero.heroName}");

 
    }


}