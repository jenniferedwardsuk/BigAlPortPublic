internal static partial class FileContents
{
	internal static string javascriptJs = @"function popwin_ff(url, name)
		{
			window.open(url, name, 'status=no,scrollbars=yes,resizable=yes,width=500,height=400');
			window.name = 'dino';
		}

		function do_action(val)
		{
			var f = document.forms[0];
			var current = f.action.value;

			//Check if the action has lower precidence than the current one
			if (
				current.indexOf(""BeAttacked"") == -1 ||
				(val.indexOf(""Wait"") == -1 && val.indexOf(""Eat"") == -1 && val.indexOf(""Attack"") == -1)
			  )
			{
				//Do the action
				f.action.value = val;
			}
			f.submit();
		}

		function switch_img(img, src)
		{
			document.images[img].src = src;
		}

		// Args: img,src, img,src, ...
		function switch_imgs()
		{
			var args = switch_imgs.arguments;
			for (i = 0; i < args.length; i += 2)
			{
				var img = args[i];
				var src = args[i + 1];
				document.images[img].src = src;
			}
		}

		// Args: src, src, ...
		function preload_images()
		{
			if (document.images)
			{
				if (!document.imgarray) document.imgarray = new Array();
				var len = document.imgarray.length;
				var args = preload_images.arguments;
				for (i = 0; i < args.length; i++)
				{
					document.imgarray[len] = new Image;
					document.imgarray[len].src = args[i];
					len++;
				}
			}
		}";
}