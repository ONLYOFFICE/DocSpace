# DropDownItem

Is a item of DropDown or ContextMenu component

### Usage

```js
import MenuItem from "@appserver/components/menu-item";
```

```jsx
<MenuItem
  isSeparator={false}
  isHeader={false}
  label="Button 1"
  icon="static/images/nav.logo.react.svg"
  onClick={() => console.log("Button 1 clicked")}
/>
```

An item can act as separator, header, line, line with arrow or container.

When used as container, it will retain all styling features and positioning. To disable hover effects in container mode, you can use _noHover_ property.`

### Properties

| Props         |      Type      | Required | Values |     Default     | Description                                                |
| ------------- | :------------: | :------: | :----: | :-------------: | ---------------------------------------------------------- |
| `className`   |    `string`    |    -     |   -    |        -        | Accepts class                                              |
| `id`          |    `string`    |    -     |   -    |        -        | Accepts id                                                 |
| `icon`        |    `string`    |    -     |   -    |        -        | Dropdown item icon                                         |
| `label`       |    `string`    |    -     |   -    | `Dropdown item` | Dropdown item text                                         |
| `isHeader`    |     `bool`     |    -     |   -    |     `false`     | Tells when the dropdown item should display like header    |
| `isSeparator` |     `bool`     |    -     |   -    |     `false`     | Tells when the dropdown item should display like separator |
| `noHover`     |     `bool`     |    -     |   -    |     `false`     | Disable default style hover effect                         |
| `onClick`     |     `func`     |    -     |   -    |        -        | What the dropdown item will trigger when clicked           |
| `style`       | `obj`, `array` |    -     |   -    |        -        | Accepts css style                                          |
