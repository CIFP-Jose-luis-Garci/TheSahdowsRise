using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    //Creamos la variable para acceder al rigidbody
    private Rigidbody rb;
    //Asignamos una potencia al salto
    private float jumpPower = 7.0f;
    //Variable para comprobar si el personaje está en el suelo
    private bool onGround;
    //Velocidad base del personaje
    private float speed = 5.0f;
    //Distancia del dash
    private float dashDistance = 10.0f;
    //Variable para saber si el personaje está en mitad de un dash
    private bool isDashing = false;
    //Variable para limitar el número de dashes en el aire
    private int dashCount = 0;
    //Creamos la variable para acceder al componente spriteRenderer
    private SpriteRenderer spriteRenderer;
    //Creamos la variable para acceder al componente animator
    private Animator animator;
    //Creamos la variable para acceder al box collider
    private BoxCollider boxCollider;
    //Variables para los tamaños del collider y sus respectivos centros
    private float sizeXbase = 0.84f;
    private float sizeYbase = 1.09f;
    private float centerYbase = 0.56f;
    private float sizeXmoving = 1.28f;
    private float sizeXcrouch = 1.68f;
    private float sizeYcrouch = 0.8f;
    private float centerYcrouch = 0.4f;
    //Creamos la variable para acceder al componente audioSource
    private AudioSource audioSource;
    [SerializeField] AudioSource crouchSource;
    [SerializeField] AudioSource jumpSource;
    //Accedemos al audio
    [SerializeField] AudioClip walkAudio;
    [SerializeField] AudioClip dashAudio;
    [SerializeField] AudioClip gateAudio;
    //Variable para controlar el tiempo que lleva esperando el personaje y el tiempo máximo de espera
    private float waitTime = 0.0f;
    private float maxWaitTime = 10.0f;
    //Variable para saber si se está agachando el personaje
    private bool crouch = false;
    //Variable para saber si el personaje está moviéndose
    private bool isMoving = false;
    //Variable para saber si está esperando sin moverse
    private bool isWaiting = false;
    //Variable para saber si acaba de empezar a andar
    private bool startedWalking = false;
    private bool startedCrouchMoving = false;
    //Variable para saber si está saltando
    private bool isJumping = false;
    [SerializeField] GameObject[] rooms;
    private int roomNumber = 0;
    [HideInInspector] public static int currentRoom = 0;
    [SerializeField] SpriteRenderer skeletonRender;
    [SerializeField] Rigidbody skeletonRB;
    private bool keepCrouching = false;
    [SerializeField] SpriteRenderer flameR;
    [SerializeField] SpriteRenderer flameL;
    [HideInInspector] public static bool isAlive = true;
    [HideInInspector] public bool awaken = false;
    private Controls controls;
    private bool shownCrouchCtrl = false;
    private bool shownDashCtrl = false;
    private bool shownInteractCtrl = false;
    [SerializeField] GameObject take;
    [SerializeField] GameObject use;
    [SerializeField] GameObject item;
    private bool coinRange = false;
    [SerializeField] GameObject item2;
    private bool keyRange = false;
    [SerializeField] GameObject lamp;
    private bool lampRange = false;
    [SerializeField] GameObject mustWait;
    private bool waitTextActive = false;
    private bool doorRange = false;
    private bool finished = false;
    [SerializeField] Animator lampAni;
    private bool gotKey = false;
    [SerializeField] GameObject key;
    private bool ledgePart = false;
    [SerializeField] GameObject[] cameras;
    private Image fadeIn;
    void Start()
    {
        //Accedemos al rigidbody, al spriteRenderer, al box collider y al animator del gameobject
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();
        controls = GameObject.Find("Control").GetComponent<Controls>();
        currentRoom = 0;
        roomChanger();
        skeletonRender.enabled = false;
        flameR.enabled = false;
        flameL.enabled = false;
        isAlive = true;
        Invoke("Despierta", 5f);
        take.SetActive(false);
        use.SetActive(false);
        item2.SetActive(false);
        mustWait.SetActive(false);
        key.SetActive(false);
        fadeIn = GameObject.Find("Fundido").GetComponent<Image>();
    }
    void Update()
    {
        //Método de los movimientos del jugador
        if (awaken == true && isAlive == true)
        {
            actionsPlayer();
        }
        if (!isMoving && !crouch && !isDashing)
        {
            boxCollider.size = new Vector3(sizeXbase, sizeYbase, 1f);
            boxCollider.center = new Vector3(0f, centerYbase, 0f);
        }
        cameras[0].SetActive(!ledgePart);
        if (cameras[0].activeInHierarchy == true)
        {
            cameras[1].SetActive(false);
            jumpPower = 7;
        }
        else if (cameras[0].activeInHierarchy == false)
        {
            cameras[1].SetActive(true);
            jumpPower = 8;
        }
        if (Input.GetButtonDown("Interact"))
        {
            if (coinRange == true)
            {
                Coin.gotCoin = true;
                take.SetActive(false);
                item.SetActive(false);
                coinRange = false;
            }
            if (lampRange == true && Skeleton.hittable == true)
            {
                use.SetActive(false);
                lamp.SetActive(false);
                lampAni.SetTrigger("Cae");
                Invoke("Llave", 1f);
                lampRange = false;
            }
            else if (lampRange == true && Skeleton.hittable == false && !waitTextActive)
            {
                mustWait.SetActive(true);
                waitTextActive = true;
                Invoke("WaitText", 3f);
            }
            if (keyRange == true && !gotKey)
            {
                take.SetActive(false);
                item2.SetActive(false);
                gotKey = true;
                key.SetActive(true);
            }
            if (doorRange == true && gotKey == true)
            {
                key.SetActive(false);
                use.SetActive(false);
                finished = true;
            }
        }
        if (finished == true)
        {
            Color color = fadeIn.color;
            color.a += 0.3f * Time.deltaTime;
            fadeIn.color = color;
            Invoke("Ending", 5);
        }
    }
    //Colisionador para saber cuando el personaje está en el suelo
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Suelo")
        {
            onGround = true;
            //Cambiamos la booleana del animator para saber si está en el suelo.
            animator.SetBool("Suelo", onGround);
        }
        if (col.gameObject.tag == "Enemigo" && !Skeleton.knockedDown)
        {
            isAlive = false;
            animator.SetTrigger("Pillado");
            speed = 0;
            audioSource.Stop();
            Invoke("Caught", 3);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Room0Activator" && currentRoom != 0)
        {
            currentRoom = 0;
            roomChanger();
            skeletonRender.enabled = false;
            skeletonRB.useGravity = false;
            flameR.enabled = false;
            flameL.enabled = false;
        }
        else if (other.gameObject.name == "Room1Activator" && currentRoom != 1)
        {
            currentRoom = 1;
            roomChanger();
            skeletonRender.enabled = true;
            skeletonRB.useGravity = true;
            flameR.enabled = true;
            flameL.enabled = true;
        }
        if (other.gameObject.tag == "AntiStandUp")
        {
            keepCrouching = true;
        }
        if (other.gameObject.tag == "ControlText")
        {
            if (other.gameObject.name == "CrouchCtrl" && !shownCrouchCtrl)
            {
                controls.crouchCtrl = true;
                shownCrouchCtrl = true;
            }
            else if (other.gameObject.name == "DashCtrl" && !shownDashCtrl)
            {
                controls.dashCtrl = true;
                shownDashCtrl = true;
            }
            else if (other.gameObject.name == "InteractCtrl" && !shownInteractCtrl)
            {
                controls.interactCtrl = true;
                shownInteractCtrl = true;
            }
        }
        if (other.gameObject.name == "Coin")
        {
            take.SetActive(true);
            coinRange = true;
        }
        if (other.gameObject.name == "Lamp" && Coin.gotCoin == true)
        {
            use.SetActive(true);
            lampRange = true;
        }
        if (other.gameObject.name == "Llave")
        {
            take.SetActive(true);
            keyRange = true;
        }
        if (other.gameObject.name == "Puerta")
        {
            use.SetActive(true);
            doorRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "AntiStandUp")
        {
            keepCrouching = false;
        }
        if (other.gameObject.name == "Coin")
        {
            take.SetActive(false);
            coinRange = false;
        }
        if (other.gameObject.name == "Lamp")
        {
            use.SetActive(false);
            lampRange = false;
        }
        if (other.gameObject.name == "Key")
        {
            take.SetActive(false);
            keyRange = false;
        }
        if (other.gameObject.name == "Puerta")
        {
            use.SetActive(false);
            doorRange = false;
        }
        if (other.gameObject.name == "CamChanger")
        {
            if (transform.position.x > 25.0f)
            {
                ledgePart = true;
            }
            else if (transform.position.x < 24.0f)
            {
                ledgePart = false;
            }
        }
    }
    //El método para los movimientos del jugador
    private void actionsPlayer ()
    {
        //Variables para la posición en X e Y del jugador 
        float posX = transform.position.x;
        float posY = transform.position.y;
        //Variable para saber el desplazamiento horizontal del personaje
        float desplX = Input.GetAxisRaw("Horizontal");
        if (desplX > 0)
        {
            desplX = 1;
        }
        else if (desplX < 0)
        {
            desplX = -1;
        }
        //Variable para saber el desplazamiento vertical del personaje
        float desplY = Input.GetAxisRaw("Vertical");
        if (desplY > 0)
        {
            desplY = 1;
        }
        else if (desplY < 0)
        {
            desplY = -1;
        }
        //Variable Vector3 para que el personaje se mueva.
        Vector3 moveDirection = new Vector3(desplX, 0, desplY / 2);
        //Comprobamos que se cumplen las condiciones para que el personaje se intente agachar
        if (Input.GetButton("Crouch"))
        {
            crouch = true;
        }
        else
        {
            if (!keepCrouching)
            {
                crouch = false;
            }
        }
        //Asignamos el movimiento al personaje si no está en un dash
        if (!isDashing && boxCollider.enabled == true)
        {
            rb.MovePosition(rb.position + moveDirection * speed * Time.deltaTime);
            StopCoroutine("Dash");
        }
        //Cambiamos el sprite según hacia donde haga el dash
        if (-desplX > 0 && !isDashing)
        {
            spriteRenderer.flipX = true;
        }
        else if (-desplX < 0 && !isDashing)
        {
            spriteRenderer.flipX = false;
        }
        //Comprobamos que se cumplen las condiciones para saltar y hacemos saltar al personaje
        if (Input.GetButtonDown("Jump") && onGround && !isDashing && !isJumping && isAlive && boxCollider.enabled == true)
        {
            StartCoroutine(Salto());
        }
        //Cambiamos el sprite del personaje para que se agache, reducimos su velocidad cuando está agachado y cambiamos el tamaño del box collider
        if (crouch && onGround && isAlive)
        {
            speed = 3.0f;
            animator.SetBool("Agacharse", crouch);
            //Cambiamos el collider a agachado
            boxCollider.size = new Vector3(sizeXcrouch, sizeYcrouch, 1f);
            boxCollider.center = new Vector3(0f, centerYcrouch, 0f);
            if (isMoving == true && !startedCrouchMoving)
            {
                crouchSource.Play();
                startedCrouchMoving = true;
            }
            else if (!isMoving || !onGround || !crouch || speed != 3.0f)
            {
                startedCrouchMoving = false;
                crouchSource.Stop();
            }
        }
        else if (!crouch || !onGround && isAlive)
        {
            speed = 5.0f;
            animator.SetBool("Agacharse", crouch);
            crouchSource.Stop();
            startedCrouchMoving = false;
        }
        //Si el jugador pulsa el botón, el personaje hace un dash en la dirección en la que esté mirando
        if (Input.GetButtonDown("Dash") && !spriteRenderer.flipX && dashCount == 0 && !isDashing && !isJumping && isAlive)
        {
            StartCoroutine(Dash(1f));
            //Si estamos en el aire hacemos que el personaje no tenga más dashes disponibles
            if (!onGround)
            {
                dashCount = 1;
            }
        }
        else if (Input.GetButtonDown("Dash") && spriteRenderer.flipX && dashCount == 0 && !isDashing && !isJumping && isAlive)
        {
            StartCoroutine(Dash(-1f));
            if (!onGround)
            {
                dashCount = 1;
            }
        }
        //Reseteamos el contador de dashes cuando el personaje está en el suelo
        if (onGround)
        {
            dashCount = 0;
        }
        //Comprobamos si el personaje se está moviendo
        if (desplX == 1 || desplX == -1 || desplY == 1 || desplY == -1)
        {
            //Cambiamos la variable para saber que el personaje se está moviendo
            isMoving = true;
            //Cambiamos la booleana para afectar al animator
            animator.SetBool("Moverse", isMoving);
            //Cambiamos el box collider si no está agachado
            if (!crouch)
            {
                boxCollider.size = new Vector3(sizeXmoving, sizeYbase, 1f);
                boxCollider.center = new Vector3(0f, centerYbase, 0f);
            }
            //Reseteamos el tiempo de espera a 0 porque se está moviendo
            waitTime = 0;
        }
        //Comprobamos si el personaje está quieto
        else if (desplX == 0 && desplY == 0)
        {
            //Cambiamos la variable para saber que el personaje está quieto
            isMoving = false;
            //Cambiamos la booleana para afectar al animator
            animator.SetBool("Moverse", isMoving);
            //Activamos el temporizador de tiempo de espera
            waitTime += Time.deltaTime;
        }
        //Comprobamos el tiempo de espera y cambiamos la booleana acorde con el tiempo
        if (waitTime >= maxWaitTime)
        {
            isWaiting = true;
        }
        else if (waitTime < maxWaitTime)
        {
            isWaiting = false;
        }
        //Cambiamos la booleana del animator para evitar que se active la animación de espera
        animator.SetBool("Esperando", isWaiting);
        if (isMoving == true && !startedWalking && onGround == true && !crouch && boxCollider.enabled == true)
        {
            //Cambiamos la booleana para que solo se ponga el audio una vez
            startedWalking = true;
            //Activamos el audio de los pasos
            audioSource.clip = walkAudio;
            audioSource.Play();
        }
        else if (!isMoving || !onGround || crouch)
        {
            //Paramos el audio
            if (!isDashing)
            {
                audioSource.Stop();
            }
            //Volvemos a poner la booleana en falso para que pueda activarse el audio otra vez
            startedWalking = false;
        }
    }
    //Corrutina para el dash
    private IEnumerator Dash(float direction)
    {
        //Cambiamos la booleana para declarar que el personaje está en un dash
        isDashing = true;
        //Cambiamos el tamaño del box collider
        boxCollider.size = new Vector3(sizeXmoving, sizeYbase, 1f);
        boxCollider.center = new Vector3(0f, centerYbase, 0f);
        //Activamos el trigger del animator para el dash
        animator.SetTrigger("Dashing");
        //Desactivamos el audio de andar
        audioSource.Stop();
        //Activamos el audio del dash
        audioSource.PlayOneShot(dashAudio);
        //Variable para tiempo de espera antes del dash
        float dashStart = 0.1f;
        //Variable del tiempo que dura el dash
        float dashTime = 0.3f;
        //Variable de tiempo de espera después del dash
        float dashEnd = 0.2f;
        //Le hacemos esperar un poco al dash para que la animación empiece bien
        yield return new WaitForSeconds(dashStart);
        //Le asignamos el movimiento al dash gracias al rigidbody
        rb.velocity = new Vector3(rb.velocity.x, 0f, 0f);
        rb.AddForce(new Vector3(dashDistance * direction, 0f, 0f), ForceMode.Impulse);
        //Quitamos la gravedad mientras dure el dash para que vaya en línea recta
        rb.useGravity = false;
        //Asignamos una duración al dash
        yield return new WaitForSeconds(dashTime);
        //Cambiamos la velocidad a 0 para que se pare en seco el dash
        rb.velocity = new Vector3(0f, 0f, 0f);
        //Regresamos la variable de gravedad a su estado principal
        rb.useGravity = true;
        //Regresamos el box collider a su estado original
        boxCollider.size = new Vector3(sizeXbase, sizeYbase, 1f);
        boxCollider.center = new Vector3(0f, centerYbase, 0f);
        //Si el personaje está en el aire acabamos el dash ya
        if (!onGround)
        {
            isDashing = false;
        }
        //Asignamos tiempo de espera para que no se pueda usar el dash muy seguido si está en el suelo
        else if (onGround)
        {
            yield return new WaitForSeconds(dashEnd);
            isDashing = false;
        }
    }
    //Esta corrutina activa el trigger para la animación del salto y luego le da una potencia de salto
    private IEnumerator Salto()
    {
        jumpSource.Play();
        isJumping = true;
        if (isJumping)
        {
            onGround = false;
            //Cambiamos la booleana del animator para saber si está en el suelo.
            animator.SetBool("Suelo", onGround);
            boxCollider.size = new Vector3(sizeXbase, sizeYbase, 1f);
            boxCollider.center = new Vector3(0f, centerYbase, 0f);
        }
        animator.SetTrigger("Salto");
        yield return new WaitForSeconds(0.2f);
        isJumping = false;
        rb.velocity = new Vector3(rb.velocity.x, jumpPower, 0);
    }
    private void roomChanger()
    {
        roomNumber = 0;
        foreach (GameObject room in rooms)
        {
            if (roomNumber == currentRoom)
            {
                rooms[roomNumber].SetActive(true);
            }
            else if (roomNumber != currentRoom)
            {
                rooms[roomNumber].SetActive(false);
            }
            roomNumber += 1;
        }
    }
    private void Despierta()
    {
        animator.SetTrigger("Despierto");
        Invoke("Jugable", 2.5f);
    }
    private void Jugable()
    {
        awaken = true;
    }
    private void Caught()
    {
        SceneManager.LoadScene("DEFINITIVA");
    }
    private void Llave()
    {
        item2.SetActive(true);
    }
    private void WaitText()
    {
        waitTextActive = false;
        mustWait.SetActive(false);
    }
    private void Ending()
    {
        SceneManager.LoadScene("Ending");
    }
}
