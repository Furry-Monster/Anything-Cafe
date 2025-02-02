/// <summary>
/// 场景枚举,手动修改。
/// 游戏打包时，记得在发布中勾选所有场景，并按SceneEnum中的顺序排序
/// 每次创建新场景的时候，在SceneEnum中添加一个新的枚举值，并添加一个新的SceneHandler脚本
/// </summary>
public enum SceneEnum
{
    TitleScene,
    PlayScene,
    ScenarioScene,
    GalleryScene,
}