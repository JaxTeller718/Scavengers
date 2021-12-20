This change modifies the UI to allow recipes to have infinite ingredients!


File : windows.xml
Window : craftingInfoPanel
 
Add following new UI : 
	
<!-- NEW PAGINATION -->
<panel pos="480,0" width="104" height="43" disableautobackground="true">
	<button depth="4" name="pageDown" style="icon30px, press" pos="20,-22" sprite="ui_game_symbol_arrow_left" pivot="center" sound="[paging_click]" />
	<rect depth="4" pos="37,-7" >
		<sprite name="background" style="icon30px" color="[black]" type="sliced" />
		<label depth="5" name="pageNumber" pos="0, -3" width="30" height="28" text="1" font_size="26" justify="center"/>
	</rect>
	<button depth="4" name="pageUp" style="icon30px, press" pos="84,-22" sprite="ui_game_symbol_arrow_right" pivot="center" sound="[paging_click]" />
</panel>
<!-- NEW PAGINATION -->
         
Add the above section ABOVE the following existing line : <rect depth="1" pos="153,-124" name="ingredients" width="447" height="264">


NOTE !!!
The IngredientList controller in windows.xml is not a RH contoller : 
 <grid rows="6" width="447" height="231" cell_height="50" cell_width="447" controller="RH_IngredientList, Mods" arrangement="vertical">