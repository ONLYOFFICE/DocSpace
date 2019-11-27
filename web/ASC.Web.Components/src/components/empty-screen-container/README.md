# EmptyScreenContainer

Used to display empty screen page

### Usage

```js
import { EmptyScreenContainer } from "asc-web-components";
```

```jsx
<EmptyScreenContainer
  imageSrc="empty_screen_filter.png"
  imageAlt="Empty Screen Filter image"
  headerText="No results matching your search could be found"
  descriptionText="No results matching your search could be found"
  buttons={<a href="/">Go to home</a>}
/>
```

### Properties

| Props             |   Type    | Required | Values | Default | Description                             |
| ----------------- | :-------: | :------: | :----: | :-----: | --------------------------------------- |
| `imageSrc`        | `string`  |    -     |   -    |    -    | Image url source                        |
| `imageAlt`        | `string`  |    -     |   -    |    -    | Alternative image text                  |
| `headerText`      | `string`  |    -     |   -    |    -    | Header text                             |
| `descriptionText` | `string`  |    -     |   -    |    -    | Description text                        |
| `buttons`         | `element` |    -     |   -    |    -    | Content of EmptyContentButtonsContainer |
