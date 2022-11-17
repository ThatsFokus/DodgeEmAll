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

	private int currentLevel;
	private UserPreferences userPreferences;
	private List<Key> pressedKeys;
	private List<GameObject> gameobjects;
	private GameObject player;
	private Random random = new Random(((int)DateTime.Now.Ticks));
	private SKImage meteorTexture;
	private double meteorTimer;
	private int score;
	private int difficulty;

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
		window.Closing += onClosing;
	}

	private void OnUpdate(double arg1){
		if (currentLevel == 0){
			showMainMenu(arg1);
		}else if( currentLevel == 1){
			showGame(arg1);
		}else if( currentLevel == 2){
			showGameOver(arg1);
		}else{
			Console.WriteLine($"[ERROR] levek {currentLevel} doesn't exist! \nPress any Key close the window.");
			Console.ReadKey(true);
			window.Close();
		}
	}

	private void OnRender(double arg1){
		if (currentLevel == 0){
			showMainMenuRender();
		}else if( currentLevel == 1){
			showGameRender();
		}else if( currentLevel == 2){
			showGameOverRender();
		}
		swapBuffers();
	}

	private void OnLoad(){
		currentLevel = 0;
		difficulty = 0;
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
		userPreferences = new UserPreferences();
		userPreferences.Load();
	}

	public void Run(){
		window.Run();
	}

	private void OnKeyDown(IKeyboard arg1, Key arg2, int arg3){
		if (arg2 == Key.Escape) window.Close();
		if (!pressedKeys.Contains(arg2)) pressedKeys.Add(arg2);
	}

	private void showGameOver(double arg1){
		if(pressedKeys.Contains(Key.Space)){
			pressedKeys.Remove(Key.Space);
			currentLevel = 0;
			if (userPreferences.HighScore < score) userPreferences.HighScore = score;
		}
	}

	private void showGameOverRender(){
		paint.TextSize = 50;
		canvas.DrawText($"{score}", SizeX / 2 - paint.MeasureText($"{score}") / 2, 100, paint);

		paint.TextSize = 36;
		canvas.DrawText("Game Over", SizeX / 2 - paint.MeasureText("Game Over") / 2, SizeY / 2, paint);
		
		paint.TextSize = 20;
		canvas.DrawText("Press Space to get back to MainMenu", SizeX / 2 - paint.MeasureText("Press Space to get back to MainMenu") / 2, SizeY / 3 * 2, paint);
	}

	private void showGame(double arg1){
		
		if(meteorTimer >= 0.8){
			meteorTimer = 0;
			for (int i = 0; i < difficulty; i++) createMeteor();
			score += 1;
			if (score % 7 == 0) difficulty += 1;
		}

		meteorTimer += arg1;

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
		List<GameObject> toRemove = new List<GameObject>();
		foreach (GameObject gameObject in gameobjects){
			if (gameObject.Move(arg1, gameobjects))toRemove.Add(gameObject);
		}

		foreach (GameObject gameObject in toRemove) gameobjects.Remove(gameObject);
		if(!gameobjects.Contains(player)) currentLevel = 2;
	}
	private void showGameRender(){
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
		paint.TextSize = 20;
		canvas.DrawText($"Score: {score}", 50, 50, paint);
	}

	private void showMainMenu(double arg1){
		if (pressedKeys.Contains(Key.Space)){
			pressedKeys.Remove(Key.Space);
			currentLevel = 1;
			difficulty = 1;
			createObjects();
			meteorTimer = 0;
			score = 0;
		}
	}

	private void showMainMenuRender(){
		paint.TextSize = 20;
		canvas.DrawText($"Highscore: {userPreferences.HighScore}", SizeX / 2 - paint.MeasureText($"Highscore: {userPreferences.HighScore}") / 2, 100, paint);
		
		paint.TextSize = 36;
		canvas.DrawText("Dodge Em All", SizeX / 2 - paint.MeasureText("Dodge Em All") / 2, SizeY / 2, paint);
		
		paint.TextSize = 20;
		canvas.DrawText("Press Space to get start dodging", SizeX / 2 - paint.MeasureText("Press Space to get start dodging") / 2, SizeY / 3 * 2, paint);
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
	


	private void swapBuffers(){
		canvas.Flush();
		window.SwapBuffers();
		canvas.Clear(SKColors.DarkSlateGray);
	}

	private void createObjects(){
		//create all variables
		pressedKeys = new List<Key>();
		gameobjects = new List<GameObject>();
		//oCanvas.DrawCircle(new SKPoint(0, 0), 150/2, paint);
		var data = SKData.Create(@"art/spaceshipsmall.png");
		var playerimg = SKImage.FromEncodedData(data);
		if(playerimg == null) {
			window.Close();
			return;
		}
		player = new GameObject(window.Size.X/2, window.Size.Y/2, playerimg, "Player", true, "Player");
		gameobjects.Add(player);

		data = SKData.Create(@"art/meteorsmall.png");
		meteorTexture = SKImage.FromEncodedData(data);
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
			velx = random.Next(10, 250);
			vely = -random.Next(100, 250);
		}else if(rand < 50){
			posx = random.Next(SizeX);
			posy = SizeY;
			velx = random.Next(100, 250);
			vely = random.Next(100, 250);
		}else if(rand < 75){
			posx = 0;
			posy = random.Next(SizeY);
			velx = random.Next(100, 250);
			vely = random.Next(100, 250);
		}else{
			posx = SizeX;
			posy = random.Next(SizeY);
			velx = -random.Next(100, 250);
			vely = random.Next(100, 250);
		}
		var meteor = new GameObject(posx, posy, meteorTexture, "Meteor", false, "Meteor");
		meteor.AddVelocity(velx, vely);
		gameobjects.Add(meteor);
	}

	void onClosing(){
		userPreferences.Save();
	}
}