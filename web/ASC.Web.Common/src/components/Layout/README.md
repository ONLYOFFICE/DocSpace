# Layout

Default page layout

### Usage

```js
import { Layout } from "asc-web-common";
```

```jsx
<Layout
  currentUser={currentUser}
  currentUserActions={currentUserActions}
  availableModules={availableModules}
  currentModuleId={currentModuleId}
  onLogoClick={onLogoClick}
  asideContent={asideContent}
  isBackdropVisible={false}
  isNavHoverEnabled={true}
  isNavOpened={false}
  isAsideVisible={false}
>
  PageLayout component content
</Layout>
```

### Properties

| Props                |   Type   | Required | Values | Default | Description                               |
| -------------------- | :------: | :------: | :----: | :-----: | ----------------------------------------- |
| `isBackdropVisible`  |  `bool`  |    -     |   -    | `false` | If you need display Backdrop              |
| `isNavHoverEnabled`  |  `bool`  |    -     |   -    | `true`  | If you need hover navigation on Backdrop  |
| `isNavOpened`        |  `bool`  |    -     |   -    | `false` | If you need display navigation            |
| `isAsideVisible`     |  `bool`  |    -     |   -    | `false` | If you need display aside                 |
| `currentUser`        | `object` |    -     |   -    | `null`  | Need for display current user information |
| `currentUserActions` | `array`  |    -     |   -    |  `[ ]`  | Actions for current user                  |
| `availableModules`   | `array`  |    -     |   -    |  `[ ]`  | Need for display modules list             |
| `currentModuleId`    | `string` |    -     |   -    |    -    | Current module                            |
| `onLogoClick`        |  `func`  |    -     |   -    |    -    | Click on logo                             |
| `asideContent`       |  `node`  |    -     |   -    |    -    | Content for aside display                 |
| `children`           |  `node`  |    -     |   -    |    -    | Main content                              |
