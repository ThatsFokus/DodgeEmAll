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
	private CircleCollider circleCollider;
	private RectangleCollider rectangleCollider;
	private string tag;
	private string name;
	public string Name{get{return name;}}
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
		tag = "";
	}

	public GameObject(float posx, float posy,SKImage texture ,bool isCircle = false, string tag = ""){
		positionX = posx;
		positionY = posy;
		velX = 0;
		velY = 0;
		this.tag = tag;
		this.texture = texture;
		if (isCircle){
			circleCollider = Collider.getCollider(Position, (float)texture.Width /2f);
		}else{
			rectangleCollider = Collider.getCollider(Position, texture.Width, texture.Height);
		}
	}

	public void Move(double deltaTime, List<GameObject> gameObjects){
		if (velX < 1 && velX > -1) velX = 0;
		if (velY < 1 && velY > -1) velY = 0;
		if (velX == 0 && velY == 0){
			return;
		}
		positionX += (velX * ((float)deltaTime));
		positionY -= (velY * ((float)deltaTime));

		var collidesWith = new List<GameObject>();
		if (velY != 0 || velX != 0){
			collidesWith = checkCollision(gameObjects);		
		}


		velX -= velX*((float)deltaTime) * drag;
		velY -= velY*((float)deltaTime) * drag;
		if(tag == "Player"){
			if(positionX < 0) positionX = Mygame.SizeX - positionX;
			else if(positionX > Mygame.SizeX) positionX = positionX - Mygame.SizeX;
			if(positionY < 0) positionY = Mygame.SizeY - positionY;
			else if(positionY > Mygame.SizeY) positionY = Mygame.SizeY - Mygame.SizeY;
		}

	}

	public void AddVelocity(float x, float y){
		velX += x;
		velY += y;
	}

	public bool CompareTag(string t){
		return t == tag;
	}

	private List<GameObject> checkCollision(List<GameObject> gameObjects){
		List<GameObject> collidesWith = new List<GameObject>();
		foreach (GameObject gameObject in gameObjects){
			if (gameObject == this) continue;
			if (this.circleCollider != null){
				if(gameObject.circleCollider != null){
					if(this.circleCollider.checkCollision(gameObject.circleCollider)){
						collidesWith.Add(gameObject);
					}
				}else{
					if(this.circleCollider.checkCollision(gameObject.rectangleCollider)){
						collidesWith.Add(gameObject);
					}
				}
			}else{
				if(gameObject.circleCollider != null){
					if(this.rectangleCollider.checkCollision(gameObject.circleCollider)){
						collidesWith.Add(gameObject);
					}
				}else{
					if(this.rectangleCollider.checkCollision(gameObject.rectangleCollider)){
						collidesWith.Add(gameObject);
					}
				}
			}
			} 
			foreach(var obj in collidesWith){
				Console.Beep();
			}
			return collidesWith;
	}
}