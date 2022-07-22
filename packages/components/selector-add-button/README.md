# SelectorAddButton

### Usage

```js
import SelectorAddButton from "@docspace/components/selector-add-button";
```

```jsx
<SelectorAddButton
  title="Add item"
  onClick={() => console.log("onClose")}
></SelectorAddButton>
```

### Properties

| Props        |      Type      | Required | Values | Default | Description                                           |
| ------------ | :------------: | :------: | :----: | :-----: | ----------------------------------------------------- |
| `className`  |    `string`    |    -     |   -    |    -    | Attribute className                                   |
| `id`         |    `string`    |    -     |   -    |    -    | Accepts id                                            |
| `isDisabled` |     `bool`     |    -     |   -    | `false` | Tells when the button should present a disabled state |
| `onClick`    |   `function`   |    -     |   -    |    -    | What the button will trigger when clicked             |
| `style`      | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                     |
| `title`      |    `string`    |    -     |   -    |    -    | Title text                                            |
