import { createAsyncThunk } from "@reduxjs/toolkit";
import { authApi } from "../global";
import { LoginRequestDto } from "../api";

export const currentUserAsyncThunk = createAsyncThunk(
  "initCurrentUser",
  async (thunkApi) => {
    const api = authApi();
    const response = await api.currentUser();    

    return response.data;
  }
);

export const listUsersAsyncThunk = createAsyncThunk(
  "listUsers",
  async (thunkApi) => {
    const api = authApi();
    const response = await api.listUsers();    

    return response.data;
  }
);

export const loginAsyncThunk = createAsyncThunk(
  "login",
  async (req: LoginRequestDto, thunkApi) => {
    const api = authApi();
    const response = await api.login({
      email: req.email,
      password: req.password,
    });

    return response.data;
  }
);

export const logoutAsyncThunk = createAsyncThunk("logout", async (thunkApi) => {
  const api = authApi();
  const response = await api.logout();

  return response.data;
});
