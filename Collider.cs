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
		set {origin = value;}
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
		set {origin = value;}
		get{return origin;}
	}
	public bool checkCollision(CircleCollider collider){
		
		return false;
	}

	public void MoveOrigin(float x, float y){
		origin.X += x;
		origin.Y += y;
	}
	public bool checkCollision(RectangleCollider collider){
		bool xT = origin.X >= collider.origin.X && origin.X <= collider.origin.X + collider.width;
		bool yT = origin.Y >= collider.origin.Y && origin.Y <= collider.origin.Y + collider.height;
		bool xB = origin.X + width >= collider.origin.X && origin.X + width <= collider.origin.X + collider.width;
		bool yB = origin.Y + height >= collider.origin.Y && origin.Y + height <= collider.origin.Y + collider.height;
		
		//following Console lines are used for debugging
		//Console.WriteLine($"{xT}, {yT}, {xB}, {yB}");
		if ((xT || xB) && (yT || yB)){
				return true;
			}
		//Console.WriteLine("Collision Rect zu Rect False");
		return false;
	}

	public RectangleCollider(SKPoint pos, float w, float h){
		origin = pos;
		width = w;
		height = h;
	}
}