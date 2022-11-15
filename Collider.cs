using SkiaSharp;
class Collider{
	public static RectangleCollider getCollider(SKPoint origin, float width, float height){
		return new RectangleCollider(origin, width, height);
	}
	public static CircleCollider getCollider(SKPoint origin, float radius){
		return new CircleCollider(origin, radius);
	}
}

class CircleCollider : Collider{
	float radius;
	public float Radius{
		get{ return radius;}
	}
	SKPoint origin;
	public SKPoint Origin{
		get{return origin;}
	}
	public CircleCollider(SKPoint o, float r){
		origin = o;
		radius = r;
	}

	public bool checkCollision(CircleCollider collider){
		return false;
	}
	public bool checkCollision(RectangleCollider collider){
		return false;
	}
}

class RectangleCollider : Collider{
	float width;
	float height;
	SKPoint origin;
	public float Width{
		get{return width;}
	}
	public float Height{
		get{return height;}
	}
	public SKPoint Origin{
		get{return origin;}
	}
	public bool checkCollision(CircleCollider collider){
		
		return false;
	}
	public bool checkCollision(RectangleCollider collider){
		
		if	(this.origin.X + this.width >= collider.origin.X && 
			this.origin.X <= collider.width + collider.origin.X && 
			this.origin.Y + this.height >= collider.origin.Y && 
			this.origin.Y <= collider.height + collider.origin.Y){
				Console.WriteLine("Collision Rect zu Rect True");
				return true;
			}
		Console.WriteLine("Collision Rect zu Rect False");
		return false;
	}

	public RectangleCollider(SKPoint pos, float w, float h){
		origin = pos;
		width = w;
		height = h;
	}
}