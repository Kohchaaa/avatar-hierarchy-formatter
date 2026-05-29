
namespace Kohcha.AvatarHierarchyFormatter
{
    public interface IAHFSettingModule
    {
        string ModuleName { get; }

        void Load();
        void Save();
        void OnGUI();
    }
}