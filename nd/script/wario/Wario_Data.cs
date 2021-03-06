$WW::Exec::Data = true;
//Not using default player persistence because it saves things by default that we don't want.

$WW::Data::File = "config/server/WarioWareData.txt";

function WW_ReadData()
{
	if(isFile($WW::Data::File))
	{
		%file = new fileObject();
		%file.openForRead($WW::Data::File);
		while(!%file.isEOF())
		{
			%line = %file.readLine();
			%bl_id = getField(%line, 0);
			$WW::Data::ReadUser[%bl_id] = true;
		}
	}
	$WW::Data::Read = true;
}

//function WW_SavePlayers(%blids) //Optimized for multiple players at once. Unused ever because it's pretty useless.
//{
//	if(%blids $= "")
//	{
//		%count = ClientGroup.getCount();
//		if(%count < 1)
//			return;
//		if(%count == 1)
//		{
//			ClientGroup.getObject(0).SaveWario();
//			return;
//		}
//		for(%i = 0; %i < %count; %i++)
//		{
//			%client = ClientGroup.getObject(%i);
//			%blid = %client.bl_id;
//			%blids = trim(%blids SPC %blid);
//			if($WW::Data::ReadUser[%blid])
//			{
//				%blids = removeWord(%blids, findWord(%blids, %blid));
//				%readblids = trim(%readblids SPC %blid);
//			}
//		}
//	}
//	else
//	{
//		%count = getWordCount(%blids);
//		if(%count < 1)
//			return;
//		if(%count == 1)
//		{
//			findClientByBL_ID(firstWord(%count)).SaveWario();
//			return;
//		}
//		for(%i = 0; %i < %count; %i++)
//		{
//			%blid = getWord(%blids, %i);
//			if($WW::Data::ReadUser[%blid])
//				%readblids = trim(%readblids SPC %blid);
//		}
//		%count = getWordCount(%readblids);
//		for(%i = 0; %i < %count; %i++)
//			%blids = removeWord(%blids, findWord(%blids, getWord(%readblids, %i)));
//	}
//	
//	
//	if(!$WW::Data::Read)
//		WW_ReadData();
//	%file = new fileObject();
//	if(%readblids !$= "")
//	{
//		%store = new ScriptObject();
//		%file.openForRead($WW::Data::File);
//		for(%i = 0; !%file.isEOF(); %i++)
//		{
//			%line = %file.readLine();
//			%bl_id = firstWord(getField(%line, 0));
//			if(!%foundClient[%bl_id])
//			{
//				if(%word = findWord(%readblids, %bl_id) != -1)
//				{
//					%store.line[%i] = "!WRITE!" SPC %word;
//					continue;
//				}
//			}
//			%store.line[%i] = %line;
//		}
//		%store.lineCt = %i + 1;
//		%file.close();
//		%file.openForWrite($WW::Data::File);
//		for(%i = 0; %i < %store.lineCt; %i++)
//		{
//			%line = %store.line[%i];
//			%bl_id = getField(%line, 0);
//			if(firstWord(%bl_id) $= "!WRITE!")
//			{
//				%id = getWord(%readblids, getWord(%bl_id, 1));
//				%client = findClientByBL_ID(%id);
//				%file.writeLine(%id TAB "POINTS" SPC %client.score);
//				continue;
//			}
//			%file.writeLine(%line);
//		}
//		%file.close();
//	}
//	if(%blids !$= "")
//	{
//		%count = getWordCount(%blids);
//		%file.openForAppend($WW::Data::File);
//		for(%i = 0; %i < %count; %i++)
//		{
//			%bl_id = getWord(%blids, %i);
//			%client = findClientByBL_ID(%bl_id);
//			%file.writeLine(%bl_id TAB "POINTS" SPC %client.score);
//			$WW::Data::ReadUser[%this.bl_id] = true;
//		}
//		%file.close();
//	}
//	%file.delete();
//}

function GameConnection::SaveWario(%this)
{
	if(!$WW::Data::Read)
		WW_ReadData();
	%file = new fileObject();
	if($WW::Data::ReadUser[%this.bl_id])
	{
		%store = new ScriptObject();
		%file.openForRead($WW::Data::File);
		for(%i = 0; !%file.isEOF(); %i++)
		{
			%line = %file.readLine();
			%bl_id = firstWord(getField(%line, 0));
			if(!%foundClient)
			{
				if(%bl_id $= %this.bl_id)
				{
					%store.line[%i] = "!WRITE!";
					continue;
				}
			}
			%store.line[%i] = %line;
		}
		%store.lineCt = %i + 1;
		%file.close();
		%file.openForWrite($WW::Data::File);
		for(%i = 0; %i < %store.lineCt; %i++)
		{
			%line = %store.line[%i];
			if(%line $= "!WRITE!")
			{
				%string = %this.bl_id TAB "POINTS" SPC %this.score;
				if(isObject($WW::Shop))
				{
					%entries = $WW::Shop.getCount();
					for(%x = 0; %x < %entries; %x++)
					{
						%entry = $WW::Shop.getObject(%x);
						if(%this.WW_HasEntry[%entry.sname])
							%shopStr = trim(%shopStr SPC %entry.sname);
					}
				}
				%string = %string TAB "SHOP" SPC %shopStr;
				%file.writeLine(%string);
				continue;
			}
			%file.writeLine(%line);
		}
		%file.close();
	}
	else
	{
		%file.openForAppend($WW::Data::File);
		%file.writeLine(%this.bl_id TAB "POINTS" SPC %this.score);
		%file.close();
		$WW::Data::ReadUser[%this.bl_id] = true;
	}
	%file.delete();
}

function GameConnection::LoadWario(%this)
{
	%file = new fileObject();
	%file.openForRead($WW::Data::File);
	while(!%file.isEOF())
	{
		%line = %file.readLine();
		%bl_id = getField(%line, 0);
		if(%bl_id !$= %this.bl_id)
			continue;
		%pointsField = findFieldFirst(%line, "POINTS");
		if(%pointsField != -1)
			%this.score = getWord(getField(%line, %pointsField), 1);
		%shopField = findFieldFirst(%line, "SHOP");
		%entryList = restWords(getField(%line, %shopField));
		%entries = getWordCount(%entryList);
		for(%i = 0; %i < %entries; %i++)
		{
			%entry = getWord(%entryList, %i);
			if(!isObject(findShopEntryByName(%entry)))
				continue;
			%this.WW_HasEntry[%entry] = true;
			commandToClient(%this, 'WW_ShopRec', "PURCHASE" SPC %entry SPC true);
		}
		break;
	}
	%file.close();
	%file.delete();
	%this.WW_hasLoadedOnce = true;
}

//package Wario_Data
//{
//	function GameConnection::autoAdminCheck(%this)
//	{
//		%r = parent::autoAdminCheck(%this);
//		if(isObject($WW::Mini))
//			%this.LoadWario();
//		return %r;
//	}
//};
//activatePackage(Wario_Data);