using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beelzebub : MonoBehaviour
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
    private float dashDistance = 15.0f;
    //Variable para saber si el personaje está en mitad de un dash
    private bool isDashing = false;
    //Variable para limitar el número de dashes en el aire
    private int dashCount = 0;
    //Creamos la variable para acceder al componente spriteRenderer
    private SpriteRenderer spriteRenderer;
    //Creamos la variable para acceder al componente animator
    private Animator animator;
    //Variable para controlar el tiempo que lleva esperando el personaje
    private float waitTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        //Accedemos al rigidbody, al spriteRenderer y al animator del gameobject
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Método de los movimientos del jugador
        actionsPlayer();
    }
    //Colisionador para saber cuando el personaje está en el suelo
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Suelo")
        {
            onGround = true;
        }
    }
    //Y para saber cuando sale del suelo y está en el aire
    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Suelo")
        {
            onGround = false;
        }
    }
    //El método para los movimientos del jugador
    void actionsPlayer ()
    {
        //Variables para la posición en X e Y del jugador 
        float posX = transform.position.x;
        float posY = transform.position.y;
        //Variable para saber el desplazamiento horizontal del personaje
        float desplX = Input.GetAxisRaw("Horizontal");
        //Variable para saber el desplazamiento vertical del personaje
        float desplY = Input.GetAxisRaw("Vertical");
        //Variable para saber si se está agachando el personaje
        bool crouch = false;
        //Variable Vector3 para que el personaje se mueva.
        Vector3 moveDirection = new Vector3(desplX, 0, desplY);
        //Comprobamos que se cumplen las condiciones para que el personaje se intente agachar
        if (Input.GetButton("Crouch"))
        {
            crouch = true;
        }
        else
        {
            crouch = false;
        }
        //Asignamos el movimiento al personaje si no está en un dash
        if (!isDashing)
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
        if (Input.GetButtonDown("Jump") && onGround)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpPower, 0);
        }
        //Cambiamos el sprite del personaje para que se agache y reducimos su velocidad cuando está agachado
        if (crouch && onGround)
        {
            speed = 3.0f;
            animator.SetBool("Agacharse", crouch);
        }
        else if (!crouch || !onGround)
        {
            speed = 5.0f;
            animator.SetBool("Agacharse", crouch);
        }
        //Si el jugador pulsa el botón, el personaje hace un dash en la dirección en la que esté mirando
        if (Input.GetButtonDown("Dash") && !spriteRenderer.flipX && dashCount == 0)
        {
            StartCoroutine(Dash(1f));
            //Si estamos en el aire hacemos que el personaje no tenga más dashes disponibles
            if (!onGround)
            {
                dashCount = 1;
            }
        }
        else if (Input.GetButtonDown("Dash") && spriteRenderer.flipX && dashCount == 0)
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
        //Variable para saber si el personaje está moviéndose
        bool isMoving = false;
        //Variable para saber si está esperando sin moverse
        bool isWaiting = false;
        //Comprobamos si el personaje se está moviendo
        if (desplX == 1 || desplX == -1 || desplY == 1 || desplY == -1)
        {
            //Cambiamos la variable para saber que el personaje se está moviendo
            isMoving = true;
            //Cambiamos la booleana para afectar al animator
            animator.SetBool("Moverse", isMoving);
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
        if (waitTime >= 10)
        {
            isWaiting = true;
        }
        else if (waitTime < 10)
        {
            isWaiting = false;
        }
        //Cambiamos la booleana del animator para evitar que se active la animación de espera
        animator.SetBool("Esperando", isWaiting);
    }
    //Corrutina para el dash
    IEnumerator Dash(float direction)
    {
        //Cambiamos la booleana para declarar que el personaje está en un dash
        isDashing = true;
        //Variable del tiempo que dura el dash
        float dashTime = 0.2f;
        //Le asignamos el movimiento al dash gracias al rigidbody
        rb.velocity = new Vector3(rb.velocity.x, 0f, 0f);
        rb.AddForce(new Vector3(dashDistance * direction, 0f, 0f), ForceMode.Impulse);
        //Quitamos la gravedad mientras dure el dash para que vaya en línea recta
        rb.useGravity = false;
        //Asignamos una duración al dash
        yield return new WaitForSeconds(dashTime);
        //Cambiamos la velocidad a 0 para que se pare en seco el dash
        rb.velocity = new Vector3(0f, 0f, 0f);
        //Regresamos las variables de gravedad y dash a su estado principal
        rb.useGravity = true;
        isDashing = false;
    }
}
