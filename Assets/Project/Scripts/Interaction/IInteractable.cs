public interface IInteractable
{
    public bool IsInteractable { get; set; }
    public void OnInteract(PlayerReferences playerReferences);
}