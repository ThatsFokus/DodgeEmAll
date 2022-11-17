class UserPreferences{
	public int HighScore;
	private static readonly string file = "settings.pref";
	public void Load(){
		if(!System.IO.File.Exists(file)){
			Save();
			return;
		}
		var text = System.IO.File.ReadAllText(file);
		var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<UserPreferences>(text);
		if (obj != null){
			HighScore = obj.HighScore;
			return;
		}
		HighScore = 0;
		Save();
	}

	public void Save(){
		var text = Newtonsoft.Json.JsonConvert.SerializeObject(this);
		System.IO.File.WriteAllText(file, text);
	}

	public UserPreferences(){
		this.HighScore = 0;
	}
}