# SelectedItem

### Usage

```js
import SelectedItem from "@docspace/components/selected-item";
```

```jsx
<SelectedItem
  text="sample text"
  onClick={() => console.log("onClose")}
></SelectedItem>
```

### Properties

| Props          |      Type      | Required | Values | Default | Description                                           |
| -------------- | :------------: | :------: | :----: | :-----: | ----------------------------------------------------- |
| `className`    |    `string`    |    -     |   -    |    -    | Accepts class                                         |
| `id`           |    `string`    |    -     |   -    |    -    | Accepts id                                            |
| `isDisabled`   |     `bool`     |    -     |   -    | `false` | Tells when the button should present a disabled state |
| `isInline`     |     `bool`     |    -     |   -    | `true`  | Sets the 'display: inline-block' property             |
| `onClose`      |     `func`     |    -     |   -    |    -    | What the selected item will trigger when clicked      |
| `style`        | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                     |
| `text`         |    `string`    |    -     |   -    |    -    | Selected item text                                    |
| `forwardedRef` |     `obj`      |    -     |   -    |    -    | Passes ref to component                               |
