using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine.UI;
using System;

public class Test : MonoBehaviour {

    public GameObject LoggedInUi, NotLoggedInUI, Friend;

	void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback);
        }
        ShowUI();
    }

    private void InitCallback()
    {
        Debug.Log("Facebook has been initialized");
    }

    public void Login()
    {
        if (!FB.IsLoggedIn)
        {
            FB.LogInWithReadPermissions(new List<String> { "user_friends" }, LoginCallback);
        }
    }

    void LoginCallback(ILoginResult result)
    {
        if(result.Error == null)
        {
            Debug.Log("FB has logged in.");
            ShowUI();
        }
        else
        {
            Debug.Log("Error during login: " + result.Error);
        }
    }

    void ShowUI()
    {
        if (FB.IsLoggedIn)
        {
            LoggedInUi.SetActive(true);
            NotLoggedInUI.SetActive(false);
            FB.API("me/picture?width=100&height=100", HttpMethod.GET, PictureCallBack);
            FB.API("me?fields=first_name", HttpMethod.GET, NameCallBack);
            FB.API("me/friends", HttpMethod.GET, FriendsCallBack);
        }
        else
        {
            LoggedInUi.SetActive(false);
            NotLoggedInUI.SetActive(true);
        }
    }

    private void FriendsCallBack(IGraphResult result)
    {
        IDictionary<string, object> data = result.ResultDictionary;
        List<object> friends = (List<object>)data["data"];
        foreach(object obj in friends)
        {
            Dictionary<string, object> dictio = (Dictionary<string, object>)obj;
            CreateFriend(dictio["name"].ToString() , dictio["id"].ToString());
        }
    }

    private void NameCallBack(IGraphResult result)
    {
        //Debug.Log(result.RawResult);
        IDictionary<string, object> profile = result.ResultDictionary;
        LoggedInUi.transform.FindChild("Name").GetComponent<Text>().text = "Hello " + profile["first_name"];
    }

    private void PictureCallBack(IGraphResult result)
    {
        Texture2D image = result.Texture;
        LoggedInUi.transform.FindChild("ProfilePicture").GetComponent<Image>().sprite = Sprite.Create(image, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f));
    }

    public void LogOut()
    {
        FB.LogOut();
        ShowUI();
    }
    public void Invite()
    {
        FB.ShareLink(new System.Uri("https://play.google.com/store/apps/developer?id=ShiandeApps"),"This game is awesome!","A Description of the game", new System.Uri("https://play.google.com/store/apps/developer?id=ShiandeApps"));
    }

    public void Share()
    {
        FB.AppRequest(message: "You should really try this game.", title: "Check this super game!");
    }

    void CreateFriend(string name, string id)
    {
        GameObject myFriend = Instantiate(Friend);
        Transform parent = LoggedInUi.transform.FindChild("ListContainer").FindChild("FriendList");
        myFriend.transform.SetParent(parent);
        myFriend.GetComponentInChildren<Text>().text = name;
        FB.API(id + "/picture?width=100&height=100", HttpMethod.GET, delegate (IGraphResult result)
        {
            myFriend.GetComponentInChildren<Image>().sprite = Sprite.Create(result.Texture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f));
        });
    }
}
