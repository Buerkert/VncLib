# VncLib

Implementation of the VNC Remote Framebuffer (RFB) Protocol

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

* .NET Framework 4.5.2 or higher
* Visual Studio 2017 Community or higher

### Installing

Download the latest release, see the [Releases](https://github.com/patdhlk/vnclib/releases) and add a reference in your .NET application or build it yourself using msbuild or/and vs2017.

Then you can use it in your ViewModel or somewhere you want to display the remote desktop.

```cs
VncConnection vncConnection = new VncConnection(bindAddress, port, password);
//create a RemoteDesktop instance
RemoteDesktop remoteDesktop = new RemoteDesktop(vncConnection);
//and, e.g. use it in your ViewModel;
RemoteDesktopViewModel vm = new RemoteDesktopViewModel(remoteDesktop);
VncRemoteDesktopView view = new VncRemoteDesktopView();
view.DataContext = vm;
return view;
```

In your ViewModel you can handle the desktop changed event:

```cs
public RemoteDesktopViewModel()
{
    _remoteDesktop.DesktopUpdated += RemoteDesktopOnDesktopUpdated;
}

private void RemoteDesktopOnDesktopUpdated(object sender, Bitmap bitmap)
{
    var bit = new Bitmap(bitmap);
    Application.Current.Dispatcher.BeginInvoke(new Action(() => 
    { 
        Desktop = RemoteDesktop.ConvertBitMapToBitmapImage(bit); 
    }));
}
```
And in your View you have a Binding to the `Desktop` property

A little demo will be added here in a few days.

## Built With

* [zlib.NET](http://www.componentace.com/zlib_.NET.htm/) - ZLIB.NET is a 100% managed version of ZLIB compression library which implements deflate and inflate compression algorithms.

## Contributing

Please read [CONTRIBUTING.md](https://github.com/patdhlk/vnclib/blob/master/CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [releases on this repository](https://github.com/patdhlk/vnclib/releases). 

## Authors

* **patdhlk** - *Initial work* - [patdhlk](https://github.com/patdhlk)

See also the list of [contributors](https://github.com/patdhlk/vnclib/contributors) who participated in this project.

## License

This project is licensed under the three-clause BSD license - see the [LICENSE](LICENSE) file for details
