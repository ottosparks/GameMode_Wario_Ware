//PREFACE TO CODERS:
//I code around the concept of object-oriented programming.
//I use objects in cases where they may not necessarily be needed, and I am aware of this.
//I feel like it's more 'natural' to code this way, and also think it helps people understand how it works more easily.
//File complaints here: https://dl.dropbox.com/u/11058668/WarioWare/dev/complaint.html
if($GameModeArg !$= "Add-Ons/GameMode_Wario_Ware/gamemode.txt" && !$WWDebug)
{
	warn("Wario Ware is in building mode...");
	$WW::BuildingMode = true;
}
%r = $WW::Dir::Root = "Add-Ons/GameMode_Wario_Ware/";
$WW::Dir::Msg = %r @ "msg/";
$WW::Dir::Script = %r @ "script/";
$WW::Dir::Micro = $WW::Dir::Script @ "micro";
$WW::Dir::Data = %r @ "data/";

$WW::BrickGroup = BrickGroup_888888;

function loadWarioWare()
{
	exec($WW::Dir::Data @ "datablocks.cs");
	exec($WW::Dir::Script @ "main.cs");
}

function createDataNameTable()
{
	%ct = DataBlockGroup.getCount();
	for(%i = 0; %i < %ct; %i++)
	{
		%obj = DataBlockGroup.getObject(%i);
		if((%obj.getClassName() !$= "PlayerData" && %obj.getClassName() !$= "ShapeBaseImageData" && %obj.getClassName() !$= "ItemData" && %obj.getClassName() !$= "AudioProfile") || %obj.uiName $= "")
			continue;
		$dataNameTable[%obj.uiName] = %obj.getID();
	}
	$dataNameTableCreated = true;
}
loadWarioWare();