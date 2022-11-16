using SkiaSharp;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Input;
class Mygame
{
	public static readonly int SizeX = 1280;
	public static readonly int SizeY = 720;
	private IWindow window;
	private SKSurface surface;
	private SKCanvas canvas;
	private GL gl;

	private SKPaint paint;

	private int currentLevel = 0;
	private UserPreferences userPreferences;
	private List<Key> pressedKeys;
	private List<GameObject> gameobjects;
	private GameObject player;
	private Random random = new Random(((int)DateTime.Now.Ticks));
	private SKImage meteorTexture;
	public Mygame(int width, int height, string title){
		var options = WindowOptions.Default;
		options.Title = title;
		options.Size = new Silk.NET.Maths.Vector2D<int>(width, height);
		options.ShouldSwapAutomatically = false;
		window = Window.Create(options);
		window.FramesPerSecond = 60;
		window.Update += OnUpdate;
		window.Render += OnRender;
		window.Load += OnLoad;
	}

	private void OnUpdate(double arg1){
		if (pressedKeys.Contains(Key.A)){
			gameobjects[0].AddVelocity(-50, 0);
		}

		if (pressedKeys.Contains(Key.D)){
			gameobjects[0].AddVelocity(50, 0);
		}

		if (pressedKeys.Contains(Key.W)){
			gameobjects[0].AddVelocity(0, 50);
		}

		if (pressedKeys.Contains(Key.S)){
			gameobjects[0].AddVelocity(0, -50);
		}

		if(pressedKeys.Contains(Key.Space)){
			createMeteor();
			pressedKeys.Remove(Key.Space);
		}
		List<GameObject> toRemove = new List<GameObject>();
		foreach (GameObject gameObject in gameobjects){
			if (gameObject.Move(arg1, gameobjects))toRemove.Add(gameObject);
		}

		foreach (GameObject gameObject in toRemove) gameobjects.Remove(gameObject);

	}

	private void OnRender(double arg1){
		drawObjects();
		drawGUI();
		swapBuffers();
	}

	private void OnLoad(){

		//create all variables
		pressedKeys = new List<Key>();
		gameobjects = new List<GameObject>();
		//create and bind input context
		var input = window.CreateInput();

		foreach (var keyboard in input.Keyboards){
			keyboard.KeyDown += OnKeyDown;
			keyboard.KeyUp += OnKeyUp;
		}

		foreach (var mouse in input.Mice){
			mouse.MouseDown += OnMouseDown;
			mouse.MouseMove += OnMouseMove;
			mouse.Scroll += OnMouseScroll;
		}

		//create and configure OpenGL context
		gl = window.CreateOpenGL();
		gl.ClearColor(1f, 1f, 1f, 1f);

		//create SkiaSharp context
		var grinterface = GRGlInterface.CreateOpenGl(name => {
			if (window.GLContext.TryGetProcAddress(name, out nint fn)) return fn;
			return (nint)0;
		});

		var skiabackendcontext = GRContext.CreateGl(grinterface);
		var format = SKColorType.Rgba8888;
		var backendrendertarget = new GRBackendRenderTarget(window.Size.X, window.Size.Y, window.Samples ?? 1, window.PreferredStencilBufferBits ?? 16, new GRGlFramebufferInfo(
			0, format.ToGlSizedFormat()
		));
		//var info = new SKImageInfo(window.Size.X, window.Size.Y);
		//surface = SKSurface.Create(info);
		surface = SKSurface.Create(skiabackendcontext, backendrendertarget, format);
		canvas = surface.Canvas;
		var typeface = SKTypeface.CreateDefault();
		paint = new SKPaint(new SKFont(typeface));

		//add gameObjects
		//add ground
		createObjects();


		//load previous settings
		userPreferences = new UserPreferences(true);
		userPreferences.Load();
	}

	public void Run(){
		window.Run();
	}

	private void OnKeyDown(IKeyboard arg1, Key arg2, int arg3){
		if (arg2 == Key.Escape) window.Close();
		if (!pressedKeys.Contains(arg2)) pressedKeys.Add(arg2);
	}

	private void OnKeyUp(IKeyboard arg1, Key arg2, int arg3){
		pressedKeys.Remove(arg2);
	}

	private void OnMouseMove(IMouse arg1, System.Numerics.Vector2 arg2){

	}

	private void OnMouseDown(IMouse arg1, MouseButton arg2){

	}

	private void OnMouseScroll(IMouse arg1, ScrollWheel arg2){

	}
	
	private void drawObjects(){
		foreach (GameObject gameObject in gameobjects){
			canvas.DrawImage(gameObject.Texture, gameObject.Position, paint);
			if (gameObject.CompareTag("Player")){
				if (gameObject.Position.X + gameObject.Texture.Width > SizeX && gameObject.Position.Y + gameObject.Texture.Height > SizeY){
					canvas.DrawImage(gameObject.Texture, gameObject.Position - new SKPoint(SizeX, SizeY), paint);
				}else if(gameObject.Position.X + gameObject.Texture.Width > SizeX){
					canvas.DrawImage(gameObject.Texture, gameObject.Position - new SKPoint(SizeX, 0), paint);
				}else if(gameObject.Position.Y + gameObject.Texture.Height > SizeY){
					canvas.DrawImage(gameObject.Texture, gameObject.Position - new SKPoint(0, SizeY), paint);
				}
			}
		}
	}

	private void drawGUI(){
		
	}

	private void swapBuffers(){
		canvas.Flush();
		window.SwapBuffers();
		canvas.Clear(SKColors.DarkSlateGray);
	}

	private void createObjects(){
		var paint = new SKPaint();
		paint.Color = SKColors.Gold;
		var info = new SKImageInfo(50, 50);
		var bmap = new SKBitmap(info);
		var oCanvas = new SKCanvas(bmap);
		oCanvas.DrawCircle(new SKPoint(0, 0), 150/2, paint);
		player = new GameObject(window.Size.X/2, window.Size.Y/2, SKImage.FromBitmap(bmap), "Player", true, "Player");
		gameobjects.Add(player);

		info = new SKImageInfo(25, 25);
		bmap = new SKBitmap(info);
		oCanvas = new SKCanvas(bmap);
		oCanvas.Clear(SKColors.OrangeRed);
		meteorTexture = SKImage.FromBitmap(bmap);
	}

	private void createMeteor(){
		
		var posx = 0f;
		var posy = 0f;
		var velx = 0f;
		var vely = 0f;
		var rand = random.Next(100);
		if(rand < 25){
			posx = random.Next(SizeX);
			posy = 0;
			velx = random.Next(10, 200);
			vely = -random.Next(100, 200);
		}else if(rand < 50){
			posx = random.Next(SizeX);
			posy = SizeY;
			velx = random.Next(100, 200);
			vely = random.Next(100, 200);
		}else if(rand < 75){
			posx = 0;
			posy = random.Next(SizeY);
			velx = random.Next(100, 200);
			vely = random.Next(100, 200);
		}else{
			posx = SizeX;
			posy = random.Next(SizeY);
			velx = -random.Next(100, 200);
			vely = random.Next(100, 200);
		}
		Console.WriteLine(meteorTexture.Info.Size.Width);
		var meteor = new GameObject(posx, posy, meteorTexture, $"Meteor{DateTime.Now.Ticks}", false, "Meteor");
		meteor.AddVelocity(velx, vely);
		gameobjects.Add(meteor);
	}
}