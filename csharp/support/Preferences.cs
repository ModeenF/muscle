
using muscle.message;

///<summary>
/// Encapsulates a Message for easier use as a Preference holder.
/// Taken from Bryan Varner's Java code
///</summary>
///
public class Preferences {
    File _prefsFile;
    protected Message _prefs;
   
    ///<summary>
    /// Creates a Preferences object with a blank message, that dosen't have a save file.
    ///</summary>
    ///
    public Preferences() {
        _prefs = new Message();
        _prefsFile = null;
    }
   
    ///<summary>
    ///Creates a Preferences object with a default message, that dosen't have a save file.
    ///</summary>
    ///
    public Preferences(Message defaults) {
        _prefs = defaults;
        _prefsFile = null;
    }
   
    ///<summary>
    ///Creates a new Preferences from the given file.
    /// <param name="loadFile>the File to load the preferences from.</param>
    /// <exception cref="IOException"/>
    ///</summary>
    ///
    public Preferences(String loadFile) {
        _prefs = new Message();
        _prefsFile = new File(loadFile);     
        DataInputStream fileStream = new DataInputStream(new FileInputStream(_prefsFile));
        try {
            _prefs.unflatten(fileStream, -1);
        } catch (Exception e) {
            // You are screwed.
        }
    }

    ///<summary>
    /// Creates a new Preferences instance using defaults for the initial settings, then over-writing any defaults with the values from loadFile.
    /// <param name="loadFile>A Flattened Message to load as Preferences.</param>
    /// <param name="defaults>An existing Message to use as the base.</param>
    ///</summary>
    ///
    public Preferences(String loadFile, Message defaults) {
        _prefs = defaults;
        try {
            _prefsFile = new File(loadFile);
            DataInputStream fileStream = new DataInputStream(new FileInputStream(_prefsFile));
            _prefs.unflatten(fileStream, -1);
        } catch (Exception e) {
            // You are screwed.
        }
    }
   
    ///<summary>
    /// Saves the preferences to the file they were opened from.
    /// <param name="loadFile>A Flattened Message to load as Preferences.</param>
    /// <param name="defaults>An existing Message to use as the base.</param>
    /// <exception cref="IOException"/>
    ///</summary>
    ///
    public void save(){
        if (_prefsFile != null) {
            save(_prefsFile.getAbsolutePath());
        }
    }

    ///<summary>
    /// Saves the preferences to the file specified.
    /// <param name="saveAs>the file to save the preferences to.</param>
    /// <exception cref="IOException"/>
    ///</summary>
    ///
    public void save(String saveAs) {
        DataOutputStream fileStream = new DataOutputStream(new FileOutputStream(saveAs, false));
        _prefs.flatten(fileStream);
        fileStream.close();
    }

    ///<summary>
    /// 
    /// <returns>the Message this object is manipulating.</returns>
    ///</summary>
    ///
    public Message getMessage() {
        return _prefs;
    }
}
