# ViewSelector

Actions with a button.

### Usage

```js
import { ViewSelector } from "app-components";
import ViewRowsReactSvgUrl from "../../../public/images/view-rows.react.svg?url";
import ViewTilesReactSvgUrl from "../../../public/images/view-tiles.react.svg?url";
```

### View Settings

```js
const viewSettings = [
  {
    value: "row",
    icon: ViewRowsReactSvgUrl,
  },
  {
    value: "tile",

    icon: ViewTilesReactSvgUrl,
    callback: createThumbnails,
  },
];
```

```jsx
<ViewSelector
  isDisabled={false}
  onChangeView={(view) => console.log("current view:", view)}
  viewSettings={viewSettings}
  viewAs="row"
  isFilter={false}
/>
```

### Properties

| Props          |   Type   | Required | Values | Default | Description                                    |
| -------------- | :------: | :------: | :----: | :-----: | ---------------------------------------------- |
| `isDisabled`   |  `bool`  |    -     |   -    |    -    | Disables the button default functionality      |
| `isFilter`     |  `bool`  |    -     |   -    |    -    | Show only available option icon in selector    |
| `onChangeView` |  `func`  |    -     |   -    |    -    | The event triggered when the button is clicked |
| `viewSettings` |  `arr`   |    -     |   -    |    -    | Array containing view settings.                |
| `viewAs`       | `string` |    -     |   -    |    -    | Current application view                       |
