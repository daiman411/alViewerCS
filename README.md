# alViewerCS

Note - 20180227
1. This is a simple example of WPF application which can display video stream via USB UVC device. 
2. It also demostrated how to wrap a unmanaged code(alAVCapDll) as a managed dll under WPF/C# aplication.
3. Unmanaged code provide an interface that end-user can use it to create an Video input object and capture raw image data from video stream. So if you got the raw image data, then you can convert it or process it whatever you want to do...This dll was programed by Direct show basically refering to Ampcap sample code.
