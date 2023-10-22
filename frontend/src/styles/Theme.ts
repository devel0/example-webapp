import { PaletteMode, ThemeOptions, createTheme } from "@mui/material";
import {
  TypographyOptions,
  TypographyStyleOptions,
} from "@mui/material/styles/createTypography";

// FONTS:
// material-ui doc: https://mui.com/material-ui/getting-started/installation/#roboto-font
// library: https://fontsource.org/
// install: https://fontsource.org/fonts/roboto/install

import "@fontsource/roboto/100.css";
import "@fontsource/roboto/300.css";
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/500.css";

import { CSS_VAR_TOOLBAR_TEXT_COLOR, THEME_INITIAL, THEME_LIGHT } from '../constants'
import { GlobalState } from '../types/GlobalState'

// follow is required to allow extends theme typography for the typescript
declare module "@mui/material/Typography" {
  interface TypographyPropsVariantOverrides {
    
  }
}

declare module "@mui/material/styles/createPalette" {
  interface PaletteOptions {
    star: PaletteColorOptions
  }
}

export const evalThemeChanged = (global: GlobalState) => {
  const theme = createTheme(getDesignTokens(global === undefined ? THEME_INITIAL : global.theme))

  // https://www.geeksforgeeks.org/how-to-dynamically-update-scss-variables-using-reactjs/

  const root = document.documentElement;

  const isLightTheme = global.theme === THEME_LIGHT

  console.log('eval theme changed')

  root?.style.setProperty(CSS_VAR_TOOLBAR_TEXT_COLOR, isLightTheme ? 'white' : '#1976d2');
  
  return theme
}

export const getDesignTokens = (mode: PaletteMode) =>
({

  palette: {
    mode,
    ...(mode === "light"
      ? {
        star: "#3267ac",
      }
      : {
        star: "#a7a711",
      }),
  },

  typography: {
    fontFamily: "Roboto, Arial",

    button: {
      textTransform: 'none'
    },

  } as TypographyOptions,

  components: {

    MuiCssBaseline: {
      styleOverrides: `
          @font-face {
            font-family: 'Roboto', sans-serif;
            font-style: normal;
            font-weight: 300;            
          }
        `,
    },
  },

} as ThemeOptions);