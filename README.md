Simulación de Restaurante 2D en Unity: Feline Restaurant.

Es un juego de simulación y gestión de tiempo en 2D desarrollado en Unity 6000.2.4f1. El jugador controla a un mesero que debe gestionar el flujo de clientes, tomar pedidos, servir comida y mantener el restaurante limpio, todo contra el reloj.

- Características Principales

- Sistema de Clientes:
  
Los clientes cuentan con las siguientes funciones:
    -  Esperan en fila en la puerta (con indicador de paciencia).
    -  Sistema de "Seguir al Jugador" para asignar mesas manualmente.
    -  Fases de paciencia: Esperando ser guiado hacia una mesa desocupada, esperando ser atendido y esperando comida.

- Ciclo de Juego Completo:

    -  Asignar mesa -> Tomar pedido -> Enviar orden al Chef -> Recoger plato -> Servir -> Cobrar (Propina basada en rapidez, si el pedido es correcto o si el pedido es incorrecto).

- Gestión de Crisis:

    -  Vitrina de Snacks: Empanadas, Arepas y Café/Tinto para restaurar la paciencia de los clientes en espera.
    -  Charcos: Aparecen aleatoriamente y se ensucian con el tiempo; deben limpiarse para ganar puntos extra y evitar que los clientes se impacienten más rápido.
    -  Basurero: Para desechar pedidos que no se hayan alcanzado a entregar.

- Sistema de Dificultad:

    -  Fácil: Clientes pacientes, ritmo lento.
    -  Normal: La experiencia estándar.
    -  Difícil: Clientes impacientes y frecuentes, pero el jugador y el chef son más rápidos.

- Accesibilidad y Controles:

Soporte nativo para Teclado y Mando (Gamepad) con navegación de menús automática y feedback visual (marco de selección).

- Audio Inmersivo:

Música de fondo, efectos de sonido (dinero, clicks, limpieza) y opciones de Muteo independientes.

- Easter Egg:

Dos gatos de origen japonés muy famosos y amantes de la música se encuentran durmiendo en una parte del mapa, al interactuar con ellos, puede que saluden al mesero con un maullido.

- Controles

El juego detecta automáticamente si el jugador usa teclado o mando.

  -   Moverse: WASD en teclado, Joystick Izquierdo en mando.
  -   Interactuar/Aceptar: (E) en teclado, Cuadrado (PS) / X (Xbox)
  -   Pausar/Atrás: Escape/P en teclado, Triángulo (PS) / Y (Xbox)
  -   Mutear Música: (M) en teclado, Equis (PS) / A (Xbox)
  -   Mutear Efectos (SFX): (F) en teclado, Círculo (PS) / B (Xbox)

Nota: En los menús de selección (Platos, Snacks, Dificultad), se usan las flechas o el joystick para mover el marco de selección.

- Mecánicas Detalladas

- Flujo de Cocina:
    -  Interactúar con el cliente para tomar la orden (luego de interactuar, aparece una burbuja de diálogo encima de estos con uno de los platillos disponibles: Hamburguesa, pizza y ensalada).
    -  Ir a la cocina e interactuar con el Chef.
    -  Seleccionar el plato correcto en el menú emergente.
    -  Esperar a que el Chef termine (el tiempo varía según la dificultad).
    -  Recoger el plato y entregarlo a la mesa correspondiente.

- Paciencia y Snacks:
    -  Si un cliente espera demasiado (en la puerta o en la mesa), se irá enojado, restando puntos a la puntuación final. Si la fila está llena o no hay mesas, el jugador puede ir a la Vitrina, tomar un Snack y dárselo a un cliente de pie para recuperar ligeramente su paciencia.

- Limpieza:

Aparecerán charcos de agua en el suelo, los cuales el jugador deberá limpiar para evitar que los clientes pierdan más paciencia de lo habitual.
    -  Estado 1 (Limpio): Da más dinero al limpiar.
    -  Estado 2 (Sucio a medias): Da dinero regular.
    -  Estado 3 (Muy sucio): Da muy poco dinero.

- Instalación y Ejecución

    -  Clonar el repositorio o directamente descargar el .zip.
    -  Abrir la build/.exe.

Para ver código fuente, o simplemente explorar:
       -  Abrir Unity Hub.
       -  Añadir el proyecto (Asegúrese de tener instalada la versión 6000.2.4f1 o superior).
       -  Abrir la escena principal.
       -  Presionar el botón Play en el editor.

- Configuración Técnica

    -  Input System: Utiliza el Input Manager clásico configurado para soportar múltiples dispositivos simultáneamente.
    -  UI: Sistema de Canvas optimizado con WorldToScreenPoint para textos flotantes que siguen a los personajes.
    -  Singleton Pattern: Implementado en GameManager, UIManager y AudioManager para la gestión centralizada.

- Nota:
    -  Link hacia la fuente utilizada en caso de necesitar instalarse: https://www.dafontfree.io/genshin-impact-font/
