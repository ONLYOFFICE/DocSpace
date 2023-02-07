# EmptyScreenContainer

Used to display empty screen page

### Usage

```js
import EmptyScreenContainer from "@docspace/components/empty-screen-container";
```

```jsx
<EmptyScreenContainer
  imageSrc="empty_screen_filter.png"
  imageAlt="Empty Screen Filter image"
  headerText="No results matching your search could be found"
  subheading="No files to be displayed in this section"
  descriptionText="No results matching your search could be found"
  buttons={<a href="/">Go to home</a>}
/>
```

### Properties

| Props             |      Type      | Required | Values | Default | Description                             |
| ----------------- | :------------: | :------: | :----: | :-----: | --------------------------------------- |
| `buttons`         |   `element`    |    -     |   -    |    -    | Content of EmptyContentButtonsContainer |
| `className`       |    `string`    |    -     |   -    |    -    | Accepts class                           |
| `descriptionText` |    `string`    |    -     |   -    |    -    | Description text                        |
| `headerText`      |    `string`    |    -     |   -    |    -    | Header text                             |
| `subheadingText`  |    `string`    |    -     |   -    |    -    | Subheading text                         |
| `id`              |    `string`    |    -     |   -    |    -    | Accepts id                              |
| `imageAlt`        |    `string`    |    -     |   -    |    -    | Alternative image text                  |
| `imageSrc`        |    `string`    |    -     |   -    |    -    | Image url source                        |
| `style`           | `obj`, `array` |    -     |   -    |    -    | Accepts css style                       |
