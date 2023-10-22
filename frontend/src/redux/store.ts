// generic: https://redux.js.org/tutorials/quick-start
// TS specific: https://react-redux.js.org/tutorials/typescript-quick-start

import {
  combineReducers,
  configureStore,  
} from "@reduxjs/toolkit";
import globalReducer from "./globalSlice";
import { createReduxHistoryContext } from 'redux-first-history'
import { createBrowserHistory } from 'history'

const {
  createReduxHistory,
  routerMiddleware,
  routerReducer
} = createReduxHistoryContext({ history: createBrowserHistory() });

export const store = configureStore({
  reducer: combineReducers({
      router: routerReducer,
      global: globalReducer,      
  }),
  middleware: (getDefaultMiddleware) => getDefaultMiddleware().concat(routerMiddleware),
});

export const history = createReduxHistory(store);

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>;

// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch;