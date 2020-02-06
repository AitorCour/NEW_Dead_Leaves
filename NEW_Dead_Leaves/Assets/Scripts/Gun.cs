using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{

    public float maxDistance;
    public LayerMask mask;
	//private AudioSource sound;

	//public Vector3 direction = Vector3.forward;
    //private Transform transform.forward;
    private int maxAmmo = 7;
    public int currentAmmo;
    public int Munition;
    //private int iniMunition = 0;
	private int variableM;
    public float fireRate;
    public float hitForce;
    public int hitDamage;

    public bool isShooting;
    public bool isReloading;
    public bool fps;
    public float ReloadTime;

    private EnemyBehaviour targetEnemy;
    public Transform gunOrigin;
    //private PlayerBehaviour plBehaviour;
    //private HUD hud;

	// Use this for initialization
	void Start ()
    {
        isShooting = false;
        isReloading = false;
        //currentAmmo = maxAmmo;
        //Munition = iniMunition;
        //plBehaviour = GetComponentInParent<PlayerBehaviour>();
        //hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();
        //currentAmmo = Data.GetAmmo();
        //Munition = Data.GetMunition();
        //hud.SetAmmo(currentAmmo, Munition);
		//sound = GetComponentInChildren<AudioSource>();
	}
    public void Shot()
    {
        if(isShooting || isReloading) return;
        if(currentAmmo <= 0)
        {
            //plBehaviour.NoShootSound();
            return;
        }
        else if(currentAmmo > 0)
        {
            //animator.SetTrigger("Shooting");
            //plBehaviour.ShootSound();
            isShooting = true;
            currentAmmo--;
            //Data.SetAmmo(currentAmmo);
            //hud.SetAmmo(currentAmmo, Munition);
            //particleShoot.Play();
            //particleSteam.Play();
        }
        if (!fps)
        {
            ShotTPS();
        }
        else
        {
            ShotFPS();
        }
        StartCoroutine(WaitFireRate());
    }

    private IEnumerator WaitFireRate() //Usar corutinas para contar tiempo
    {
        yield return new WaitForSeconds(fireRate);
        isShooting = false;

       // yield return null;//cierra la corutina
    }

    public void Reload()
    {
        if(isReloading) return;
        if(Munition <= 0 ) return;
		if(currentAmmo == maxAmmo) return;
        isReloading = true;
        //plBehaviour.ReloadSound();
        // animacion.SetTrigger("recharge");
        //reload.SetTrigger ("reload");
        StartCoroutine(WaitForReload());
		//animator.SetTrigger("Reloading");
        //Data.SetAmmo(currentAmmo);
    }

    private IEnumerator WaitForReload()
    {
        yield return new WaitForSeconds(ReloadTime);
		if (currentAmmo > 0)
		{
			variableM = maxAmmo - currentAmmo;
			if(variableM > Munition)
			{
				currentAmmo += Munition;
				Munition = 0;
			}
			if(variableM <= Munition)
			{
				Munition -= variableM;
				currentAmmo += variableM;
			}
		}
		else if (Munition >= maxAmmo)
		{
			currentAmmo = maxAmmo;
			Munition -= maxAmmo;
		}
        else if (Munition < maxAmmo)
		{
			currentAmmo = Munition;
			Munition = 0;
		}
        isReloading = false;
        //Data.SetAmmo(currentAmmo);
        //hud.SetAmmo(currentAmmo, Munition);
        //Data.SetMunition(Munition);
    }

    public void GetAmmo(int m)
    {
        Munition += m;
        //hud.SetAmmo(currentAmmo, Munition);
        //Data.SetMunition(Munition);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = gunOrigin.TransformDirection(Vector3.forward) * maxDistance;
        Gizmos.DrawRay(gunOrigin.position, direction); //forward
    }
    private void ShotTPS()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(gunOrigin.position, gunOrigin.forward, out hit, maxDistance, mask))
        {
            Debug.Log(hit.transform.name);
            //Debug.DrawRay(transform.position, transform.forward * maxDistance, Color.red, 10.0f);
            if (hit.rigidbody != null && hit.rigidbody.tag == "Enemy")
            {
                Debug.Log("No null");
                hit.rigidbody.AddForce(gunOrigin.forward * hitForce, ForceMode.Impulse);

                EnemyBehaviour target = hit.transform.gameObject.GetComponent<EnemyBehaviour>();

                targetEnemy = target;
                /*targetEnemy.LoseLife(hitDamage);
                particleBlood[currentBlood].transform.position = hit.point;
                particleBlood[currentBlood].time = 0;
                particleBlood[currentBlood].Play();
                currentBlood++;
                if (currentBlood >= maxBlood) currentBlood = 0;*/
            }
        }
    }
    private void ShotFPS()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Coje el punto de la posicion del mouse y lanza un rayo

        //RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, maxDistance, mask))
        {
            Debug.Log(hit.transform.name);
            Debug.DrawRay(transform.position, ray.direction * maxDistance, Color.blue, 1.0f);

            if (hit.rigidbody != null && hit.rigidbody.tag == "Enemy")
            {
                Debug.Log("No null");
                hit.rigidbody.AddForce(ray.direction * hitForce, ForceMode.Impulse);

                EnemyBehaviour target = hit.transform.gameObject.GetComponent<EnemyBehaviour>();

                targetEnemy = target;
                /*targetEnemy.LoseLife(hitDamage);
                particleBlood[currentBlood].transform.position = hit.point;
                particleBlood[currentBlood].time = 0;
                particleBlood[currentBlood].Play();
                currentBlood++;
                if (currentBlood >= maxBlood) currentBlood = 0;*/
            }
        }
    }
}
