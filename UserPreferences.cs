class UserPreferences{
	public bool useFullscreen;
	private static readonly string file = "settings.pref";
	public void Load(){
		if(!System.IO.File.Exists(file)){
			Save();
			return;
		}
		var text = System.IO.File.ReadAllText(file, System.Text.Encoding.UTF8);
		var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<UserPreferences>(text);
		if (obj != null){
			useFullscreen = obj.useFullscreen;
			return;
		}
		useFullscreen = false;
		Save();
	}

	public void Save(){
		var text = Newtonsoft.Json.JsonConvert.SerializeObject(this);
		System.IO.File.WriteAllText(file, text, System.Text.Encoding.UTF8);
	}

	public UserPreferences(bool useFullscreen = false){
		this.useFullscreen = useFullscreen;
	}
}