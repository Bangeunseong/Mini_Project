using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintPanelController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _images;
    private int[] arr = { 0, 0, 0, 0, 0 };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var image in _images)
        {
            Button button = Helper.GetComponentHelper<Button>(image);
            button.onClick.AddListener(() =>
            {
                int index = _images.IndexOf(image);
                arr[index] = (arr[index] + 1) % 2;
                StartCoroutine(WaitForAnimation_ChangeSprite(index));
            });
        }
    }

    private IEnumerator WaitForAnimation_ChangeSprite(int _id)
    {
        Animator animator = Helper.GetComponentHelper<Animator>(_images[_id]);
        if(animator != null) { animator.SetTrigger("Fade_trig"); }
        yield return new WaitForSeconds(0.5f);
        Image img = Helper.GetComponentHelper<Image>(_images[_id]);
        img.sprite = TableManager.Instance.GetTable<MemberTable>().GetMemberInfoById(_id).Selfies[arr[_id]];
    }
}
