using System.Linq;

public class MoveDisplay : DisplayBase {

    public override IEvents DisplayEvents
    {
        get { return _events; }
    }

    /// <summary>
    /// UIオブジェクト呼び出しイベントクラス
    /// </summary>
    public MoveEvents _events = new MoveEvents();

    public override void OnAwake(ISceneCache cache)
    {
        gameObject.SetActive(true);
        // シーンキャッシュとイベントクラスを各UIオブジェクトに渡す
        uiList.ForEach(e => e.OnAwake(cache, _events));
        isCallOnAwake = true;
        switchAnim.OnAwake(uiList);
    }

    void Update()
    {
        if (!isCallOnAwake)
            return;
        // 描画処理
        uiList.Where(e => e.IsDrawUpdate()).ToList().ForEach(e => e.OnDrawUpdate());
    }
}
