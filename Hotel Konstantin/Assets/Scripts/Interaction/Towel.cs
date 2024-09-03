using UnityEngine;

public class Towel : MonoBehaviour, IInteractable
{
    [SerializeField] private Room Room;
    [SerializeField] private Animator Animator;

    [SerializeField] private MeshRenderer Renderer;
    [SerializeField] private Material[] Materials;

    [SerializeField] private bool Updated;

    public float _BeforeTime => 2;
    public bool _Updated
    {
        get
        {
            return Updated;
        }
        set
        {
            if(Updated == value)
            {
                return;
            }

            Updated = value;

            Renderer.material = Materials[Updated ? 0 : 1];

            if (Updated)
            {
                Animator.Play("update");
            }

            Room.UpdateTaskInfo();
        }
    }
    public bool _CanInteract => !Updated;

    public void Interact()
    {
        _Updated = !Updated;
    }
}
