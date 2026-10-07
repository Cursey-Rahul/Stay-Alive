using UnityEngine;
using UnityEngine.Rendering;

public class Enity_AnimationEvents : MonoBehaviour
{
    private Entity entity;
    private void Awake() { 
        entity = GetComponentInParent<Entity>(); 
        

    }
   

   
    private void DisableMovementAndJump() => entity.EnableMovement(false);
    private void EnableMovementAndJump() => entity.EnableMovement(true);
    public void DamageTargets() => entity.DamageTargets();

    public void EndAttack()=> entity.EndAttack();


}
