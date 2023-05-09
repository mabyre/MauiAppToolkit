## MauiAppToolkit

Toolkit denomination take place in simple code that can be used to make simple integration.

This project comes from the need of understanding what happend on Android plateform
when you save a file the content is not saved it just disapear in cache memory
what is the way to save a file for exemple in OneDrive...

storage/emulated/0/Android/data/com.compagnyname.appname

### Getting started

This application is a Flyout Tab ContentPage :

```xaml
<FlyoutItem Title="Console" Route="consolepage" FlyoutIcon="{StaticResource IconTwo}">
    <Tab Title="File" Icon="{StaticResource IconOneTab}">
        <ShellContent
            Title="One"
            ContentTemplate="{DataTemplate local:MainPage}"
            Route="mainpage" />
    </Tab>
```

