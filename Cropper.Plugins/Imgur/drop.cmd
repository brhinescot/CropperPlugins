@setlocal

@set plugin=SendToImgur
@set pluginOutputDir=..\Output\x86\Debug
@set cropperPluginDir="\Program Files (x86)\Fusion8Design\Cropper\Plugins"

copy %pluginOutputDir%\Cropper.%plugin%.dll         %cropperPluginDir%
copy %pluginOutputDir%\Cropper.%plugin%.pdb         %cropperPluginDir%

@endlocal
