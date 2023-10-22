import { PaletteMode } from '@mui/material'

export const PostSetTheme = (theme: PaletteMode) => {
  console.log(`theme changed to ${theme}`)
  document.body.style.colorScheme = theme
}