using SkiaSharp;
class GameObject{
	private float positionX;
	private float positionY;

	public SKPoint Position{
		get{
			return new SKPoint(positionX, positionY);
		}
	}

	public SKImage Texture{
		get{ return texture;}
	}
	private float drag = 3;
	private SKImage texture;
	private float velX;
	private float velY;
	private RectangleCollider rectangleCollider;
	private string tag;
	private string name;
	public string Name{get{return name;}}
	bool loosesVelocity;
	public GameObject(){
		positionX = 0;
		positionY = 0;
		velX = 0;
		velY = 0;
		var iminfo = new SKImageInfo(25, 25);
		var bmap = new SKBitmap(iminfo);
		var canvas = new SKCanvas(bmap);
		canvas.Clear(SKColors.Black);
		canvas.Flush();
		texture = SKImage.FromBitmap(bmap);
		rectangleCollider = new RectangleCollider(Position, texture.Width, texture.Height);
		tag = "";
		this.name = "GameObject";
		loosesVelocity = false;
	}

	public GameObject(float posx, float posy,SKImage texture, string name, bool loosesVelocity , string tag = ""){
		positionX = posx;
		positionY = posy;
		this.loosesVelocity = loosesVelocity;
		velX = 0;
		velY = 0;
		this.name = name;
		this.tag = tag;
		this.texture = texture;
		rectangleCollider = new RectangleCollider(Position, texture.Width, texture.Height);
	}

	public bool Move(double deltaTime, List<GameObject> gameObjects){
		if (velX < 1 && velX > -1) velX = 0;
		if (velY < 1 && velY > -1) velY = 0;

		positionX += (velX * ((float)deltaTime));
		positionY -= (velY * ((float)deltaTime));

		rectangleCollider.Origin = Position;

		if (tag == "Player"){
			if (checkCollision(gameObjects)) return true;
		}

		if (loosesVelocity){
			velX -= velX*((float)deltaTime) * drag;
			velY -= velY*((float)deltaTime) * drag;
		}

		if(tag == "Player"){
			if(positionX < 0) positionX = Mygame.SizeX - positionX;
			else if(positionX > Mygame.SizeX) positionX = positionX - Mygame.SizeX;
			if(positionY < 0) positionY = Mygame.SizeY - positionY;
			else if(positionY > Mygame.SizeY) positionY = Mygame.SizeY - Mygame.SizeY;
		}
		else if (tag == "Meteor"){
			if(positionX < -texture.Width || positionX  > Mygame.SizeX || positionY < - texture.Height || positionY  > Mygame.SizeY){
				return true;
			}
		}

		return false;
	}

	public void AddVelocity(float x, float y){
		velX += x;
		velY += y;
	}

	public bool CompareTag(string t){
		return t == tag;
	}

	public SKRect getColliderRect(){
		return new SKRect(rectangleCollider.Origin.X, rectangleCollider.Origin.Y, rectangleCollider.Width + rectangleCollider.Origin.X, rectangleCollider.Height + rectangleCollider.Origin.Y);
	}

	private bool checkCollision(List<GameObject> gameObjects){
		foreach (GameObject gameObject in gameObjects){
			if (gameObject == this) continue;
			if (rectangleCollider.checkCollision(gameObject.rectangleCollider)) return true;
			}
			return false;
	}
}