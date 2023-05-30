# ThemeProvider

Custom theme provider based on [Theme Provider](https://www.styled-components.com/docs/advanced).

List of themes:

- [Base theme](https://dotnet.onlyoffice.com:8084/?path=/story/components-themeprovider--base-theme)
- [Dark theme](https://dotnet.onlyoffice.com:8084/?path=/story/components-themeprovider--dark-theme)

You can change the CSS styles in the theme, and they will be applied to all children components of ThemeProvider

### Usage

```js
import ThemeProvider from "@docspace/components/theme-provider";
import Themes from "@docspace/components/themes";
```

```jsx
const newTheme = {...Themes.Base, color: "red"}

<ThemeProvider theme={newTheme}>
  <Box>
    <Text>Base theme</Text>
  </Box>
</ThemeProvider>;
```

### ThemeProvider Properties

| Props                |        Type         | Required | Values |    Default    | Description                                     |
| -------------------- | :-----------------: | :------: | :----: | :-----------: | ----------------------------------------------- |
| `theme`              |      `object`       |    ✅    |   -    | `Base styles` | Applies a theme to all children components      |
| `currentColorScheme` | `object`, `boolean` |    ✅    |   -    |       -       | Applies a colorTheme to all children components |
