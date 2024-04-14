using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

public class Jukebox : MonoBehaviour{
public static Jukebox INSTANCE;
	public AudioMixer audioMixer;
	public AudioMixerGroup mixerGroup;
	//[SerializeField]public bool muteInsteadOfStopping;
	[AssetsOnly,Searchable][SerializeField]public GMusic[] musics;
	[AssetsOnly,Searchable][SerializeField]public _GMusicRef[] _musicsRef;
	[Searchable][HideInEditorMode][SerializeField]List<GMusic> notFadedInList;
    public float fadeDuration = 1.0f;
    private float fadeTimer = 0.0f;
    void Awake(){
        fadeTimer=fadeDuration;
        if(Jukebox.INSTANCE!=null){Destroy(gameObject);}else{INSTANCE=this;DontDestroyOnLoad(gameObject);}

        foreach(GMusic s in musics){
            notFadedInList.Add(s);
            s.source=gameObject.AddComponent<AudioSource>();
            s.source.clip=s.clip;
            s.source.volume=s.volume;
            s.source.pitch=s.pitch;
            s.source.loop=true;
            s.source.playOnAwake=false;

            s.source.outputAudioMixerGroup=mixerGroup;
        }
    }
    void Update(){
        if(fadeTimer < fadeDuration){
            fadeTimer += Time.deltaTime;
            float normalizedTime = fadeTimer / fadeDuration;
            foreach(GMusic s in notFadedInList){
                float volume = Mathf.Lerp(0f, s.volume, normalizedTime);
                s.source.volume = volume;
            }
        }else{
            notFadedInList = null;
        }
    }
    public void PlayMusic(string sound){
        GMusic s=Array.Find(musics,x=>x.name==sound);
		if(s==null){
			Debug.LogWarning("Music: "+sound+" not found!");
			return;
  		}
        else{if(s.source!=null){
            if(!s.source.isPlaying){
                StopAllMusic();
                s.source.Play();
                s.source.volume=s.volume;
            }else{UnmuteMusic(sound);}
        }else{Debug.LogWarning("No source for: "+sound);}}
    }
    public void PlayMusicForce(string sound){
        GMusic s=Array.Find(musics,x=>x.name==sound);
		if(s==null){
			Debug.LogWarning("Music: "+sound+" not found!");
			return;
  		}
        else{if(s.source!=null){
            if(!s.source.isPlaying){
                s.source.Play();
                s.source.volume=s.volume;
            }else{UnmuteMusic(sound);}
        }else{Debug.LogWarning("No source for: "+sound);}}
    }
    public void PlayMusicMuted(string sound){
        GMusic s=Array.Find(musics,x=>x.name==sound);
		if(s==null){
			Debug.LogWarning("Music: "+sound+" not found!");
			return;
  		}
        else{if(s.source!=null){
            if(!s.source.isPlaying){
                s.source.Play();
                s.source.volume=0;
            }
        }else{Debug.LogWarning("No source for: "+sound);}}
    }
    public void StopMusic(string sound){
        GMusic s=Array.Find(musics,x=>x.name==sound);
		if(s==null){
			Debug.LogWarning("Music: "+sound+" not found!");
			return;
  		}
        else{if(s.source!=null){
		    if(s.source.isPlaying){
                /*if(!s.ignoreStop&&!muteInsteadOfStopping){*/s.source.Stop();//}
                //else if(s.ignoreStop||muteInsteadOfStopping){MuteMusic(s.name);}
            }
        }else{Debug.LogWarning("No source for: "+sound);}}
    }
    public void StopMusicForce(string sound){
        GMusic s=Array.Find(musics,x=>x.name==sound);
		if(s==null){
			Debug.LogWarning("Music: "+sound+" not found!");
			return;
  		}
        else{if(s.source!=null){
		    if(s.source.isPlaying)s.source.Stop();
        }else{Debug.LogWarning("No source for: "+sound);}}
    }
    public void StopAllMusic(){
        foreach(GMusic s in musics){
            if(s.source!=null){if(s.source.isPlaying){
                /*if(!s.ignoreStop&&!muteInsteadOfStopping){*/s.source.Stop();//}
                //else if(s.ignoreStop||muteInsteadOfStopping){MuteMusic(s.name);}
            }}else{Debug.LogWarning("No source for: "+s.name);}
        }
    }
    public void FadeInStart(){
        fadeTimer=0;
        foreach(GMusic s in musics){
            if(s.source!=null){
                if(s.source.isPlaying && notFadedInList.Contains(s)){
                    s.source.volume = Mathf.Lerp(0, s.volume, Time.fixedDeltaTime);
                    if(s.source.volume >= s.volume){notFadedInList.Remove(s);}
                }
            }
        }
    }


    public void MuteMusic(string sound){
        GMusic s=Array.Find(musics,x=>x.name==sound);
		if(s==null){
			Debug.LogWarning("Music: "+sound+" not found!");
			return;
        }
		else{if(s.source!=null){
            s.source.volume=0;
        }else{Debug.LogWarning("No source for: "+sound);}}
    }
    public void UnmuteMusic(string sound){
        GMusic s=Array.Find(musics,x=>x.name==sound);
		if(s==null){
			Debug.LogWarning("Music: "+sound+" not found!");
			return;
  		}
        else{if(s.source!=null){
		    s.source.volume=s.volume;
        }else{Debug.LogWarning("No source for: "+sound);}}
    }


    public AudioClip GetMusic(string sound){
		GMusic s=Array.Find(musics,x=>x.name==sound);
		if(s==null){
			Debug.LogWarning("Music: "+sound+" not found!");
			return null;
		}else{return s.clip;}
	}
	public AudioSource GetMusicSource(string sound){
		GMusic s=Array.Find(musics,x=>x.name==sound);
		if(s==null){
			Debug.LogWarning("Music: "+sound+" not found!");
			return null;
		}else{if(s.source!=null){return s.source;}else{Debug.LogWarning("No source for: "+sound);return null;}}
	}

    public string GetNameRef(string str){string _str="";
        _str=Array.Find(_musicsRef,x=>x.name==str)._nameRef;
        return _str;
    }
}


[System.Serializable]
public class GMusic{
	public string name;
	public AudioClip clip;
	
	[Range(0f,1f)]
	public float volume=.75f;

	[Range(.1f,3f)]
	public float pitch=1f;

	//public bool ignoreStop;

	[DisableInEditorMode]
	public AudioSource source;
}

[System.Serializable]
public class _GMusicRef{
    public string name;
    public string _nameRef;
}