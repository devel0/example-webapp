import { PaletteMode } from "@mui/material";

//------------------------------------------------------------------
// APP LOCAL STORAGE
//------------------------------------------------------------------

export const LOCAL_STORAGE_LANG = "lang";
export const LOCAL_STORAGE_THEME = "theme";
export const LOCAL_STORAGE_CURRENT_USER_NFO = "currentUserNfo";

//------------------------------------------------------------------
// CSS VARS
//------------------------------------------------------------------

export const CSS_VAR_TOOLBAR_TEXT_COLOR = "--toolbar-text-color";

//------------------------------------------------------------------
// APP
//------------------------------------------------------------------

export const APP_TITLE = "ExampleWebApp";
export const APP_HOME_TEXT = "ExampleWebApp";
export const API_URL = "http://localhost:5000"; // test
export const JWT_Token = "ExampleWebApp";

export const APP_URL_Home = "/";
export const APP_URL_Login = "/login";
export const APP_URL_Admin_Users = "/admin/users";

//------------------------------------------------------------------
// I18N
//------------------------------------------------------------------

export type LANG_TYPES = "it" | "en" | "de";
export const LANG_INITIAL: LANG_TYPES = "it";

//------------------------------------------------------------------
// GUI
//------------------------------------------------------------------

export const THEME_DARK = "dark";
export const THEME_LIGHT = "light";
export const THEME_INITIAL: PaletteMode = THEME_LIGHT;
