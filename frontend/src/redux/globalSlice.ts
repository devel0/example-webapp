import { PayloadAction, createSlice } from "@reduxjs/toolkit";
import { GlobalState } from "../types/GlobalState";
import {
  APP_URL_Login,
  LANG_INITIAL,
  LANG_TYPES,
  LOCAL_STORAGE_CURRENT_USER_NFO,
  LOCAL_STORAGE_LANG,
  LOCAL_STORAGE_THEME,
  THEME_INITIAL,
} from "../constants";
import { PaletteMode } from "@mui/material";
import { PostSetTheme } from "../utils";
import {
  currentUserAsyncThunk,
  listUsersAsyncThunk,
  loginAsyncThunk,
  logoutAsyncThunk,
} from "./auth";
import {
  CurrentUserResponseDto,
  LoginResponseDto,
  UserListItemResponseDto,  
} from "../api";
import { CurrentUserNfo } from "../types/CurrentUserNfo";

// recupera theme da local storage
let currentTheme = THEME_INITIAL;
{
  const themeSaved = localStorage.getItem(LOCAL_STORAGE_THEME);
  if (themeSaved as PaletteMode) currentTheme = themeSaved as PaletteMode;
  PostSetTheme(currentTheme);
}

// recupera lang da local storage
let currentLang = LANG_INITIAL;
{
  const langSaved = localStorage.getItem(LOCAL_STORAGE_LANG);
  if (langSaved as LANG_TYPES) currentLang = langSaved as LANG_TYPES;
}

// recupera utente autenticato da local storage
let initialCurrentUser: CurrentUserNfo | undefined = undefined;
{
  const qLocalStorage = localStorage.getItem(LOCAL_STORAGE_CURRENT_USER_NFO);
  if (qLocalStorage) initialCurrentUser = JSON.parse(qLocalStorage);
}

const initialState: GlobalState = {
  theme: currentTheme,
  lang: currentLang,

  currentUser: initialCurrentUser,
  usersList: undefined,
};

export const globalSlice = createSlice({
  name: "global",

  initialState,

  reducers: {
    setLang: (state, action: PayloadAction<LANG_TYPES>) => {
      const newLang = action.payload;
      state.lang = newLang;

      localStorage.setItem(LOCAL_STORAGE_LANG, newLang);

      console.log(`set lang to ${newLang}`);
    },

    setTheme: (state, action: PayloadAction<PaletteMode>) => {
      const theme = action.payload;
      state.theme = theme;

      localStorage.setItem(LOCAL_STORAGE_THEME, theme);

      PostSetTheme(theme);
    },
  },

  extraReducers: (builder) => {
    // login
    builder.addCase(
      loginAsyncThunk.fulfilled,
      (state, action: PayloadAction<LoginResponseDto>) => {
        const res = action.payload;

        if (res.userName && res.email && res.roles) {
          state.currentUser = {
            userName: res.userName,
            email: res.email,
            roles: res.roles,
          };
          console.log(
            `logged in, saved currentUser ${action.payload.userName} to state `
          );

          localStorage.setItem(
            LOCAL_STORAGE_CURRENT_USER_NFO,
            JSON.stringify(state.currentUser)
          );
        }
      }
    );

    // current user
    builder.addCase(
      currentUserAsyncThunk.fulfilled,
      (state, action: PayloadAction<CurrentUserResponseDto>) => {
        const res = action.payload;

        if (res.userName && res.email && res.roles) {
          state.currentUser = {
            userName: res.userName,
            email: res.email,
            roles: res.roles,
          };
          console.log(
            `read current user ${action.payload.userName} saved to state `
          );

          localStorage.setItem(
            LOCAL_STORAGE_CURRENT_USER_NFO,
            JSON.stringify(state.currentUser)
          );
        }
      }
    );

    // list users
    builder.addCase(
      listUsersAsyncThunk.fulfilled,
      (state, action: PayloadAction<UserListItemResponseDto[]>) => {
        const res = action.payload;

        state.usersList = res
      }
    );

    builder.addCase(
      listUsersAsyncThunk.rejected,
      (state, action) => {
        console.log(`rejected`)      
      }
    );

    // logout
    builder.addCase(
      logoutAsyncThunk.fulfilled,
      (state, action: PayloadAction<void>) => {
        const res = action.payload;

        localStorage.removeItem(LOCAL_STORAGE_CURRENT_USER_NFO);

        window.location.href = APP_URL_Login;
      }
    );

    builder.addCase(logoutAsyncThunk.rejected, (state) => {
      localStorage.removeItem(LOCAL_STORAGE_CURRENT_USER_NFO);

      window.location.href = APP_URL_Login;
    });
  },
});

export const { setLang, setTheme } = globalSlice.actions;

export default globalSlice.reducer;
